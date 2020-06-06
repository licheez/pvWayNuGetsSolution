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
		{"en", "a nice text in English"},
		{"fr", "un autre texte en français"}
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
    // INSERT INTO [dbo].[MyTable] ([MpText]) VALUES ('en::a nice text in English::fr::un autre texte en français::');

    // for the simplicity i do not provide here the code executing this insert

```

The key value dictionnary is serialized to a single string that can be saved into the db into a VARCHAR(MAX) (or NVARCHAR(xxx)) column. Up to you to see if you need a MAX lenght or if a smaller column will do the job. the serialization cost is 4 char per dictionary entry. It takes the form '&lt;key&gt;::&lt;value&gt;::'. If the value of the key containst a ':' char it will be escaped with a '\' char. This should also be taken into consideration for determining the final size of the string.

#### Retrieving the data from the Db

``` csharp

  // here above the SELECT code that populate the IDataRecord object 
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
	// ==> displays "a nice text in English"                
    
    // using the GetPartForKey method
    var frVal = retrievedMpVarChar.GetPartForKey("fr");
    Console.WriteLine(frVal);
	// ==> displays "un autre texte en français"                
    
    // using the TryGetPartForKey method
    var deOk = retrievedMpVarChar.TryGetPartForKey("de", out var deVal);
    Console.WriteLine(deVal);
	// ==> displays "a nice text in English" taking de first key in the dic as default value                
  }

```

### SQL side
#### Create a function that you can use from a Stored Procedure

``` SQL

  CREATE FUNCTION [dbo].[FnGetTranslation] 
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

  END

```

#### Call the function from any SQL SELECT

``` SQL

  SELECT [dbo].[FnGetTranslation]([MpText], 'en')
  FROM [dbo].[MyTable]
  ORDER BY [dbo].[FnGetTranslation]([MpText], 'en')
  
```

Thanks for reading so far :-)

