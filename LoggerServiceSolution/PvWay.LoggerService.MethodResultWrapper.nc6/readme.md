# Method Result Wrapper DotNet Core 6 by pvWay

Provides a generic wrapper that returns whether or not a method succeeded or failed carrying the method result on success 
or a list of notifications in case of failure.

## Interfaces

Interfaces are defined in the [LoggerService.Abstractions for dotNet core 8](https://www.nuget.org/packages/PvWay.LoggerService.Abstractions.nc8/) nuGet

### MethodResult interfaces


```csharp
public interface IMethodResult
{
    /// <summary>
    /// At least one notification has a severity
    /// greater or equal to Error
    /// </summary>
    bool Failure { get; }

    /// <summary>
    /// No notification or all notifications severity
    /// are lower than Error
    /// </summary>
    bool Success { get; }

    SeverityEnum Severity { get; }

    /// <summary>
    /// Bulk string made of the concatenation
    /// of the notifications separated by new
    /// lines
    /// </summary>
    string ErrorMessage { get; }

    IEnumerable<IMethodResultNotification> Notifications { get; }

    void AddNotification(string message, SeverityEnum severity);
    void AddNotification(IMethodResultNotification notification);

    /// <summary>
    /// Will throw new Exception(ErrorMessage)
    /// </summary>
    void Throw();
}

public interface IMethodResult<out T> : IMethodResult
{
    T? Data { get; }
}

public interface IMethodResultNotification
{
    SeverityEnum Severity { get; }
    string Message { get; }
}

```

## Features

* **MethodResult** (implementing **IMethodResult**) is a class that
  * returns whether or not a method _succeeded_, has _fatal_, _errors_ or _warnings_
  * the returned object provides
    * a boolean property named Failure that will be set when at least on notification has a severity of _error_ or _fatal_
    * a boolean property named Success that is simply equals to !Failure
    * a list of notifications (message and severity)
    * an ErrorMessage string (list of notifications separated by new lines)
    * a method that allows to throw an exception

* **MethodResult&lt;T&gt;** (inheriting from **IMethodResult**) is a generic class that
  * returns an object of type **T** if the method succeeded


## Usage
```csharp
using System.Data;
using PvWay.LoggerService.Abstractions.nc8;
using PvWay.LoggerService.MethodResultWrapper.nc8;

namespace PvWay.MethodResultWrapperLab.nc8;

internal sealed class MethodResultWrapperDemo(
    ILoggerService ls,
    IUserStore userStore)
{
    public async Task<IMethodResult<string>> GetUserFirstNameAsync(
        string userName)
    {
        // let's call the GetUser Method and see its result
        // the method returns a IMethodResult<IUser> object
        var getUser = await GetUserAsync(userName);
        if (getUser.Failure)
        {
            // something wrong happened
            // let's log this and return a
            // MethodResult object that will carry
            // the notifications collected by the getUser method
            await ls.LogAsync(getUser);
            return new MethodResult<string>(getUser);
        }

        // the user was found
        // let's get the user object from the getUser.Data
        var user = getUser.Data!; // this returns an IUser

        var firstName = user.FirstName;

        // let's call the MethodResult success constructor
        // by passing the expected data type object (here 
        // a string)
        return new MethodResult<string>(firstName);
    }

    private async Task<IMethodResult<IUser>> GetUserAsync(
        string userName)
    {
        try
        {
            var user = await userStore.GetUserAsync(userName);
            if (user != null)
            {
                // the user was found
                // let's call the MethodResult success constructor
                // by passing the expected data type object (here 
                // a IUser object)
                return new MethodResult<IUser>(user);
            }

            // the user was not found...
            // this is a Business (non technical error)
            // let's construct a failure MethodResult object
            // with the Error (business error) severity
            var err = new MethodResult<IUser>(
                $"User {userName} not found", SeverityEnu.Error);

            // let's log this (business) error
            await ls.LogAsync(err);

            // let's return the MethodResult to the caller
            return err;
        }
        catch (Exception e)
        {
            // something raised an exception...
            // for example the data base might not be up
            // let's log this fatal error
            await ls.LogAsync(e);
            // let's construct and return a failure MethodResult
            // with the Fatal (technical error) severity
            // and the exception.
            return new MethodResult<IUser>(e);
        }
    }

}

internal interface IUser
{
    string FirstName { get; }
}

internal interface IUserStore
{
    Task<IUser?> GetUserAsync(string userName);
}

internal class UserStore : IUserStore
{
    public Task<IUser?> GetUserAsync(string userName)
    {
        throw new DataException();
    }
}
```

## Output

14:13:48 <span style="background-color: red;">FTL</span> Exception: Data Exception.<BR>
StackTrace:    at PvWay.MethodResultWrapperLab.nc8.UserStore.GetUserAsync(String userName) in C:\gitHub\pvWayNuGetsSolution\LoggerServiceSolution\PvWay.MethodResultWrapperLab.nc8\MethodResultWrapperDemo.cs:line 96
at PvWay.MethodResultWrapperLab.nc8.MethodResultWrapperDemo.GetUserAsync(String userName) in C:\gitHub\pvWayNuGetsSolution\LoggerServiceSolution\PvWay.MethodResultWrapperLab.nc8\MethodResultWrapperDemo.cs:line 44 from LNV14A in GetUserAsync (C:\gitHub\pvWayNuGetsSolution\LoggerServiceSolution\PvWay.MethodResultWrapperLab.nc8\MethodResultWrapperDemo.cs) line 72 at 12/13/2023 13:13:48

14:13:48 <span style="background-color: red;">FTL</span> Fatal:Exception: Data Exception.<BR>
StackTrace:    at PvWay.MethodResultWrapperLab.nc8.UserStore.GetUserAsync(String userName) in C:\gitHub\pvWayNuGetsSolution\LoggerServiceSolution\PvWay.MethodResultWrapperLab.nc8\MethodResultWrapperDemo.cs:line 96
at PvWay.MethodResultWrapperLab.nc8.MethodResultWrapperDemo.GetUserAsync(String userName) in C:\gitHub\pvWayNuGetsSolution\LoggerServiceSolution\PvWay.MethodResultWrapperLab.nc8\Me
thodResultWrapperDemo.cs:line 44 from LNV14A in GetUserFirstNameAsync (C:\gitHub\pvWayNuGetsSolution\LoggerServiceSolution\PvWay.MethodResultWrapperLab.nc8\MethodResultWrapperDemo.cs) line 23 at 12/13/2023 13:13:48

14:13:48 <span style="background-color: red;">FTL</span> Fatal:Exception: Data Exception.<BR>
StackTrace:    at PvWay.MethodResultWrapperLab.nc8.UserStore.GetUserAsync(String userName) in C:\gitHub\pvWayNuGetsSolution\LoggerServiceSolution\PvWay.MethodResultWrapperLab.nc8\MethodResultWrapperDemo.cs:line 96
at PvWay.MethodResultWrapperLab.nc8.MethodResultWrapperDemo.GetUserAsync(String userName) in C:\gitHub\pvWayNuGetsSolution\LoggerServiceSolution\PvWay.MethodResultWrapperLab.nc8\Me
thodResultWrapperDemo.cs:line 44 from LNV14A in <Main>$ (C:\gitHub\pvWayNuGetsSolution\LoggerServiceSolution\PvWay.MethodResultWrapperLab.nc8\Program.cs) line 26 at 12/13/2023 13:13:48


Happy coding !
