namespace PvWayEnumConvNc8.nut;

public class EnumConvertTests
{
    [Test]
    public void Sunny()
    {
        const SomeEnu enu = SomeEnu.ValueOneOrTwo;
        var code = enu.GetCode();
        Assert.That(code, Is.EqualTo("Val1"));
        
        var v1 = EnumConvert.GetValue<SomeEnu>(code);
        Assert.That(v1, Is.EqualTo(enu));
        var v2 = EnumConvert.GetValue<SomeEnu>("VAL2");
        Assert.That(v2, Is.EqualTo(enu));
    }

    [Test]
    public void ShouldMatch()
    {
        var v = EnumConvert.GetValue<SomeEnu>(
            "3", (valueCode, code) =>
                valueCode.Contains(code, StringComparison.InvariantCultureIgnoreCase));
        Assert.That(v, Is.EqualTo(SomeEnu.ValueThree));

        v = EnumConvert.GetValue(
            "noMatch", SomeEnu.ValueOneOrTwo,
            (valueCode, code) =>
                valueCode.Contains(code, StringComparison.InvariantCultureIgnoreCase));
        Assert.That(v, Is.EqualTo(SomeEnu.ValueOneOrTwo));
    }

    [Test]
    public void ShouldUseDefaultValue()
    {
        var val = EnumConvert.GetValue(null, SomeEnu.ValueOneOrTwo);
        Assert.That(val, Is.EqualTo(SomeEnu.ValueOneOrTwo));
        val = EnumConvert.GetValue("Val3", SomeEnu.ValueOneOrTwo);
        Assert.That(val, Is.EqualTo(SomeEnu.ValueThree));
    }

    [Test]
    public void RainyGetCode()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() =>
        {
            const SomeEnu wrongVal = (SomeEnu)9;
            wrongVal.GetCode();
        });
    }

    [Test]
    public void RainyGetValue()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() =>
        {
            const string wrongCode = "wrongCode";
            _ = EnumConvert.GetValue<SomeEnu>(wrongCode);
        });
        
        Assert.Throws<ArgumentOutOfRangeException>(() =>
        {
            _ = EnumConvert.GetValue<SomeEnu>(string.Empty);
        });
    }

}

internal enum SomeEnu
{
    [System.ComponentModel.Description("Val1, Val2")]
    ValueOneOrTwo,
    [System.ComponentModel.Description("Val3")]
    ValueThree
}