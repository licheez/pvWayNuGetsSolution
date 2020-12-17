using System;
using System.Collections.Generic;
using System.Linq;

namespace pvWay.MsSqlMultiPartVarChar.Fw
{
    public interface IMpVarChar
    {
        IDictionary<string, string> MpDic { get; }
        string GetPartForKey(string key);
        /// <summary>
        /// Try to find the value for the given key.
        /// When not found value is set to the first
        /// value from the dictionary if any.
        /// </summary>
        /// <param name="key">the key to search for</param>
        /// <param name="value">the value corresponding to the key or the first value (default value) if any</param>
        /// <returns>true when in case of exact match</returns>
        bool TryGetPartForKey(string key, out string value);
    }

    public class MpVarChar : IMpVarChar
    {
        public IDictionary<string, string> MpDic { get; }

        private MpVarChar()
        {
            MpDic = new Dictionary<string, string>();
        }

        public MpVarChar(IDictionary<string, string> mpDic)
        {
            MpDic = mpDic;
        }

        public static bool TryDeserialize(string mpString,
            out IMpVarChar mpVarChar, out string result)
        {
            if (string.IsNullOrEmpty(mpString))
            {
                mpVarChar = new MpVarChar();
                result = "Empty";
                return true;
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
                    throw new Exception("invalid string");
                }

                result = "Ok";
                mpVarChar = new MpVarChar(dic);
                return true;
            }
            catch (Exception e)
            {
                result = e.Message;
                mpVarChar = null;
                return false;
            }
        }

        private static string Encode(string str)
        {
            str = str.Replace("&lt;", ";tl&");
            str = str.Replace("&gt;", ";tg&");
            str = str.Replace("<", "&lt;");
            str = str.Replace(">", "&gt;");
            return str;
        }

        private static string Decode(string str)
        {
            str = str.Replace("&lt;", "<");
            str = str.Replace("&gt;", ">");
            str = str.Replace(";tl&", "&lt;");
            str = str.Replace(";tg&", "&gt;");
            return str;
        }

        public string GetPartForKey(string key)
        {
            var ok = MpDic.TryGetValue(key, out var value);
            return ok ? value : null;
        }

        /// <summary>
        /// Try to find the value for the given key.
        /// When not found value is set to the first
        /// value from the dictionary if any.
        /// </summary>
        /// <param name="key">the key to search for</param>
        /// <param name="value">the value corresponding to the key or the first value (default value) if any</param>
        /// <returns>true when in case of exact match</returns>
        public bool TryGetPartForKey(string key, out string value)
        {
            var ok = MpDic.TryGetValue(key, out value);
            if (ok) return true;
            if (MpDic.Count > 0)
                value = MpDic.Values.ToArray()[0];
            return false;
        }

        /// <summary>
        /// Serialization
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            var res = string.Empty;
            foreach (var kvp in MpDic)
            {
                var encodedValue = Encode(kvp.Value);
                res += $"<{kvp.Key}>{encodedValue}</{kvp.Key}>";

            }
            return res;
        }

        public static string CreateFunctionScript(string schemaName, string functionName) =>
            $@"CREATE FUNCTION [{schemaName}].[{functionName}] 
            (
	            @str NVARCHAR(MAX),
	            @key VARCHAR(MAX)
            )
            RETURNS NVARCHAR(MAX)
            AS
            BEGIN

	            IF @str IS NULL
	            BEGIN
		            RETURN NULL;
	            END

                /*
                within the values the character '<' and '>' are replaced by '&lt;' and '&gt;'.
	            the keys are encoded as XML tags <key></key> 
	            '<en>english &lt;text&gt;</en><fr>texte en français</fr><nl>nederlandse tekst</nl>'
	            */
	            DECLARE @startTag NVARCHAR(MAX) = '<' + @key + '>';
	            DECLARE @startTagPos INT = CHARINDEX(@startTag, @str, 0);

	            if (@startTagPos = 0)
	            BEGIN
	                -- find the position of the first '>' in the string
		            DECLARE @gtPos INT = CHARINDEX('>', @str, 0);
		            -- set the key to the first key in the collection
		            SET @key = SUBSTRING(@str, 2, @gtPos - 2);
		            -- adjust startTag & pos
		            SET @startTag = '<' + @key + '>';
		            SET @startTagPos = 1;
	            END

	            DECLARE @startTagLen INT = LEN(@startTag);
	            DECLARE @valueStartPos INT = @startTagPos + @startTagLen;
	            DECLARE @endTag NVARCHAR(MAX) = '</' + @key + '>';
	            DECLARE @endTagPos INT = CHARINDEX(@endTag, @str, @valueStartPos);
	            DECLARE @valueLen INT = @endTagPos - @valueStartPos;
	            DECLARE @encodedValue NVARCHAR(MAX) = SUBSTRING(@str, @valueStartPos, @valueLen);
	            DECLARE @value NVARCHAR(MAX) = REPLACE(@encodedValue, '&lt;', '<');
	            SET @value = REPLACE(@value, ';tl&', '&lt;');
	            SET @value = REPLACE(@value, '&gt;', '>');
	            SET @value = REPLACE(@value, ';tg&', '&gt;');
	            RETURN @value;

            END";

    }
}
