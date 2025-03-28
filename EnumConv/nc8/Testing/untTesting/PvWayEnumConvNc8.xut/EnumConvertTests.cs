namespace PvWayEnumConvNc8.xut;

public class EnumConvertTests
{
    [Fact]
    public void Sunny()
    {
        const SomeEnu enu = SomeEnu.ValueOneOrTwo;
        var code = enu.GetCode();
        Assert.Equal("Val1", code);
        var v1 = EnumConvert.GetValue<SomeEnu>(code);
        Assert.Equal(enu, v1);
        var v2 = EnumConvert.GetValue<SomeEnu>("VAL2");
        Assert.Equal(enu, v2);
    }

    [Fact]
    public void ShouldMatch()
    {
        var v = EnumConvert.GetValue<SomeEnu>(
            "3", (valueCode, code) =>
                valueCode.Contains(code, StringComparison.InvariantCultureIgnoreCase));
        Assert.Equal(SomeEnu.ValueThree, v);

        v = EnumConvert.GetValue(
            "noMatch", SomeEnu.ValueOneOrTwo,
            (valueCode, code) =>
                valueCode.Contains(code, StringComparison.InvariantCultureIgnoreCase));
        Assert.Equal(SomeEnu.ValueOneOrTwo, v);
    }

    [Fact]
    public void ShouldUseDefaultValue()
    {
        var val = EnumConvert.GetValue(null, SomeEnu.ValueOneOrTwo);
        Assert.Equal(SomeEnu.ValueOneOrTwo, val);
        val = EnumConvert.GetValue("Val3", SomeEnu.ValueOneOrTwo);
        Assert.Equal(SomeEnu.ValueThree, val);
    }

    [Fact]
    public void RainyGetCode()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() =>
        {
            const SomeEnu wrongVal = (SomeEnu)9;
            wrongVal.GetCode();
        });
    }

    [Fact]
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