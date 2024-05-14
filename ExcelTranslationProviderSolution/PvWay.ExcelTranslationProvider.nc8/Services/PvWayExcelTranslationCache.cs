using System.Text;
using PvWay.ExcelTranslationProvider.Abstractions.nc8;

namespace PvWay.ExcelTranslationProvider.nc8.Services;

public class PvWayExcelTranslationCache : 
    IPvWayExcelTranslationCache
{
    private readonly IPvWayExcelTranslationService _ts;
    private readonly TimeSpan _rescanInterval;
    public IDictionary<string, IDictionary<string, string>> Translations { get; private set; }
    public DateTime LastUpdateDateUtc => _ts.LastUpdateDateUtc;
    private DateTime? _cacheDate;
    private bool _stopRequested;
    
    public PvWayExcelTranslationCache(
        IPvWayExcelTranslationService ts,
        IPvWayExcelTranslationCacheConfig config)
    {
        _ts = ts;
        Translations = ts.ReadTranslations();
        _rescanInterval = config.RescanInterval;
        if (_rescanInterval == TimeSpan.Zero) return;
        var worker = new Thread(Updater)
        {
            Priority = ThreadPriority.Lowest
        };
        worker.Start();
    }

    private void Updater()
    {
        while (true)
        {
            var lastUpdate = _ts.LastUpdateDateUtc;

            if (_cacheDate == null || _cacheDate < lastUpdate)
            {
                try
                {
                    Translations = _ts.ReadTranslations();
                    _cacheDate = lastUpdate;
                }
                catch (Exception)
                {
                    // nop
                }
            }

            if (_stopRequested) break;
            Thread.Sleep(_rescanInterval);
        }
    }

    public void StopRescan()
    {
        _stopRequested = true;
    }

    public void RefreshNow()
    {
        Translations = _ts.ReadTranslations();
        _cacheDate = _ts.LastUpdateDateUtc;
    }

    public string GetTranslation(string languageCode, string keysString)
    {
        var keyParts = keysString.Split('.');
        var translation = GetTranslation(languageCode, keyParts);
        if (string.IsNullOrEmpty(translation))
            translation = $"{keysString} not found";
        return translation;
    }
    
    private string GetTranslation(string languageCode, params string[] keys)
    {
        var translations = GetTranslations(keys);
        if (translations.TryGetValue(languageCode, out var translation))
            return translation;
        return translations.Count > 0
            ? translations.Values.First()
            : string.Empty;
    }
    
    private IDictionary<string, string> GetTranslations(params string[] keys)
    {
        var sb = new StringBuilder();
        foreach (var keyPart in keys)
        {
            if (sb.Length > 0) sb.Append('.');
            sb.Append(keyPart);
        }
        var key = sb.ToString();

        return Translations.TryGetValue(key, out var translations) 
            ? translations 
            : new Dictionary<string, string>();
    }


}