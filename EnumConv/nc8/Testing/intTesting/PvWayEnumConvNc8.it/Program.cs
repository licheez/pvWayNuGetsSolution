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
