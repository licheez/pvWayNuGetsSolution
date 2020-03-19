# Method Result Wrapper
## Version 2.0.0

Provides a generic wrapper that returns whether or not a method succeeded or failed carrying the method result on success or a list of notifications in case of failure.

## Interfaces

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

        /// <summary>
        /// Will throw new Exception(ErrorMessage)
        /// </summary>
        void Throw();
    }
    
    public interface IMethodResult<out T> : IMethodResult
    {
        T Data { get; }
    }

    public interface IMethodResultNotification
    {
        SeverityEnum Severity { get; }
        string Message { get; }
    }
    

```

### ILoggerService interface

* This nuget package also provides the ILoggerService interface and a ConsoleLogger concrete that supports the MethodResult object

* Remark: feel free to get the MsSqlLoggerService nuget package that implements this interface enabling to persist logs into a table of your choice in a Ms Sql database

```csharp

        /// <summary>
        /// Subsequent calls to the Log method will
        /// store the provided UserId and CompanyId
        /// into their corresponding columns
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="companyId"></param>
        void SetUser(string userId, string companyId = null);

        /// <summary>
        /// Topic is an optional extra column in the log
        /// that enables grouping logs. Subsequent calls to
        /// the Log method will store the provided
        /// topic into the corresponding column
        /// </summary>
        /// <param name="topic"></param>
        void SetTopic(string topic);

        // TOPIC LESS METHODS
        void Log(
            string message,
            SeverityEnum severity = SeverityEnum.Debug,
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string filePath = "",
            [CallerLineNumber] int lineNumber = -1);

        void Log(
            IEnumerable<string> messages,
            SeverityEnum severity,
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string filePath = "",
            [CallerLineNumber] int lineNumber = -1);

        void Log(
            IMethodResult result,
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string filePath = "",
            [CallerLineNumber] int lineNumber = -1);


        void Log(
            Exception e,
            SeverityEnum severity = SeverityEnum.Fatal,
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string filePath = "",
            [CallerLineNumber] int lineNumber = -1);


        // TOPIC LESS METHODS
        void Log(
            string message,
            string topic,
            SeverityEnum severity = SeverityEnum.Debug,
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string filePath = "",
            [CallerLineNumber] int lineNumber = -1);

        void Log(
            IEnumerable<string> messages,
            string topic,
            SeverityEnum severity,
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string filePath = "",
            [CallerLineNumber] int lineNumber = -1);

        void Log(
            IMethodResult result,
            string topic,
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string filePath = "",
            [CallerLineNumber] int lineNumber = -1);

        void Log(
            Exception e,
            string topic,
            SeverityEnum severity = SeverityEnum.Fatal,
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string filePath = "",
            [CallerLineNumber] int lineNumber = -1);

```

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
            : this(
                $"Exception: {e.GetDeepMessage()} // StackTrace: {e.StackTrace}", 
                severity)
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
    public class MethodResult<T> : MethodResult, IMethodResult<T>
    {
        public T Data { get; }

        /// <summary>
        /// Successful constructor that carries
        /// the result of the called method in the
        /// property Data of type T
        /// </summary>
        /// <param name="data"></param>
        public MethodResult(T data)
        {
            Data = data;
        }

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


```

## Usage
```csharp
using pvWay.MethodResultWrapper;
using System;

namespace MrwConsole
{
    internal static class Program
    {
        private static void Main( /*string[] args*/)
        {
            var ls = new ConsoleLogger();
            var userStore = new UserStore();

            var getFirstName = GetUserFirstName(
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

            Console.WriteLine("hit enter to quit");
            Console.ReadLine();
        }

        private static IMethodResult<string> GetUserFirstName(
            ILoggerService ls, IUserStore userStore,
            string userName)
        {
            // let's call the GetUser Method and see its result
            // the method returns a IMethodResult<IUser> object
            var getUser = GetUser(ls, userStore, userName);
            if (getUser.Failure)
            {
                // something bad happened
                // let's log this and return a
                // MethodResult object that will carry
                // the notifications collected by the getUser method
                ls.Log(getUser);
                return new MethodResult<string>(getUser);
            }

            // the user was found
            // let's get the user object from the getUser.Data
            var user = getUser.Data; // this returns an IUser

            var firstName = user.FirstName;

            // let's call the MethodResult success constructor
            // by passing the expected data type object (here 
            // a string)
            return new MethodResult<string>(firstName);
        }

        private static IMethodResult<IUser> GetUser(
            ILoggerService ls, IUserStore userStore,
            string userName)
        {
            try
            {
                var user = userStore.GetUser(userName);
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
                ls.Log(err);

                // let's return the MethodResult to the caller
                return err;
            }
            catch (Exception e)
            {
                // something raised an exception...
                // for example the data base might not be up
                // let's log this fatal error
                ls.Log(e);
                // let's construct and return a failure MethodResult
                // with the Fatal (technical error) severity
                // and the exception.
                return new MethodResult<IUser>(e);
            }
        }
    }
}
```
