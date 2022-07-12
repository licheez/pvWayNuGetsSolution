namespace pvWay.MailJetClientNuGet.nc6;

public interface IMailJetMessageSendResult
{
    string Status { get; }
    string CustomId { get; }
    IEnumerable<IMailJetAddresseeSendResult> To { get; }
    IEnumerable<IMailJetAddresseeSendResult> Cc { get; }
    IEnumerable<IMailJetAddresseeSendResult> Bcc { get; }
}