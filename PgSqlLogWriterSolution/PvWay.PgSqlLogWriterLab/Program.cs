using PvWay.PgSqlLogWriter.nc6;


Console.WriteLine("Hello, PgSqlLogWriter");

const string cs = "Server=localhost;" +
                  "Database=postgres;" +
                  "User Id=sa;" +
                  "Password=S0mePwd_;";

var lw = new PgSqlLogWriter(
    async () => await Task.FromResult(cs));

lw.WriteLog("userId", "companyId", "topic", "D",
    Environment.MachineName, "member", "file", 1,
    "message", DateTime.UtcNow);

