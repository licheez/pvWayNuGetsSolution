namespace pvWay.ExcelTranslationProvider.Fw
{
    public interface IExcelTranslationCache
    {
        /// <summary>
        /// key string should contain the keys separated by dots. example : 'enum.size'
        /// </summary>
        /// <param name="languageCode"></param>
        /// <param name="keysString"></param>
        /// <returns></returns>
        string GetTranslation(string languageCode, string keysString);
    }

}