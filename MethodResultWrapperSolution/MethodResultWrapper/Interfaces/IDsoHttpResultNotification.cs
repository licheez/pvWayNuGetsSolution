namespace pvWay.MethodResultWrapper.Interfaces
{
    public interface IDsoHttpResultNotification
    {
        string Message { get; }
        /// <summary>
        /// Severity O (ok) W (warning) E (error) F (fatal)...
        /// </summary>
        string SeverityCode { get; }
    }
}