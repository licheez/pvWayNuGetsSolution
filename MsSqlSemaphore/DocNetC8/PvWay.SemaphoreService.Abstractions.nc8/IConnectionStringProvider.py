from typing import Protocol
from enum import Enum

class SqlRoleEnu(Enum):
    Application = 1

class IConnectionStringProvider(Protocol):
    async def get_connection_string_async(self, role: SqlRoleEnu = SqlRoleEnu.Application) -> str:
        pass