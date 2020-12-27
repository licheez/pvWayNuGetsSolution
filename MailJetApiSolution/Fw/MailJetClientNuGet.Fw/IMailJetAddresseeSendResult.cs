using System;

namespace pvWay.MailJetClientNuGet.Fw
{
    public interface IMailJetAddresseeSendResult
    {
        string Email { get; }
        Guid MessageUuid { get; }
        long MessageId { get; }
        string MessageHRef { get; }
    }
}