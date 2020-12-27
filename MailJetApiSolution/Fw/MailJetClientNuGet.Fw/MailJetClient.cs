using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace pvWay.MailJetClientNuGet.Fw
{
    public class MailJetClient: IMailJetClient
    {
        private readonly Action<Exception> _logError;
        private readonly AuthenticationHeaderValue _authHeader;

        /// <summary>
        /// Provide your MailJet credentials here.
        /// </summary>
        /// <param name="apiKey">see MailJet</param>
        /// <param name="apiSecret">see MailJet</param>
        /// <param name="logError">a logger method of your choice</param>
        public MailJetClient(
            string apiKey,
            string apiSecret,
            Action<Exception> logError)
        {
            _logError = logError;
            var credString = $"{apiKey}:{apiSecret}";
            var credBytes = Encoding.ASCII.GetBytes(credString);
            var token = Convert.ToBase64String(credBytes);
            _authHeader = new AuthenticationHeaderValue("Basic", token);
        }

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
        public async Task<IMailJetSendResult> SendTemplateAsync(
            int templateId,
            string fromEmail, string fromName,
            string toEmail, string toName, 
            IDictionary<string, string> variables = null,
            string subject = null)
        {
            try
            {
                using (var httpClient = new HttpClient())
                {
                    var request = new HttpRequestMessage(
                        HttpMethod.Post, "https://api.mailjet.com/v3.1/send");
                    request.Headers.Authorization = _authHeader;

                    var message = new MailJetMessage(
                        new MailJetEmailAddress(fromEmail, fromName),
                        new MailJetEmailAddress(toEmail, toName),
                        templateId,
                        subject,
                        variables);

                    var payload = new MailJetSendPayload(message);

                    var settings = new JsonSerializerSettings
                    {
                        NullValueHandling = NullValueHandling.Ignore
                    };
                    var json = JsonConvert.SerializeObject(payload, settings);

                    request.Content = new StringContent(json);
                    request.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

                    var response = await httpClient.SendAsync(request);

                    if (response.IsSuccessStatusCode)
                    {
                        var jRes = await response.Content.ReadAsStringAsync();
                        var res = JsonConvert.DeserializeObject<MailJetSendResult>(jRes);
                        return res;
                    }
                    else
                    {
                        var err = await response.Content.ReadAsStringAsync();
                        var e = new Exception($"MailJet failed {err}");
                        _logError(e);
                        return MailJetSendResult.Failure(e);
                    }
                }

            }
            catch (Exception e)
            {
                _logError(e);
                return MailJetSendResult.Failure(e);
            }
        }
    }
}
