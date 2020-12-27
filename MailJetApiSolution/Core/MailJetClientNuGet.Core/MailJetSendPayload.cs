using System.Collections.Generic;

// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable UnusedAutoPropertyAccessor.Global

namespace pvWay.MailJetClientNuGet.Core
{
    internal class MailJetSendPayload
    {
        public IEnumerable<MailJetMessage> Messages { get; }

        public MailJetSendPayload(MailJetMessage message)
        {
            Messages = new List<MailJetMessage> { message };
        }
    }
}
