using PvWay.LoggerService.Abstractions.nc6;
using PvWay.LoggerService.nc6;

namespace PvWay.LoggerServiceLab.Tests.nc6;

[TestFixture]
public class Tests
{

    [Test]
    public async Task Test1()
    {
        var ls = PvWayLoggerService.CreateUTestingLoggerService();

        // inject the unit testing logger to the class
        // so that it will be possible to retrieve the logs

        var svc = new SomeClassToTest(ls);

        await svc.PerformSomeActionAsync();

        Assert.Multiple(() =>
        {
            Assert.That(ls.LogRows, Is.Not.Empty);
            Assert.That(ls.LogRows.Count, Is.EqualTo(2));
            Assert.That(ls.HasLog("before writing"), Is.True);
        });

        var fRow = ls.FindFirstMatchingRow("writing");
        Assert.Multiple(() =>
        {
            Assert.That(fRow, Is.Not.Null);
            StringAssert.Contains("before", fRow?.Message);
        });

        var lRow = ls.FindLastMatchingRow("writing");
        Assert.Multiple(() =>
        {
            Assert.That(lRow, Is.Not.Null);
            StringAssert.Contains("after", lRow?.Message);
        });

    }
}

internal class SomeClassToTest
{
    private readonly ILoggerService _ls;

    public SomeClassToTest(
        ILoggerService ls)
    {
        _ls = ls;
    }

    public async Task PerformSomeActionAsync()
    {
        await _ls.LogAsync("before writing to the console");
        Console.WriteLine("Hello there");
        await _ls.LogAsync("after writing to the console");
    }
}