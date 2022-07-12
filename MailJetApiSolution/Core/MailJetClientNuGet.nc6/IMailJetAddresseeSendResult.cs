namespace pvWay.MailJetClientNuGet.nc6;

public interface IMailJetAddresseeSendResult
{
    string Email { get; }
    Guid MessageUuid { get; }
    long MessageId { get; }
    string MessageHRef { get; }
}