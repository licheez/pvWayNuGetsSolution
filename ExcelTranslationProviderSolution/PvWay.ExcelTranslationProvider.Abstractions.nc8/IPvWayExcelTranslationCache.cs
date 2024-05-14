namespace PvWay.ExcelTranslationProvider.Abstractions.nc8
{
    public interface IPvWayExcelTranslationCache
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
        IDictionary<string, IDictionary<string, string>> Translations { get; }

        /// <summary>
        /// Stop re-scanning the folder
        /// </summary>
        void StopRescan();
        
        /// <summary>
        /// Re-scan the folder for updated Excel files
        /// </summary>
        void RefreshNow();

        /// <summary>
        /// key string should contain the keys separated by dots. example : 'enum.size'
        /// </summary>
        /// <param name="languageCode"></param>
        /// <param name="keysString"></param>
        /// <returns></returns>
        string GetTranslation(string languageCode, string keysString);
    }

}