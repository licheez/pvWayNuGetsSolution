import asyncio
from datetime import datetime, timedelta
from psycopg2 import connect, sql
from psycopg2.extensions import AsIs

class SemaphoreService:
    def __init__(self, config):
        self._schema_name = config.SchemaName
        self._table_name = config.TableName
        self._get_cs_async = config.GetCsAsync
        self._log_exception = config.LogException
        self._log_info = config.LogInfo
        
    NameField = "Name"
    OwnerField = "Owner"
    TimeoutInSecondsField = "TimeOutInSeconds"
    CreateDateUtcField = "CreateDateUtc"
    UpdateDateUtcField = "UpdateDateUtc"

    async def acquire_semaphore_async(self, name, owner, timeout):
        name = DaoHelper.truncate_then_escape(name, 50)

        cs = await self._get_cs_async()
        async with connect(cs) as cn:

            try:
                await cn.open()
                
                await self.create_table_if_not_exists_async(cn)
                
                owner = DaoHelper.truncate_then_escape(owner, 128)
                self.log_info(f"{owner} is acquiring {name}")

                utc_now = datetime.utcnow()
                sql_now = DaoHelper.get_timestamp(utc_now)
                timeout_in_seconds = int(timeout.total_seconds())
                insert_text = (
                    f"INSERT INTO \"{self._schema_name}\".\"{self._table_name}\" "
                    "("
                    f" \"{self.NameField}\", "
                    f" \"{self.OwnerField}\", "
                    f" \"{self.TimeoutInSecondsField}\", "
                    f" \"{self.CreateDateUtcField}\", "
                    f" \"{self.UpdateDateUtcField}\" "
                    ") VALUES ("
                    f"'{name}', "
                    f"'{owner}', "
                    f"{timeout_in_seconds}, "
                    f"{sql_now}, "
                    f"{sql_now}"
                    ")"
                )

                async with cn.cursor() as insert_cmd:
                    await insert_cmd.execute(insert_text)

                self.log_info(f"{owner} acquired {name}")
                return DbSemaphore(
                    SemaphoreStatusEnu.Acquired,
                    name, owner, timeout,
                    utc_now, utc_now)
                
            except Exception:
                # INSERT FAILED
                try:
                    self.log_info(f"{name} was not acquired")
                    f_semaphore = await self.get_semaphore_async(cn, name)
                    if f_semaphore is None:
                        self.log_info(f"{name} was released in the mean time")
                        # the semaphore was released (deleted) in the meantime
                        return DbSemaphore(
                            SemaphoreStatusEnu.ReleasedInTheMeanTime,
                            name, None, timeout,
                            utc_now, utc_now)

                    # the semaphore was found
                    time_elapsed = utc_now - f_semaphore.UpdateDateUtc
                    # if the elapsed time is less than the timeout limit
                    # consider the semaphore is still valid
                    if time_elapsed <= timeout:
                        self.log_info(f"{name} is still in use by {f_semaphore.Owner}")
                        return DbSemaphore(
                            SemaphoreStatusEnu.OwnedSomeoneElse,
                            f_semaphore)

                    # the elapsed time is greater than the timeout limit
                    # force the release of the semaphore
                    self.log_info(f"{name} force released")
                    await self.release_semaphore_async(cn, name)
                    return DbSemaphore(
                        SemaphoreStatusEnu.ForcedReleased,
                        f_semaphore)
                except Exception as e:
                    self._log_exception(e)
                    raise

    async def touch_semaphore_async(self, name):
        name = DaoHelper.truncate_then_escape(name, 50)
        cs = await self._get_cs_async()
        async with connect(cs) as cn:
            sql_now = DaoHelper.get_timestamp(datetime.utcnow())
            update_text = (
                f"UPDATE \"{self._schema_name}\".\"{self._table_name}\" "
                f"SET \"{self.UpdateDateUtcField}\" = {sql_now} "
                f"WHERE \"{self.NameField}\" = '{name}'"
            )

            try:
                await cn.open()
                async with cn.cursor() as update_cmd:
                    await update_cmd.execute(update_text)
                self.log_info(f"{name} touched")
            except Exception as e:
                self.log_info(update_text)
                self._log_exception(e)
                raise PvWayPgSqlSemaphoreException(e)

    async def release_semaphore_async(self, name):
        name = DaoHelper.truncate_then_escape(name, 50)
        cs = await self._get_cs_async()
        async with connect(cs) as cn:
            await cn.open()
            await self.create_table_if_not_exists_async(cn)

            try:
                await self.release_semaphore_async(cn, name)
                self.log_info(f"{name} released")
            except Exception as e:
                self._log_exception(e)
                raise

    async def get_semaphore_async(self, name):
        name = DaoHelper.truncate_then_escape(name, 50)
        cs = await self._get_cs_async()
        async with connect(cs) as cn:
            await cn.open()
            await self.create_table_if_not_exists_async(cn)

            try:
                return await self.get_semaphore_async(cn, name)
            except Exception as e:
                self._log_exception(e)
                raise

    async def isolate_work_async(self, semaphore_name, owner, timeout, work_async, notify=None, sleep_between_attempts_in_seconds=15):
        while True:
            si = await self.acquire_semaphore_async(semaphore_name, owner, timeout)
            if si.Status == SemaphoreStatusEnu.Acquired:
                try:
                    return await work_async()
                except Exception as e:
                    self._log_exception(e)
                    raise
                finally:
                    await self.release_semaphore_async(semaphore_name)

            if notify:
                notification = (
                    f"mutex {semaphore_name} is locked by {si.Owner} "
                    f"since {si.CreateDateUtc} UTC "
                    f"It will expire on {si.ExpiresAtUtc} UTC. "
                    f"Sleeping {sleep_between_attempts_in_seconds} seconds."
                )
                notify(notification)
            await asyncio.sleep(sleep_between_attempts_in_seconds)

    async def isolate_work_async_void(self, semaphore_name, owner, timeout, work_async, notify=None, sleep_between_attempts_in_seconds=15):
        await self.isolate_work_async(
            semaphore_name,
            owner,
            timeout,
            lambda: work_async(),
            notify,
            sleep_between_attempts_in_seconds)

    async def get_semaphore_async_internal(self, cn, name):
        select_text = (
            "SELECT "
            f" \"{self.OwnerField}\", "
            f" \"{self.TimeoutInSecondsField}\", "
            f" \"{self.CreateDateUtcField}\", "
            f" \"{self.UpdateDateUtcField}\" "
            f"FROM \"{self._schema_name}\".\"{self._table_name}\" "
            f"WHERE \"{self.NameField}\" = '{name}'"
        )

        async with cn.cursor() as select_cmd:
            await select_cmd.execute(select_text)

            row_found = await select_cmd.fetchone()
            if row_found is None:
                return None
            owner = row_found[0]
            timeout_i_seconds = row_found[1]
            timeout = timedelta(seconds=timeout_i_seconds)
            create_date_utc = row_found[2]
            update_date_utc = row_found[3]
            return DbSemaphore(
                SemaphoreStatusEnu.Acquired,
                name, owner, timeout,
                create_date_utc, update_date_utc)

    async def release_semaphore_async_internal(self, cn, name):
        delete_text = (
            f"DELETE FROM \"{self._schema_name}\".\"{self._table_name}\" "
            f"WHERE \"{self.NameField}\" = '{name}'"
        )
        async with cn.cursor() as delete_cmd:
            await delete_cmd.execute(delete_text)

    def log_info(self, info):
        if self._log_info:
            self._log_info(f"pvWaySemaphoreService: {info}")

    async def create_table_if_not_exists_async(self, cn):
        exists_command_text = (
            "SELECT 1 FROM information_schema.tables "
            f"   WHERE table_schema = '{self._schema_name}' "
            f"   AND   table_name = '{self._table_name}' "
        )

        async with cn.cursor() as exists_cmd:
            await exists_cmd.execute(exists_command_text)
            table_exists = await exists_cmd.fetchone()

        if table_exists:
            return

        self.log_info("schema or table does not exists yet")
        
        # need to be db owner for this to work
        try:
            self.log_info(f"creating schema {self._schema_name} if it does not yet exists")
            create_schema_text = f"CREATE SCHEMA IF NOT EXISTS \"{self._schema_name}\""
            async with cn.cursor() as schema_cmd:
                await schema_cmd.execute(create_schema_text)
                
            self.log_info(f"creating table {self._table_name}")
            create_command_text = (
                f"CREATE TABLE \"{self._schema_name}\".\"{self._table_name}\" ("
                f" \"{self.NameField}\" character varying (50) PRIMARY KEY, "
                f" \"{self.OwnerField}\" character varying (128) NOT NULL, "
                f" \"{self.TimeoutInSecondsField}\" integer NOT NULL, "
                f" \"{self.CreateDateUtcField}\" timestamp NOT NULL, "
                f" \"{self.UpdateDateUtcField}\" timestamp NOT NULL"
                ")"
            )
            async with cn.cursor() as table_cmd:
                await table_cmd.execute(create_command_text)
        except Exception as e:
            self._log_exception(e)
            raise PvWayPgSqlSemaphoreException(e)