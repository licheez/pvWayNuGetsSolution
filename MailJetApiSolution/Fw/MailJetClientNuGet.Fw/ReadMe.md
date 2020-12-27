# MailJet API Client for .Net Framework by pvWay

## Preconditions

For using this nuGet you should obviously dispose of a MailJet account.

## Usage

### Interfaces

``` csharp
	
    public interface IMailJetClient
    {
        /// <summary>
        /// Post a mail using a template via the MailJet REST interface
        /// </summary>
        /// <param name="templateId">The unique id of you MailJet template</param>
        /// <param name="fromEmail">The email of the sender</param>
        /// <param name="fromName">The name of the sender</param>
        /// <param name="toEmail">The email of the addressee</param>
        /// <param name="toName">The name of the addressee</param>
        /// <param name="variables">key value dictionary with your MailJet template variables</param>
        /// <param name="subject"></param>
        /// <returns>IMailJetSendResult or null on failure</returns>
        Task<IMailJetSendResult> SendTemplateAsync(
            int templateId,
            string fromEmail, string fromName,
            string toEmail, string toName,
            IDictionary<string, string> variables = null,
            string subject = null);
    }

```

### Constructor

From the business layer of your application you start by instanciating the MailJet client

``` csharp
	
            var mjClient = new MailJetClient(
                apiKey: "******", // replace with your own MailJet ApiKey
                apiSecret:"******", // replace with your own MailJet ApiSecret
                logError: Console.WriteLine); // using Console as Logger

```
### Posting an email using a MailJet template

``` csharp
	
            IMailJetSendResult res = mjClient.SendTemplateAsync(
                templateId: 12345678,
                fromEmail: "invoicing@pvway.com",
                fromName: "pvWay SRL",
                toEmail: "someOne@someDomain",
                toName: "Mr someOne",
                variables: new Dictionary<string, string>
                {
                    {"invoiceNumber", "202015436"},
                    {"invoiceDate", "2020 Dec 25"},
                    {"product", "Merry Christmas Gift"},
                    {"price", "$ 125.33"}
                },
                // use the default template subject
                subject: null)
                .Result; 
            if (res.Success)
            {
                foreach (var resMessage in res.Messages)
                {
                    Console.WriteLine($"status: {resMessage.Status}");
                    foreach (var recipientRes in resMessage.To)
                    {
                        Console.WriteLine(
                            $"{recipientRes.Email} - {recipientRes.MessageUuid}");
                    }
                }
            }
            else
            {
                Console.WriteLine(res.Exception);
            }
```

Happy coding :-)
