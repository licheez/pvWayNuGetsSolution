﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace pvWay.ExcelTranslationProvider.Fw
{
    public class ExcelTranslationCacheSingleton : IExcelTranslationCache
    {
        private readonly IExcelTranslationService _ts;
        private readonly TimeSpan _sleepTime;
        private static volatile IExcelTranslationCache _instance;
        private static readonly object Locker = new object();
        private IDictionary<string, IDictionary<string, string>> _translations;
        private DateTime? _cacheDate;

        private ExcelTranslationCacheSingleton(
            IExcelTranslationService ts,
            TimeSpan sleepTime)
        {
            _ts = ts;
            _sleepTime = sleepTime;
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
                        _translations = _ts.Translations;
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
                _instance = new
                    ExcelTranslationCacheSingleton(ts, sleepTime);
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

            return _translations.ContainsKey(key)
                ? _translations[key]
                : new Dictionary<string, string>();
        }

        private string GetTranslation(string languageCode, params string[] keys)
        {
            var translations = GetTranslations(keys);
            if (translations.ContainsKey(languageCode))
                return translations[languageCode];
            return translations.Count > 0 
                ? translations.Values.First() 
                : string.Empty;
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
