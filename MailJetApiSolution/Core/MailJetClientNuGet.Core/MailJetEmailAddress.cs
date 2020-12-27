
// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable UnusedAutoPropertyAccessor.Global

namespace pvWay.MailJetClientNuGet.Core
{
    public class MailJetEmailAddress
    {
        public string Email { get; }
        public string Name { get; }

        public MailJetEmailAddress(string email, string name)
        {
            Email = email;
            Name = name;
        }
    }
}