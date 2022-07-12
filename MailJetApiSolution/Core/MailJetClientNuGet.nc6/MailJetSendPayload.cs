
// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable UnusedAutoPropertyAccessor.Global

namespace pvWay.MailJetClientNuGet.nc6;

internal class MailJetSendPayload
{
    public IEnumerable<MailJetMessage> Messages { get; }

    public MailJetSendPayload(MailJetMessage message)
    {
        Messages = new List<MailJetMessage> { message };
    }
}