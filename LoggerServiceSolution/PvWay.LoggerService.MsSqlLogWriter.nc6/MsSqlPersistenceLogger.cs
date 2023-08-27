using PvWay.LoggerService.Abstractions.nc6;
// ReSharper disable ExplicitCallerInfoArgument

namespace PvWay.LoggerService.MsSqlLogWriter.nc6;

internal class MsSqlPersistenceLogger : IPvWayMsSqlLoggerService
{
    private readonly ILoggerService _ls;

    public MsSqlPersistenceLogger(ILoggerService ls)
    {
        _ls = ls;
    }
    public void Dispose()
    {
        GC.SuppressFinalize(this);
        _ls.Dispose();
    }

    public ValueTask DisposeAsync()
    {
        GC.SuppressFinalize(this);
        return _ls.DisposeAsync();
    }

    public void SetUser(string? userId, string? companyId = null)
    {
        _ls.SetUser(userId, companyId);
    }

    public void SetTopic(string? topic)
    {
        _ls.SetTopic(topic);
    }

    public void Log(
        string message, SeverityEnum severity = SeverityEnum.Debug,
        string memberName = "", string filePath = "",
        int lineNumber = -1)
    {
        _ls.Log(message, severity,
            memberName, filePath, lineNumber);
    }

    public async Task LogAsync(
        string message, SeverityEnum severity = SeverityEnum.Debug,
        string memberName = "", string filePath = "",
        int lineNumber = -1)
    {
        await _ls.LogAsync(message, severity,
            memberName, filePath, lineNumber);
    }

    public void Log(
        IEnumerable<string> messages, SeverityEnum severity,
        string memberName = "", string filePath = "",
        int lineNumber = -1)
    {
        _ls.Log(messages, severity,
            memberName, filePath, lineNumber);
    }

    public async Task LogAsync(
        IEnumerable<string> messages, SeverityEnum severity,
        string memberName = "", string filePath = "",
        int lineNumber = -1)
    {
        await _ls.LogAsync(messages, severity,
            memberName, filePath, lineNumber);
    }

    public void Log(
        Exception e, SeverityEnum severity = SeverityEnum.Fatal,
        string memberName = "", string filePath = "",
        int lineNumber = -1)
    {
        _ls.Log(e, severity,
            memberName, filePath, lineNumber);
    }

    public async Task LogAsync(
        Exception e, SeverityEnum severity = SeverityEnum.Fatal,
        string memberName = "", string filePath = "",
        int lineNumber = -1)
    {
        await _ls.LogAsync(e, severity,
            memberName, filePath, lineNumber);
    }

    public void Log(
        IMethodResult result, string memberName = "",
        string filePath = "", int lineNumber = -1)
    {
        _ls.Log(result, memberName, filePath, lineNumber);
    }

    public async Task LogAsync(
        IMethodResult result, string memberName = "",
        string filePath = "", int lineNumber = -1)
    {
        await _ls.LogAsync(result, memberName, filePath, lineNumber);
    }

    public void Log(
        string message, string? topic,
        SeverityEnum severity = SeverityEnum.Debug,
        string memberName = "", string filePath = "",
        int lineNumber = -1)
    {
        _ls.Log(message, topic, severity,
            memberName, filePath, lineNumber);
    }

    public async Task LogAsync(
        string message, string? topic,
        SeverityEnum severity = SeverityEnum.Debug,
        string memberName = "", string filePath = "",
        int lineNumber = -1)
    {
        await _ls.LogAsync(message, topic, severity,
            memberName, filePath, lineNumber);
    }

    public void Log(
        IEnumerable<string> messages, string? topic,
        SeverityEnum severity,
        string memberName = "", string filePath = "",
        int lineNumber = -1)
    {
        _ls.Log(messages, topic, severity,
            memberName, filePath, lineNumber);
    }

    public async Task LogAsync(
        IEnumerable<string> messages, string? topic,
        SeverityEnum severity,
        string memberName = "", string filePath = "",
        int lineNumber = -1)
    {
        await _ls.LogAsync(messages, topic, severity,
            memberName, filePath, lineNumber);
    }

    public void Log(
        Exception e, string? topic,
        SeverityEnum severity = SeverityEnum.Fatal,
        string memberName = "", string filePath = "",
        int lineNumber = -1)
    {
        _ls.Log(e, topic, severity,
            memberName, filePath, lineNumber);
    }

    public async Task LogAsync(
        Exception e, string? topic,
        SeverityEnum severity = SeverityEnum.Fatal,
        string memberName = "", string filePath = "",
        int lineNumber = -1)
    {
        await _ls.LogAsync(e, topic, severity,
            memberName, filePath, lineNumber);
    }

    public void Log(
        IMethodResult result, string? topic,
        string memberName = "", string filePath = "",
        int lineNumber = -1)
    {
        _ls.Log(result, topic,
            memberName, filePath, lineNumber);
    }

    public async Task LogAsync(
        IMethodResult result, string? topic,
        string memberName = "", string filePath = "",
        int lineNumber = -1)
    {
        await _ls.LogAsync(result, topic,
            memberName, filePath, lineNumber);
    }

}
