# Ms Sql MultiPart VarChar for .Net Core

Persists multi part text values (dictionary string - string) into one single VARCHAR column into 
an Ms SQL Db (2014 or >=). 

The package contains a cSharp class for storing/retrieving the dictionary to/from 
the field and the SQL code for creating a scalar function for stored procedure implementation.

### Usage

#### Constructor

From the business layer of your applicaiton use the Dictionary constructor

``` csharp
	
	var dic = new Dictionary<string, string>()
	{
		{"en", "bear"},
		{"fr", "ours"}
	};
	IMpVarChar myMpVarChar = new MpVarChar(dic);

```

#### Persisting into the Db

Now let's persist this value in one single NVARCHAR(MAX) into the Db.

The following example prepares a simple SQL statement for a DAO implementation of the DAL but of course you may want to use this with the ORM of your choice (EF, NHibernate...)

``` csharp
	
    // convert myMpVarChar to a string for insertion into the Db.
    var mpText = myMpVarChar.ToString(); 
    
    // hum yes... in this case we should make sure we escape the single quotes if any
    mpText = mpText.Replace("'", "''");

    // now we can use this var into an insert statement
    var insertStatement = $"INSERT INTO [dbo].[MyTable] ([MpText]) VALUES ('{mpText}');";
    
    // The line above will generate the following text
    // INSERT INTO [dbo].[MyTable] ([MpText]) VALUES ('<en>bear</en><fr>ours</fr>');

    // for the simplicity i do not provide here the code executing this insert

```

The key value dictionnary is serialized to a single string that can be saved into the db into a VARCHAR(MAX) (or NVARCHAR(xxx)) column. Up to you to see if you need a MAX lenght or if a smaller column will do the job. the serialization cost is 4 char per dictionary entry. It takes the form '&lt;key&gt;::&lt;value&gt;::'. If the value of the key containst a ':' char it will be escaped with a '\' char. This should also be taken into consideration for determining the final size of the string.

#### Retrieving the data from the Db

``` csharp

  // (not shown) here above the SELECT code that populates the IDataRecord object 
  var ord = dataRecord.GetOrdinal("MpText");
  var retrievedMpText = dataRecord.GetString(ord); // let's retreive the raw text from the Db

  // time to deserialize
  var deserializeSucceeded = MpVarChar.TryDeserialize(
  	retrievedMpText, 
  	out var retrievedMpVarChar, 
  	out var deserializationResult);

  if (!deserializeSucceeded)
  {
  	Console.WriteLine("it failed");
  	Console.WriteLine(deserializationResult);
  	// log and throw
  }
  else
  {
  	// some ways to get the data    
    // using the Dicionnary
    var enVal = retrievedMpVarChar.MpDic["en"];
    Console.WriteLine(enVal);
	// ==> displays "bear"                
    
    // using the GetPartForKey method
    var frVal = retrievedMpVarChar.GetPartForKey("fr");
    Console.WriteLine(frVal);
	// ==> displays "ours"                
    
    // using the TryGetPartForKey method
    var deOk = retrievedMpVarChar.TryGetPartForKey("de", out var deVal);
    Console.WriteLine(deVal);
	// ==> displays "bear" taking de first key in the dic as default value                
  }

```

### SQL side
#### Create a function that you can use from a Stored Procedure

``` SQL

  CREATE FUNCTION [dbo].[FnGetTranslation] 
  (
      @str NVARCHAR(MAX),
      @key NVARCHAR(MAX)
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

    END
```

#### Call the function from any SQL SELECT

``` SQL

  SELECT [dbo].[FnGetTranslation]([MpText], 'en')
  FROM [dbo].[MyTable]
  ORDER BY [dbo].[FnGetTranslation]([MpText], 'en')
  
```

Thanks for reading so far :-)

