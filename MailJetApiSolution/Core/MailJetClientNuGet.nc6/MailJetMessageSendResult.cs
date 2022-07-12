// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable UnusedAutoPropertyAccessor.Global

namespace pvWay.MailJetClientNuGet.nc6;

// ReSharper disable once ClassNeverInstantiated.Global
internal class MailJetMessageSendResult
    : IMailJetMessageSendResult
{
    public string Status { get; set; } = null!;
    public string CustomId { get; set; } = null!;

    public IEnumerable<MailJetAddresseeSendResult> To { get; set; } = null!;
    public IEnumerable<MailJetAddresseeSendResult> Cc { get; set; } = null!;
    public IEnumerable<MailJetAddresseeSendResult> Bcc { get; set; } = null!;

    IEnumerable<IMailJetAddresseeSendResult> IMailJetMessageSendResult.To => To;

    IEnumerable<IMailJetAddresseeSendResult> IMailJetMessageSendResult.Cc => Cc;

    IEnumerable<IMailJetAddresseeSendResult> IMailJetMessageSendResult.Bcc => Bcc;
}