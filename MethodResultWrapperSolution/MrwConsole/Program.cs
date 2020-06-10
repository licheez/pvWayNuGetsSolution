using System;
using pvWay.MethodResultWrapper.Enums;
using pvWay.MethodResultWrapper.Interfaces;
using pvWay.MethodResultWrapper.Model;

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

namespace MrwConsole
{
    interface IUser
    {
        string FirstName { get; }
    }

    internal interface IUserStore
    {
        IUser GetUser(string userName);
    }

    internal class UserStore : IUserStore
    {
        public IUser GetUser(string userName)
        {
            throw new Exception();
            // return null;
        }
    }
}
