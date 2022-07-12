namespace pvWay.MailJetClientNuGet.nc6;

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
    /// <param name="variables">an optional key value dictionary with your MailJet template variables</param>
    /// <param name="subject">an optional subject</param>
    /// <returns>IMailJetSendResult or null on failure</returns>
    Task<IMailJetSendResult> SendTemplateAsync(
        int templateId,
        string fromEmail, string fromName,
        string toEmail, string toName,
        IDictionary<string, string>? variables = null,
        string? subject = null);
}