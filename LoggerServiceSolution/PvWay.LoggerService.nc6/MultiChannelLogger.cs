using PvWay.LoggerService.Abstractions.nc6;
// ReSharper disable ExplicitCallerInfoArgument

namespace PvWay.LoggerService.nc6;

internal class MultiChannelLogger: IPvWayMultiChannelLoggerService
{
    private readonly IEnumerable<ILoggerService> _loggerServices;

    public MultiChannelLogger(IEnumerable<ILoggerService> loggerServices)
    {
        _loggerServices = loggerServices;
    }

    public void Dispose()
    {
        GC.SuppressFinalize(this);
        foreach (var ls in _loggerServices)
        {
            ls.Dispose();
        }
    }

    public ValueTask DisposeAsync()
    {
        GC.SuppressFinalize(this);
        foreach (var ls in _loggerServices)
        {
            ls.Dispose();
        }
        return ValueTask.CompletedTask;
    }

    public void SetUser(string? userId, string? companyId = null)
    {
        foreach (var ls in _loggerServices)
        {
            ls.SetUser(userId, companyId);
        }
    }

    public void SetTopic(string? topic)
    {
        foreach (var ls in _loggerServices)
        {
            ls.SetTopic(topic);
        }
    }

    public void Log(
        string message, SeverityEnum severity = SeverityEnum.Debug, 
        string memberName = "", string filePath = "",
        int lineNumber = -1)
    {
        foreach (var ls in _loggerServices)
        {
            ls.Log(message, severity, 
                memberName, filePath, lineNumber);
        }
    }

    public async Task LogAsync(
        string message, SeverityEnum severity = SeverityEnum.Debug, 
        string memberName = "", string filePath = "",
        int lineNumber = -1)
    {
        foreach (var ls in _loggerServices)
        {
            await ls.LogAsync(message, severity,
                memberName, filePath, lineNumber);
        }
    }

    public void Log(
        IEnumerable<string> messages, SeverityEnum severity, 
        string memberName = "", string filePath = "",
        int lineNumber = -1)
    {
        var messageList = messages.ToList();
        foreach (var ls in _loggerServices)
        {
            ls.Log(messageList, severity,
                memberName, filePath, lineNumber);
        }
    }

    public async Task LogAsync(
        IEnumerable<string> messages, SeverityEnum severity, 
        string memberName = "", string filePath = "",
        int lineNumber = -1)
    {
        var messageList = messages.ToList();
        foreach (var ls in _loggerServices)
        {
            await ls.LogAsync(messageList, severity,
                memberName, filePath, lineNumber);
        }
    }

    public void Log(
        Exception e, SeverityEnum severity = SeverityEnum.Fatal, 
        string memberName = "", string filePath = "",
        int lineNumber = -1)
    {
        foreach (var ls in _loggerServices)
        {
            ls.Log(e, severity,
                memberName, filePath, lineNumber);
        }
    }

    public async Task LogAsync(
        Exception e, SeverityEnum severity = SeverityEnum.Fatal, 
        string memberName = "", string filePath = "",
        int lineNumber = -1)
    {
        foreach (var ls in _loggerServices)
        {
            await ls.LogAsync(e, severity,
                memberName, filePath, lineNumber);
        }
    }

    public void Log(
        IMethodResult result, string memberName = "", 
        string filePath = "", int lineNumber = -1)
    {
        foreach (var ls in _loggerServices)
        {
            ls.Log(result,
                memberName, filePath, lineNumber);
        }
    }

    public async Task LogAsync(
        IMethodResult result, string memberName = "", 
        string filePath = "", int lineNumber = -1)
    {
        foreach (var ls in _loggerServices)
        {
            await ls.LogAsync(result,
                memberName, filePath, lineNumber);
        }
    }

    public void Log(
        string message, string? topic, SeverityEnum severity = SeverityEnum.Debug, 
        string memberName = "", string filePath = "", 
        int lineNumber = -1)
    {
        foreach (var ls in _loggerServices)
        {
            ls.Log(message, topic, severity,
                memberName, filePath, lineNumber);
        }
    }

    public async Task LogAsync(
        string message, string? topic, SeverityEnum severity = SeverityEnum.Debug, 
        string memberName = "", string filePath = "", 
        int lineNumber = -1)
    {
        foreach (var ls in _loggerServices)
        {
            await ls.LogAsync(message, topic, severity,
                memberName, filePath, lineNumber);
        }
    }

    public void Log(
        IEnumerable<string> messages, string? topic, SeverityEnum severity, 
        string memberName = "", string filePath = "",
        int lineNumber = -1)
    {
        var messageList = messages.ToList();
        foreach (var ls in _loggerServices)
        {
            ls.Log(messageList, topic, severity,
                memberName, filePath, lineNumber);
        }
    }

    public async Task LogAsync(
        IEnumerable<string> messages, string? topic, SeverityEnum severity, 
        string memberName = "", string filePath = "",
        int lineNumber = -1)
    {
        var messageList = messages.ToList();
        foreach (var ls in _loggerServices)
        {
            await ls.LogAsync(messageList, topic, severity,
                memberName, filePath, lineNumber);
        }
    }

    public void Log(
        Exception e, string? topic, SeverityEnum severity = SeverityEnum.Fatal, 
        string memberName = "", string filePath = "", 
        int lineNumber = -1)
    {
        foreach (var ls in _loggerServices)
        {
            ls.Log(e, topic, severity,
                memberName, filePath, lineNumber);
        }
    }

    public async Task LogAsync(
        Exception e, string? topic, SeverityEnum severity = SeverityEnum.Fatal, 
        string memberName = "", string filePath = "",
        int lineNumber = -1)
    {
        foreach (var ls in _loggerServices)
        {
            await ls.LogAsync(e, topic, severity,
                memberName, filePath, lineNumber);
        }
    }

    public void Log(
        IMethodResult result, string? topic, 
        string memberName = "", string filePath = "", 
        int lineNumber = -1)
    {
        foreach (var ls in _loggerServices)
        {
            ls.Log(result, topic,
                memberName, filePath, lineNumber);
        }
    }

    public async Task LogAsync(
        IMethodResult result, string? topic, 
        string memberName = "", string filePath = "", 
        int lineNumber = -1)
    {
        foreach (var ls in _loggerServices)
        {
            await ls.LogAsync(result, topic,
                memberName, filePath, lineNumber);
        }
    }
}