# pvWay PostgreSQL DAO services .Net Core 6

This nuGet is tiny subset of DAO services based on PostgreSql

It includes the following classes implementing the abstractions of pvWay Dao Service nuGet

## Classes

### Dependency injection

For the dependency injection you need to use the ServiceCollection extension

AddPvWayMsSqlDaoService passing 

* an async delegate for logging any exception raised during execution 
* and the Ms SQL Connection string

### IDaoService

The DaoService interface contains the contract for two main 
classes:

* StoredProcExecutor that implements IDaoStoredProcExecutor
* TextCommandExecutor that implements IDaoTextCommandExecutor

Three async methods enable to wrap commands into transactions:
* BeginTransactionAsync 
* CommitTransactionAsync 
* RollbackTransactionAsync

``` csharp
public interface IDaoService : IAsyncDisposable
{
    string GetDatabaseName();
    IDaoStoredProcExecutor StoredProcExecutor { get; }
    IDaoTextCommandExecutor TextCommandExecutor { get; }
    Task<IDbTransaction> BeginTransactionAsync();
    Task CommitTransactionAsync();
    Task RollbackTransactionAsync();
}
```

### IDaoCommandExecutor

This interface includes two simple methods

* CreateCommand within or without an associated transaction
* Execute a command that returns a scalar object

``` csharp
public interface IDaoCommandExecutor
{
    IDbCommand CreateCommand(string commandText,
        IDbTransaction? transaction = null);
    Task<object?> ExecuteScalarAsync(IDbCommand cmd);
}
```

### IDaoTextCommandExecutor

This extends the IDaoCommandExecutor with two async methods

* Execute Reader, a generic that returns a IEnumerable of object by passing a command and a factory
* Execute NonQueryAsync that executes a command

``` csharp
public interface IDaoTextCommandExecutor : IDaoCommandExecutor
{
    Task<IEnumerable<T>> ExecuteReaderAsync<T>(
        Func<IDaoReader, T> factor,
        IDbCommand cmd);

    Task ExecuteNonQueryAsync(IDbCommand cmd);
}
```

### IDaoStoredProcExecutor

This extends the IDaoTextCommandExecutor with one method enabling
to pass some string parameters to the stored proc

``` csharp
public interface IDaoStoredProcExecutor : IDaoTextCommandExecutor
{
    void AddStringParam(IDbCommand cmd, string paramName, string paramValue);
    // feel free to add more param types if needed
}
```

Enjoy it:=)
