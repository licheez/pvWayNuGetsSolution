using System.Collections.Generic;

// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable UnusedAutoPropertyAccessor.Global

namespace pvWay.MailJetClientNuGet.Fw
{
    internal class MailJetMessage
    {
        public MailJetEmailAddress From { get; }
        public IEnumerable<MailJetEmailAddress> To { get; }
        public int TemplateId { get; }
        public bool TemplateLanguage { get; }
        public string Subject { get; }
        public IDictionary<string, string> Variables { get; }

        public MailJetMessage(
            MailJetEmailAddress fromAddress,
            MailJetEmailAddress toAddress,
            int templateId,
            string subject = null, 
            IDictionary<string, string> variables = null)
        {
            From = fromAddress;
            To = new List<MailJetEmailAddress> { toAddress };
            TemplateId = templateId;
            TemplateLanguage = true;
            Subject = subject;
            Variables = variables;
        }
    }
}
