using System.Net;
using PvWay.LoggerService.Abstractions.nc6;

// ReSharper disable MemberCanBeProtected.Global
// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable UnusedAutoPropertyAccessor.Global

namespace PvWay.LoggerService.MethodResultWrapper.nc6;

/// <summary>
/// Downstream object Http Result
/// </summary>
public class DsoHttpResult 
{
    public HttpStatusCode HttpStatusCode =>
        Status == SeverityEnu.Fatal
        || Status == SeverityEnu.Error
            ? HttpStatusCode.InternalServerError
            : HttpStatusCode.OK;

    internal SeverityEnu Status => EnumSeverity.GetValue(StatusCode);

    public string StatusCode { get; }
    public string MutationCode { get; }
    public ICollection<DsoHttpResultNotification> Notifications { get; }
    public bool HasMoreResults { get; }

    /// <summary>
    /// Successful constructor 
    /// </summary>
    public DsoHttpResult()
    {
        StatusCode = EnumSeverity.GetCode(SeverityEnu.Ok);
        Notifications = new List<DsoHttpResultNotification>();
        MutationCode = EnumDsoHttpResultMutation
            .GetCode(DsoHttpResultMutationEnu.None);
    }

    /// <summary>
    /// Enables to set the StatusCode
    /// </summary>
    /// <param name="severity"></param>
    public DsoHttpResult(SeverityEnu severity)
    {
        StatusCode = EnumSeverity.GetCode(severity);
        Notifications = new List<DsoHttpResultNotification>();
        MutationCode = EnumDsoHttpResultMutation
            .GetCode(DsoHttpResultMutationEnu.None);
    }

    /// <summary>
    /// Successful constructor passing the mutation type
    /// </summary>
    /// <param name="mutation"></param>
    public DsoHttpResult(DsoHttpResultMutationEnu mutation) :
        this(SeverityEnu.Ok, false, mutation)
    {
    }

    protected DsoHttpResult(
        SeverityEnu severity,
        bool hasMoreResults,
        DsoHttpResultMutationEnu mutation = DsoHttpResultMutationEnu.None) :
        this()
    {
        StatusCode = EnumSeverity.GetCode(severity);
        MutationCode = EnumDsoHttpResultMutation.GetCode(mutation);
        HasMoreResults = hasMoreResults;
    }

    /// <summary>
    /// Failure constructor passing back a MethodResult
    /// </summary>
    /// <param name="res"></param>
    public DsoHttpResult(
        IMethodResult res) :
        this(res.Severity, false)
    {
        Notifications = res.Notifications.Select(
                x => new DsoHttpResultNotification(x.Severity, x.Message))
            .ToList();
    }

    public DsoHttpResult(Exception e) :
        this(new MethodResult(e))
    {
    }

}

public class DsoHttpResult<T> : DsoHttpResult
{
    public T Data { get; }

    /// <summary>
    /// Successful constructor passing back some data
    /// </summary>
    /// <param name="data"></param>
    public DsoHttpResult(T data)
    {
        Data = data;
    }

    /// <summary>
    /// Successful constructor passing back some data and extra parameters
    /// </summary>
    /// <param name="data"></param>
    /// <param name="hasMoreResults"></param>
    public DsoHttpResult(
        T data,
        bool hasMoreResults) :
        base(SeverityEnu.Ok, hasMoreResults)
    {
        Data = data;
    }

    /// <summary>
    /// Successful constructor passing back some data and extra parameters
    /// </summary>
    /// <param name="data"></param>
    /// <param name="mutation"></param>
    public DsoHttpResult(
        T data,
        DsoHttpResultMutationEnu mutation) :
        base(SeverityEnu.Ok, false, mutation)
    {
        Data = data;
    }

    /// <summary>
    /// Failure constructor passing back a MethodResult
    /// </summary>
    /// <param name="res"></param>
    public DsoHttpResult(
        IMethodResult res) :
        base(res)
    {
        Data = default!;
    }

    public DsoHttpResult(Exception e) :
        base(e)
    {
        Data = default!;
    }
}