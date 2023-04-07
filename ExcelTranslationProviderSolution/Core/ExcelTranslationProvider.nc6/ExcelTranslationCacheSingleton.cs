namespace ExcelTranslationProvider.nc6
{
    public class ExcelTranslationCacheSingleton : IExcelTranslationCache
    {
        private readonly IExcelTranslationService _ts;
        private readonly TimeSpan _sleepTime;
        private static volatile IExcelTranslationCache? _instance;
        private static readonly object Locker = new ();
        private DateTime? _cacheDate;

        private ExcelTranslationCacheSingleton(
            IExcelTranslationService ts,
            TimeSpan sleepTime)
        {
            _ts = ts;
            _sleepTime = sleepTime;
            Translations = ts.ReadTranslations();
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
                Thread.Sleep(_sleepTime);
            }
            // ReSharper disable once FunctionNeverReturns
        }

        public static IExcelTranslationCache GetInstance(
            IExcelTranslationService ts,
            TimeSpan sleepTime)
        {
            if (_instance != null) return _instance;
            lock (Locker)
            {
                _instance = new ExcelTranslationCacheSingleton(ts, sleepTime);
                // ReSharper disable once NonAtomicCompoundOperator
                return _instance;
            }
        }

        private IDictionary<string, string> GetTranslations(params string[] keys)
        {
            var key = string.Empty;
            foreach (var keyPart in keys)
            {
                if (!string.IsNullOrEmpty(key)) key += ".";
                key += keyPart;
            }

            return Translations.TryGetValue(key, out var translations) 
                ? translations 
                : new Dictionary<string, string>();
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

        public DateTime LastUpdateDateUtc => _cacheDate ?? _ts.LastUpdateDateUtc;

        public IDictionary<string, IDictionary<string, string>> Translations { get; private set; }

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

    }
}
