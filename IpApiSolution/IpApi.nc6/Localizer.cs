﻿using Newtonsoft.Json;
using pvWay.IpApi.nc6.interfaces;
using pvWay.IpApi.nc6.Model;

namespace pvWay.IpApi.nc6;

public class Localizer : ILocalizer
{
    private readonly string _apiKey;

    public Localizer(string apiKey)
    {
        _apiKey = apiKey;
    }

    public async Task<ILocalizerResult> LocalizeAsync(string ip)
    {
        using var httpClient = new HttpClient();
        var url = $"http://api.ipstack.com/{ip}?access_key={_apiKey}&hostname=1";
        try
        {
            var response = await httpClient.GetAsync(url);
            if (!response.IsSuccessStatusCode)
                throw new Exception(
                    $"Url {url} returns an error {response.StatusCode} - {response.ReasonPhrase}");

            var responseBody = await response.Content.ReadAsStringAsync();

            dynamic rd = JsonConvert.DeserializeObject(responseBody)!;
            if (rd == null)
                throw new Exception(
                    $"Url {url} returns an error. Response body is null");

            var error = rd.error;
            if (error != null)
            {
                string code = error.code;
                string type = error.type;
                string info = error.info;
                throw new Exception(
                    $"Url {url} returns an error. Code {code} - type {type} - info {info}");
            }

            var loc = new Localization(rd);
            return LocalizerResult.Succeeded(loc);
        }
        catch (Exception e)
        {
            return LocalizerResult.Failed(e);
        }
    }
}