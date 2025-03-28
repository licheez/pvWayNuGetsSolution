# EnumConvert

## Introduction

Provides utility static methods for converting between enum values and their associated codes.
This class supports operations such as retrieving the primary code for an enum value,
finding the enum value corresponding to a given code, and handling descriptions with
multiple codes separated by commas.

## Usage

```csharp
using PvWayEnumConvNc8;

Console.WriteLine("Integration testing for EnumConvert");
Console.WriteLine("===================================");

// Getting the primary code representing the Severity Fatal
var codeForFatal = Severity.Fatal.GetCode();
Console.WriteLine($"The primary code for Severity.Fatal is '{codeForFatal}'.");

// Getting the severity value corresponding to the code 'V'
var severity = EnumConvert.GetValue<Severity>("V");
Console.WriteLine($"The Severity corresponding to code 'V' is '{severity}'");

// Getting the default value when the code cannot be found
var defaultValue = Severity.Info;
var severityOrDefault = EnumConvert.GetValue("X", defaultValue);
Console.WriteLine($"The Severity corresponding to code 'X' is '{severityOrDefault}'");

/* Console Output
 
    Integration testing for EnumConvert
    ===================================
    The primary code for Severity.Fatal is 'F'.
    The Severity corresponding to code 'V' is 'Trace'
    The Severity corresponding to code 'X' is 'Info'

*/

internal enum Severity {
    [System.ComponentModel.Description("F,Fatal,C,Crit,Critical")]
    Fatal,
    
    [System.ComponentModel.Description("E,Err,Error")]
    Error,
    
    [System.ComponentModel.Description("W,Warn,Warinng")]
    Warning,
    
    [System.ComponentModel.Description("I,Info")]
    Info,
    
    [System.ComponentModel.Description("D,Debug")]
    Debug,
    
    [System.ComponentModel.Description("T,Trc,Trace,V,Vrb,Verbose")]
    Trace,
}

```

## Methods

### GetCode
#### string GetCode(this Enum value)

```csharp
/// <summary>
/// <p>Gets the code (first term) from the raw description of the specified Enum value.</p>
/// <p>The raw description can contain a list of ',' separated terms (codes) for a given enum value</p>
/// </summary>
/// <param name="value">The Enum value from which to retrieve the code.</param>
/// <returns>Returns the code (first term) extracted from the raw description of the Enum value if available; otherwise, throws an exception.</returns>
/// <exception cref="ArgumentOutOfRangeException"></exception>
public static string GetCode(this Enum value)
{
    var rawDescription = GetRawDescription(value);
    if (!rawDescription.Contains(',')) return rawDescription;
    var codeList = rawDescription.Split(',', StringSplitOptions.TrimEntries);
    return codeList[0];
}
```

### GetValue
#### T GetValue<T>(string code) where T : Enum

```csharp
/// <summary>
/// Returns the Enum value that corresponds to the specified code based on the default Equality matcher
/// </summary>
/// <param name="code">The code to search for within the Enum values descriptions.</param>
/// <returns>The Enum value associated with the given code if found; otherwise, throws an ArgumentOutOfRangeException.</returns>
/// <exception cref="ArgumentOutOfRangeException"></exception>
public static T GetValue<T>(string code) where T : Enum
{
    return GetValue<T>(code, EqualityMatcher);
}
```

#### T GetValue<T>(string code, Func<string, string, bool> match)

```csharp
/// <summary>
/// Retrieves the Enum value based on the provided code using the specified match function.
/// </summary>
/// <param name="code">The code to search for within the Enum values descriptions.</param>
/// <param name="match">The custom match function used to determine the comparison between the code and Enum descriptions.</param>
/// <returns>The Enum value associated with the given code if found; otherwise, throws an ArgumentOutOfRangeException.</returns>
/// <exception cref="ArgumentOutOfRangeException"></exception>
public static T GetValue<T>(
    string code,
    Func<string, string, bool> match) where T : Enum
{
    if (string.IsNullOrEmpty(code))
        throw new ArgumentOutOfRangeException(
            nameof(code), code);
    var found = TryFindValue<T>(code, match, out var value);
    if (found) return value!;
    throw new ArgumentOutOfRangeException(
        nameof(code), code);
}
```

#### T GetValue<T>(string? code, T defaultValue)

```csharp
/// <summary>
/// Returns the Enum value that corresponds to the specified code based on the default Equality matcher
/// </summary>
/// <param name="code">The code to search for within the Enum values descriptions.</param>
/// <param name="defaultValue">The default Enum value to return if no matching value is found.</param>
/// <returns>The Enum value that corresponds to the specified code if found; otherwise, the default value.</returns>
public static T GetValue<T>(
    string? code,
    T defaultValue) where T : Enum => 
    GetValue(code, defaultValue, EqualityMatcher);
```

####  T GetValue<T>(string? code, T defaultValue, Func<string, string, bool> match)

```csharp
/// <summary>
/// Returns the Enum value that corresponds to the specified code based on a custom match function.
/// </summary>
/// <param name="code">The code to search for within the Enum values descriptions.</param>
/// <param name="defaultValue">The default Enum value to return if no matching value is found.</param>
/// <param name="match">A custom match function that defines the comparison logic between the code and Enum descriptions.</param>
/// <returns>The Enum value that corresponds to the specified code if found; otherwise, the default value.</returns>
public static T GetValue<T>(
    string? code,
    T defaultValue,
    Func<string, string, bool> match) where T : Enum
{
    if (string.IsNullOrEmpty(code)) return defaultValue;

    var found = TryFindValue<T>(code, match, out var value);

    return found ? value! : defaultValue;
}
```
