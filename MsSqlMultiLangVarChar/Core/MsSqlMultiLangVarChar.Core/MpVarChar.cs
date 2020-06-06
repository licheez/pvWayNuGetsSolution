using System;
using System.Collections.Generic;
using System.Linq;


/*

en::english text::fr::texte en français avec le caractère \: au milieu::nl::nederlandse tekst::

en::english text::
fr::texte en français avec le caractère \: au milieu::
nl::nederlandse tekst::

0 en
1 english text
2 fr
3 texte en français avec le caractère ''\:'' au milieu
4 nl
5 nederlandse text
*/


namespace pvWay.MsSqlMultiPartVarChar.Core
{
    /// <summary>
    /// Multipart Var Char Service.
    /// Persists multi lingual text values (dictionary iso639 - value)
    /// into one single VARCHAR column into an Ms SQL Db (2014 or >=)
    /// </summary>
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
                var parts = mpString.Split("::",
                    StringSplitOptions.RemoveEmptyEntries);
                for (var i = 0; i < parts.Length; i += 2)
                {
                    var key = parts[i];
                    var value = parts[i+1].Replace("\\:", ":");
                    dic.Add(key, value);
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

        public string GetPartForKey(string key)
        {
            var ok = MpDic.TryGetValue(key, out var value);
            return ok ? value : null;
        }

        public bool TryGetPartForKey(string key, out string value)
        {
            var ok = MpDic.TryGetValue(key, out value);
            if (ok) return true;
            if (MpDic.Count > 0)
                value = MpDic.Values.ToArray()[0];
            return false;
        }

        public override string ToString()
        {
            var res = string.Empty;
            foreach (var (key, val) in MpDic)
            {
                var value = val.Replace(":", "\\:");
                res += $"{key}::{value}::";
            }
            return res;
        }

        public static string CreateFunctionScript =>
            @"CREATE FUNCTION [dbo].[FnGetTranslation] 
                        (
	                        @str NVARCHAR(MAX),
	                        @lang VARCHAR(3)
                        )
                        RETURNS NVARCHAR(MAX)
                        AS
                        BEGIN

	                        IF @str IS NULL
	                        BEGIN
		                        RETURN NULL;
	                        END

                            /*
	                        'en::english text::fr::texte en français avec le caractère ''\:'' au milieu::nl::nederlandse tekst::'
	                        */

	                        DECLARE @a INT = LEN(@lang);
	                        DECLARE @p1 INT = CHARINDEX(@lang + '::', @str, 0);

	                        IF @p1 = 0
	                        BEGIN

		                        SET @p1 = CHARINDEX('::', @str, 0);
		                        SET @a = 0;

	                        END

	                        DECLARE @p2 INT = CHARINDEX('::', @str, @p1 + @a + 2);
	                        DECLARE @len INT = @p2 - @p1 - @a - 2;
	                        DECLARE @s NVARCHAR(MAX) = SUBSTRING(@str, @p1 + @a + 2, @len);
	                        SET @s = REPLACE(@s, '\:', ':');
	                        RETURN @s;

                        END";
    }
}
