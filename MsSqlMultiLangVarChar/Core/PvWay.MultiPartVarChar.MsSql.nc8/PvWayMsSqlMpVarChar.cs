using System.Diagnostics.CodeAnalysis;
using PvWay.MultiPartVarChar.nc8;

namespace PvWay.MultiPartVarChar.MsSql.nc8;

public class PvWayMsSqlMpVarChar(IDictionary<string, string> mpDic) : 
	PvWayMpVarChar(mpDic)
{
    [SuppressMessage("ReSharper", "StringLiteralTypo")]
    public static string CreateFunctionScript(
	    string schemaName, string functionName) =>
        @$"CREATE FUNCTION [{schemaName}].[{functionName}] 
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
	            '<en>english &lt;text&gt;</en><fr>texte en français</fr><nl>nederlandse text</nl>'
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