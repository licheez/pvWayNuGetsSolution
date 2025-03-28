using System.ComponentModel;

namespace PvWayEnumConvNc8;

/// <summary>
/// Provides utility static methods for converting between enum values and their associated codes.
/// This class supports operations such as retrieving the primary code for an enum value,
/// finding the enum value corresponding to a given code, and handling descriptions with
/// multiple codes separated by commas.
/// </summary>
public static class EnumConvert
{
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
    
    /// <summary>
    /// <p>Gets the raw description of the specified Enum value.</p>
    /// <p>The description can contain a list of ',' separated
    /// terms (codes) for a given enum value</p>
    /// </summary>
    /// <param name="value">The Enum value to retrieve the raw description from.</param>
    /// <returns>Returns the raw description of the Enum value if available;
    /// otherwise, throws an exception.</returns>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    private static string GetRawDescription(this Enum value)
    {
        var fieldInfo = value.GetType()
            .GetField(value.ToString())!;
        DescriptionAttribute[]? descriptionAttributes = null;
        try
        {
            descriptionAttributes = (DescriptionAttribute[]?)fieldInfo
                .GetCustomAttributes(
                    typeof(DescriptionAttribute), false);
        }
        catch (Exception)
        {
            // nop
        }

        if (descriptionAttributes is not null
            && descriptionAttributes.Length > 0)
            return descriptionAttributes[0].Description;

        throw new ArgumentOutOfRangeException(
            nameof(value), value, null);
    }

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

    /// <summary>
    /// Tries to find an Enum value based on the provided code using a custom match function.
    /// </summary>
    /// <param name="code">The code to search for within the Enum values descriptions.</param>
    /// <param name="match">A custom match function that defines the comparison logic between the code and Enum descriptions.</param>
    /// <param name="value">When this method returns, contains the Enum value that corresponds to the specified code if found; otherwise, the default value.</param>
    /// <returns>True if an Enum value with the specified code is found; otherwise, false.</returns>
    private static bool TryFindValue<T>(
        string code,
        Func<string, string, bool> match,
        out T? value) where T : Enum
    {
        value = default;

        var values = Enum.GetValues(typeof(T)).Cast<T>();

        foreach (var enumValue in values)
        {
            var rawDescription = enumValue.GetRawDescription();
            var vCodes = rawDescription
                .Split(',', StringSplitOptions.TrimEntries)
                .ToList();
            if (!vCodes.Exists(vCode => match(vCode, code))) continue;
            value = enumValue;
            return true;
        }

        return false;
    }

    private static bool EqualityMatcher(string x, string y) =>
        x.Equals(y, StringComparison.InvariantCultureIgnoreCase);

}