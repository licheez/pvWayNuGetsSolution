namespace PvWay.MultiPartVarChar.nc8;

public interface IPvWayMpVarChar
{
    /// <summary>
    /// Retrieves a dictionary from a string field.
    /// In the repository the information is coded using xml tags.
    /// Such as the following: 
    /// &lt;en&gt;english text&lt;/en&gt;
    /// &lt;fr&gt;texte en français&lt;/fr&gt;
    /// &lt;nl&gt;nederlandse text&lt;/nl&gt;
    /// This info will return a dictionary with the following entries
    /// {"key": "en", "value":"english text"},
    /// {"key": "fr", "value":"texte en français"},
    /// {"key": "nl", "value":"nederlandse text"},
    /// </summary>
    IDictionary<string, string> MpDic { get; }
    
    string? GetPartForKey(string key);
    
    /// <summary>
    /// Try to find the value for the given key.
    /// When not found value is set to the first
    /// value from the dictionary if any.
    /// </summary>
    /// <param name="key">the key to search for</param>
    /// <param name="value">
    /// the value corresponding to the key
    /// or the first value (default value) if any
    /// </param>
    /// <returns>ExactMatch or FirstValue or NotFound</returns>
    PvWayMpCharFindPartResult FindPartForKey(string key, out string? value);
}