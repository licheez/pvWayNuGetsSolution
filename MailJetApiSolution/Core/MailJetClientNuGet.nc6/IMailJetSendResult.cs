namespace pvWay.MailJetClientNuGet.nc6;

public interface IMailJetSendResult
{
    bool Success { get; }
    Exception Exception { get; }
    IEnumerable<IMailJetMessageSendResult> Messages { get; }
}