using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace PvWay.MultiPartVarChar.nc8;

public class PvWayMpVarChar(
	IDictionary<string, string> mpDic) : IPvWayMpVarChar
{
    public IDictionary<string, string> MpDic { get; } = mpDic;

    private PvWayMpVarChar() : 
	    this(new Dictionary<string, string>())
    {
    }

    public string? GetPartForKey(string key)
    {
        var ok = MpDic.TryGetValue(key, out var value);
        return ok ? value : null;
    }

    public PvWayMpCharFindPartResult FindPartForKey(string key, out string? value)
    {
        var ok = MpDic.TryGetValue(key, out value);
        if (ok) 
	        return PvWayMpCharFindPartResult.ExactMatch;

        if (MpDic.Count == 0) 
	        return PvWayMpCharFindPartResult.NotFound;
        
        value = MpDic.Values.ToArray()[0];
        return PvWayMpCharFindPartResult.FirstValue;
    }
    
    public static PvWayMpCharDeserializationResult TryDeserialize(
        string mpString,
        out IPvWayMpVarChar? mpVarChar, 
        out string? errorMessage)
    {
        if (string.IsNullOrEmpty(mpString))
        {
            mpVarChar = new PvWayMpVarChar();
            errorMessage = null;
            return PvWayMpCharDeserializationResult.Empty;
        }
        try
        {
            var dic = new Dictionary<string, string>();

            var curPos = 0;

            while (true)
            {
                // 0123456789ABC
                // <en>bear</en>
                var ltPos = mpString.IndexOf('<', curPos); // 0
                if (ltPos < 0) break;
                var gtPos = mpString.IndexOf('>', ltPos); // 3
                var startValuePos = gtPos + 1; // 4
                var keyLen = gtPos - ltPos - 1; // 3 - 0 - 1 = 2
                var key = mpString.Substring(ltPos + 1, keyLen); // en
                var endTag = $"</{key}>"; // </en>
                var endTagPos = mpString.IndexOf(endTag, gtPos, StringComparison.InvariantCultureIgnoreCase); // 8
                var valueLen = endTagPos - startValuePos; // 8 - 4 = 4
                var value = mpString.Substring(startValuePos, valueLen); // bear
                key = Decode(key);
                value = Decode(value);
                dic.Add(key, value);
                curPos = endTagPos + keyLen + 2; // 8 + 2 + 2 = C
            }

            if (dic.Count == 0)
            {
                throw new PvWayMpVarCharException();
            }

            mpVarChar = new PvWayMpVarChar(dic);
            errorMessage = null;
            return PvWayMpCharDeserializationResult.Ok;
        }
        catch (Exception e)
        {
            errorMessage = e.Message;
            mpVarChar = null;
            return PvWayMpCharDeserializationResult.Failed;
        }
    }
    
    private static string Encode(string str)
    {
        str = str.Replace("&lt;", ";tl&", StringComparison.InvariantCultureIgnoreCase);
        str = str.Replace("&gt;", ";tg&", StringComparison.InvariantCultureIgnoreCase);
        str = str.Replace("<", "&lt;");
        str = str.Replace(">", "&gt;");
        return str;
    }

    private static string Decode(string str)
    {
        str = str.Replace("&lt;", "<");
        str = str.Replace("&gt;", ">");
        str = str.Replace(";tl&", "&lt;", StringComparison.InvariantCultureIgnoreCase);
        str = str.Replace(";tg&", "&gt;", StringComparison.InvariantCultureIgnoreCase);
        return str;
    }

    public override string ToString()
    {
        var sb = new StringBuilder();
        foreach (var (key, value) in MpDic)
        {
            var encodedValue = Encode(value);
            sb.Append($"<{key}>{encodedValue}</{key}>");
        }
        return sb.ToString();
    }
    
}