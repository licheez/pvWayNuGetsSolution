// See https://aka.ms/new-console-template for more information

using Microsoft.Extensions.Logging;
using PvWay.StackTraceConsole.nc8;

Console.WriteLine("Hello, StackTrace console");

var stLogger = new StackTraceLogger();
stLogger.Log(LogLevel.Trace, "someTrace");

var ex = new ApplicationException("some test exception");
stLogger.LogCritical(ex, "error");

var ct = new CrashTester(stLogger);
ct.Crash();