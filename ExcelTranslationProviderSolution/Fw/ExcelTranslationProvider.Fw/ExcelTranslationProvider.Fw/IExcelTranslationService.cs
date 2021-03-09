using System;
using System.Collections.Generic;

namespace pvWay.ExcelTranslationProvider.Fw
{
    /// <summary>
    /// This service provides the mechanism for managing a cached
    /// translation dictionary at data consumer side
    /// </summary>
    public interface IExcelTranslationService : IDisposable
    {
        /// <summary>
        /// With this date any data consumer can verify
        /// whether or not its cached copy of the dictionary
        /// is still up to date and if needed refresh the
        /// cache by getting the Translation property.
        /// </summary>
        /// <returns></returns>
        DateTime LastUpdateDateUtc { get; }

        /// <summary>
        /// The key is build from the concatenation of
        /// up to 4 key parts separated by dots. Example: 'components.buttons.save').
        /// The associated value is dictionary languageCode:string
        /// </summary>
        IDictionary<string, IDictionary<string, string>> ReadTranslations();
    }
}