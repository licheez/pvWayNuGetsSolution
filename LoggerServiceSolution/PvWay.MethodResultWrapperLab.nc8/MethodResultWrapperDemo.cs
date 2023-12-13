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