# Method Result Wrapper Core DotNet 6 by pvWay

Provides a generic wrapper that returns whether or not a method succeeded or failed carrying the method result on success 
or a list of notifications in case of failure.

## Interfaces

### MethodResult interfaces

```csharp
namespace MethodResultWrapper.Nc6;

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

### ILoggerService interface

* This nuget package also provides the ILoggerService interface with 3 built in implementations. 
(1) ConsoleLogger that writes logs onto the Console,
(2) MuteLogger that can be used for unit testing and
(3) PersistenceLogger that can be used for persiting rich log rows into a database, a file or any other persistence layer
by injecting the appropriate LogWriter (see **[pvWay.MsSqlLogWriter.Core nuGet](https://www.nuget.org/packages/MsSqlLogWriter.Core/)**)

* The ILoggerService provides both sync and async methods with serveral signatures including
(1) simple message,
(2) list of messages,
(3) MethodResult object (see above)
(4) Exception

* Each log row is also qualified by a Severity level from Debug to Fatal and enables also some interesting meta data like
(1) UserId,
(2) CompanyId,
(3) Topic
(4) MachineName

* The service will also capture MemberName, FilePath and LineNumber

## Features

### MethodResult plain class (see MethodResult &lt;T&gt; generic class below)

* **MethodResult** (implementing **IMethodResult**) is a class that
 * returns whether or not a method succeeded, has fatals, errors or warnings
 * the returned object provides
   * a boolean property named Failure that will be set when at least on notification has a severity of error or fatal
   * a boolean property named Success that is simply equals to !Failure
   * a list of notifications (message and severity)
   * an ErrorMessage string (list of notifications separated by new lines)
   * a method that allows to throw an exception
* **Constructors**

```csharp
        
namespace MethodResultWrapper.Nc6;

public class MethodResult : IMethodResult
{
    private readonly ICollection<IMethodResultNotification> _notifications;

    private class Notification : IMethodResultNotification
    {
        public SeverityEnum Severity { get; }
        public string Message { get; }

        public Notification(SeverityEnum severity, string message)
        {
            Severity = severity;
            Message = message;
        }

        public override string ToString()
        {
            return $"{Severity}:{Message}";
        }
    }

    /// <summary>
    /// Successful constructor
    /// </summary>
    public MethodResult()
    {
        _notifications = new List<IMethodResultNotification>();
    }

    /// <summary>
    /// Wraps the result of a previous MethodResult
    /// by copying all its notifications (message and severity).
    /// Not really useful for the non generic MethodResult class
    /// but makes sense when using the generic version of the class
    /// </summary>
    /// <param name="res"></param>
    public MethodResult(IMethodResult res)
        : this()
    {
        foreach (var notification in res.Notifications)
        {
            AddNotification(notification);
        }
    }

    /// <summary>
    /// Instantiates a MethodResult object with one
    /// notification (message and severity)
    /// </summary>
    /// <param name="message"></param>
    /// <param name="severity"></param>
    public MethodResult(string message, SeverityEnum severity) :
        this()
    {
        AddNotification(message, severity);
    }

    /// <summary>
    /// Failure constructor that instantiates a MethodResult
    /// object with one notification. The notification message
    /// is built by recursively walking down the exception
    /// and its inner exceptions (using the extension method
    /// e.GetDeepMessage()). The stack trace is also added at
    /// the end of the message. By default the Severity is
    /// considered as Fatal but you select the one of your choice
    /// </summary>
    /// <param name="e"></param>
    /// <param name="severity"></param>
    public MethodResult(Exception e, SeverityEnum severity = SeverityEnum.Fatal)
        : this(e.GetDeepMessage(), severity)
    {
    }

    /// <summary>
    /// Same as the single message constructor but this time
    /// passing a list of messages. This creates one notification
    /// for each message in the list. All notifications get the
    /// same severity
    /// </summary>
    /// <param name="messages"></param>
    /// <param name="severity"></param>
    public MethodResult(IEnumerable<string> messages, SeverityEnum severity) :
        this()
    {
        foreach (var message in messages)
        {
            AddNotification(message, severity);
        }
    }

    public void AddNotification(string message, SeverityEnum severity)
    {
        AddNotification(new Notification(severity, message));
    }

    public void AddNotification(IMethodResultNotification notification)
    {
        _notifications.Add(notification);
    }

    /// <summary>
    /// At least one notification has a severity
    /// greater or equal to Error
    /// </summary>
    public bool Failure => _notifications
        .Any(n => n.Severity >= SeverityEnum.Error);

    /// <summary>
    /// No notification or all notifications severity
    /// are lower than Error
    /// </summary>
    public bool Success => !Failure;

    /// <summary>
    /// returns the highest severity from the list of notifications
    /// </summary>
    public SeverityEnum Severity =>
        _notifications.Any()
            ? _notifications.Max(x => x.Severity)
            : SeverityEnum.Ok;

    public IEnumerable<IMethodResultNotification> Notifications =>
        _notifications;

    public void Throw()
    {
        throw new Exception(ErrorMessage);
    }

    public string ErrorMessage => ToString();

    public override string ToString()
    {
        var str = string.Empty;
        foreach (var notification in _notifications)
        {
            if (!string.IsNullOrEmpty(str))
                str += Environment.NewLine;
            str += notification.ToString();
        }
        return str;
    }

    public static MethodResult Ok => new();
        
}

```

### MethodResult &lt;T&gt; generic class inheriting of MethodResult

* **MethodResult&lt;T&gt;** (implementing **IMethodResult&lt;T&gt;**) is a generic class that inherits from MethodResult plain class (see above)
 * returns whether or not a method succeeded, has fatals, errors or warnings
 * the returned object provides
   * a **Data** object property (of type T) that is set when the method has succeeded
   * a boolean property named **Failure** that will be set when at least on notification has a severity of error or fatal
   * a boolean property named **Success** that is simply equals to !Failure
   * a list of notifications (message and severity)
   * an **ErrorMessage** string (list of notifications separated by new lines)
   * a method that allows to throw an exception
* **Constructors**

```csharp

namespace MethodResultWrapper.Nc6;

public class MethodResult<T> : MethodResult, IMethodResult<T>
{
    public T? Data { get; }

    /// <summary>
    /// Successful constructor that carries
    /// the result of the called method in the
    /// property Data of type T
    /// </summary>
    /// <param name="data"></param>
    public MethodResult(T? data)
    {
        Data = data;
    }

    public static IMethodResult<T> Null => new MethodResult<T>(default(T));

    /// <summary>
    /// Wraps the result of a previous MethodResult
    /// by copying all its notifications (message and severity).
    /// Not really useful for the non generic MethodResult class
    /// but makes sense when using the generic version of the class
    /// </summary>
    /// <param name="methodResult"></param>
    public MethodResult(IMethodResult methodResult) :
        base(methodResult)
    {
    }

    /// <summary>
    /// Instantiates a MethodResult object with one
    /// notification (message and severity)
    /// </summary>
    /// <param name="message"></param>
    /// <param name="severity"></param>
    public MethodResult(string message, SeverityEnum severity)
        : base(message, severity)
    {
    }

    /// <summary>
    /// Same as the single message constructor but this time
    /// passing a list of messages. This creates one notification
    /// for each message in the list. All notifications get the
    /// same severity
    /// </summary>
    /// <param name="messages"></param>
    /// <param name="severity"></param>
    public MethodResult(IEnumerable<string> messages, SeverityEnum severity)
        : base(messages, severity)
    {
    }

    /// <summary>
    /// Failure constructor that instantiates a MethodResult
    /// object with one notification. The notification message
    /// is built by recursively walking down the exception
    /// and its inner exceptions (using the extension method
    /// e.GetDeepMessage()). The stack trace is also added at
    /// the end of the message. By default the Severity is
    /// considered as Fatal but you select the one of your choice
    /// </summary>
    /// <param name="e"></param>
    /// <param name="severity"></param>
    public MethodResult(Exception e, SeverityEnum severity = SeverityEnum.Fatal)
        : base(e, severity)
    {
    }

}
```

## Usage
```csharp
using pvWay.MethodResultWrapper.nc6;

namespace MethodResultWrapperDemo.Nc6;

internal static class Program
{
    private static async Task Main( /*string[] args*/)
    {
        var ls = new ConsoleLogger();
        var userStore = new UserStore();

        var getFirstName = await GetUserFirstNameAsync(
            ls, userStore, "pierre@pvWay.com");
        if (getFirstName.Failure)
        {
            Console.WriteLine("oops... something went wrong");
            Console.WriteLine(getFirstName.ErrorMessage);
        }
        else
        {
            var firstName = getFirstName.Data;
            Console.WriteLine("everything went fine");
            Console.WriteLine($"user first name is {firstName}");
        }
    }

    private static async Task<IMethodResult<string>> GetUserFirstNameAsync(
        ILoggerService ls, IUserStore userStore,
        string userName)
    {
        // let's call the GetUser Method and see its result
        // the method returns a IMethodResult<IUser?> object
        var getUser = await GetUserAsync(ls, userStore, userName);
        if (getUser.Failure)
        {
            // something bad happened
            // let's log this and return a
            // MethodResult object that will carry
            // the notifications collected by the getUser method
            await ls.LogAsync(getUser);
            return new MethodResult<string>(getUser);
        }

        // the user was found
        // let's get the user object from the getUser.Data
        var user = getUser.Data; // this returns an IUser

        var firstName = user!.FirstName;

        // let's call the MethodResult success constructor
        // by passing the expected data type object (here 
        // a string)
        return new MethodResult<string>(firstName);
    }

    private static async Task<IMethodResult<IUser>> GetUserAsync(
        ILoggerService ls, IUserStore userStore,
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
                $"User {userName} not found", SeverityEnum.Error);

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
        Console.WriteLine($"searching for user with username {userName}");
        throw new Exception();
        // return null;
    }
}```

Happy coding !
