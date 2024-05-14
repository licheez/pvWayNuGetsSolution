using System.Reflection;
using System.Text.RegularExpressions;

namespace PvWay.ExcelTranslationProvider.Abstractions.nc8;


public interface IPvWayAssemblyBasedExcelTranslationServiceConfig
{
    /// <summary>
    /// This will be used for selecting Excel resources from
    /// the collection of resource names
    /// Example: "tr.+\\.xlsx$" for getting all resources
    /// where the name starts with "tr." and ends with ".xlsx"
    /// </summary>
    Regex ResourceNameMatcher { get; }
    /// <summary>
    /// The assembly containing the Excel resources
    /// </summary>
    Assembly Assembly { get; }
    
    Action<Exception> LogException { get; } 
}