// ReSharper disable UnusedAutoPropertyAccessor.Global

namespace pvWay.MailJetClientNuGet.nc6;

// ReSharper disable once ClassNeverInstantiated.Global
internal class MailJetAddresseeSendResult :
    IMailJetAddresseeSendResult
{
    public string Email { get; set; } = null!;
    public Guid MessageUuid { get; set; }
    public long MessageId { get; set; }
    public string MessageHRef { get; set; } = null!;
}