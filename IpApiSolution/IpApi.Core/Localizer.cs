using Newtonsoft.Json;
using pvWay.MethodResultWrapper.Core;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace pvWay.IpApi.Core
{
    public class Localizer : ILocalizer
    {
        private readonly string _apiKey;

        public Localizer(string apiKey)
        {
            _apiKey = apiKey;
        }

        public async Task<IMethodResult<ILocalization>> LocalizeAsync(string ip)
        {
            using var httpClient = new HttpClient();
            var url = $"http://api.ipstack.com/{ip}?access_key={_apiKey}&hostname=1";
            try
            {
                var response = await httpClient.GetAsync(url);
                if (!response.IsSuccessStatusCode)
                {
                    var err = new MethodResult<ILocalization>(
                        $"Url {url} returns an error {response.StatusCode} - {response.ReasonPhrase}",
                        SeverityEnum.Error);
                    return new MethodResult<ILocalization>(err);
                }

                var responseBody = await response.Content.ReadAsStringAsync();

                dynamic rd = JsonConvert.DeserializeObject(responseBody);

                if (rd == null)
                {
                    var err = new MethodResult<ILocalization>(
                        $"Url {url} returns an error. Response body is null",
                        SeverityEnum.Error);
                    return new MethodResult<ILocalization>(err);
                }

                var error = rd.error;
                if (error != null)
                {
                    string code = error.code;
                    string type = error.type;
                    string info = error.info;
                    var err = new MethodResult<ILocalization>(
                        $"Url {url} returns an error. Code {code} - type {type} - info {info}",
                        SeverityEnum.Error);
                    return new MethodResult<ILocalization>(err);
                }

                var loc = new Localization(rd);
                return new MethodResult<ILocalization>(loc);
            }
            catch (Exception e)
            {
                return new MethodResult<ILocalization>(e);
            }
        }
    }
}
