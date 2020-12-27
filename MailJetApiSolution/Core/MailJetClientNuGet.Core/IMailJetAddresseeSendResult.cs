using System;

namespace pvWay.MailJetClientNuGet.Core
{
    public interface IMailJetAddresseeSendResult
    {
        string Email { get; }
        Guid MessageUuid { get; }
        long MessageId { get; }
        string MessageHRef { get; }
    }
}