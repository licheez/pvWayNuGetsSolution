using System;

namespace pvWay.MailJetClientNuGet.Core
{
    internal class MailJetAddresseeSendResult :
        IMailJetAddresseeSendResult
    {
        public string Email { get; set; }
        public Guid MessageUuid { get; set; }
        public long MessageId { get; set; }
        public string MessageHRef { get; set; }
    }
}