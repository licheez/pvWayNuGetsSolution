using System.Collections.Generic;

namespace pvWay.MailJetClientNuGet.Fw
{
    internal class MailJetMessageSendResult
        : IMailJetMessageSendResult
    {
        public string Status { get; set; }
        public string CustomId { get; set; }
        
        public IEnumerable<MailJetAddresseeSendResult> To { get; set; }
        public IEnumerable<MailJetAddresseeSendResult> Cc { get; set; }
        public IEnumerable<MailJetAddresseeSendResult> Bcc { get; set; }

        IEnumerable<IMailJetAddresseeSendResult> IMailJetMessageSendResult.To => To;

        IEnumerable<IMailJetAddresseeSendResult> IMailJetMessageSendResult.Cc => Cc;

        IEnumerable<IMailJetAddresseeSendResult> IMailJetMessageSendResult.Bcc => Bcc;
    }
}