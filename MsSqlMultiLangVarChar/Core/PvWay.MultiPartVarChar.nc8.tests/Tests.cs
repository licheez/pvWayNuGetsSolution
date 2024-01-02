namespace PvWay.MultiPartVarChar.nc8.tests;

public class Tests
{
    [Test]
    public void SerializeShouldSucceed()
    {
        var dic = new Dictionary<string, string>
        {
            { "en", "bear" },
            { "fr", "ours" }
        };
        var mpVarChar = new PvWayMpVarChar(dic);
        var mpString = mpVarChar.ToString();
        Assert.That(mpString, Is.EqualTo("<en>bear</en><fr>ours</fr>"));
    }

    [Test]
    public void EscapingShouldSucceed()
    {
        var dic = new Dictionary<string, string>
        {
            { "en", "five > four < six" }
        };
        var mpVarChar = new PvWayMpVarChar(dic);
        var mpString = mpVarChar.ToString();
        Assert.That(mpString, Is.EqualTo("<en>five &gt; four &lt; six</en>"));
    }

    [Test]
    public void KeepGtLtCodeShouldSucceed()
    {
        var dic = new Dictionary<string, string>
        {
            { "en", "five &gt; four &lt; six" }
        };
        var mpVarChar = new PvWayMpVarChar(dic);
        var mpString = mpVarChar.ToString();
        Assert.That(mpString, Is.EqualTo("<en>five ;tg& four ;tl& six</en>"));
    }

    [Test]
    public void DeserializeShouldSucceed()
    {
        const string mpString = "<en>bear</en><fr>ours</fr>";
        var res = PvWayMpVarChar
            .TryDeserialize(mpString, out var mpVarChar, out var errorMessage);
        Assert.Multiple(() =>
        {
            Assert.That(res, Is.EqualTo(PvWayMpCharDeserializationResult.Ok));
            Assert.That(mpVarChar, Is.Not.Null);
            Assert.That(mpVarChar!.ToString(), Is.EqualTo(mpString));
            Assert.That(errorMessage, Is.Null);
            Assert.That(mpVarChar.MpDic, Has.Count.EqualTo(2));
            Assert.That(mpVarChar.MpDic["en"], Is.EqualTo("bear"));
            Assert.That(mpVarChar.MpDic["fr"], Is.EqualTo("ours"));
        });
    }

    [Test]
    public void DeserializeEscapingShouldSucceed()
    {
        const string mpString = "<en>five &gt; four &lt; six</en>";
        var res = PvWayMpVarChar.TryDeserialize(mpString, out var mpVarChar, out var errorMessage);
        Assert.Multiple(() =>
        {
            Assert.That(res, Is.EqualTo(PvWayMpCharDeserializationResult.Ok));
            Assert.That(mpVarChar, Is.Not.Null);
            Assert.That(mpVarChar!.ToString(), Is.EqualTo(mpString));
            Assert.That(errorMessage, Is.Null);
            Assert.That(mpVarChar.MpDic, Has.Count.EqualTo(1));
            Assert.That(mpVarChar.MpDic["en"], Is.EqualTo("five > four < six"));
        });
    }

    [Test]
    public void DeserializeNullOrEmptyShouldSucceed()
    {
        var res = PvWayMpVarChar.TryDeserialize(
            null!, out var mpVarChar, out var errorMessage);
        Assert.Multiple(() =>
            {
                Assert.That(res, Is.EqualTo(PvWayMpCharDeserializationResult.Empty));
                Assert.That(mpVarChar, Is.Not.Null);
                Assert.That(mpVarChar!.MpDic, Is.Empty);
                Assert.That(errorMessage, Is.Null);
            });
    }

    [Test]
    public void DeserializeShouldFail()
    {
        const string mpString =
            "invalid string";
        var res = PvWayMpVarChar.TryDeserialize(
            mpString, out var mpVarChar, out var errorMessage);
        Assert.Multiple(() =>
        {
            Assert.That(res, Is.EqualTo(PvWayMpCharDeserializationResult.Failed));
            Assert.That(mpVarChar, Is.Null);
            Assert.That(errorMessage, Is.Not.Empty);
        });
    }

    [Test]
    public void GetPartForKeyShouldSucceed()
    {
        var dic = new Dictionary<string, string>
        {
            { "en", "bear" },
            { "fr", "ours" }
        };
        var mpVarChar = new PvWayMpVarChar(dic);

        var part = mpVarChar.GetPartForKey("en");
        Assert.That(part, Is.EqualTo("bear"));
        part = mpVarChar.GetPartForKey("fr");
        Assert.That(part, Is.EqualTo("ours"));
        part = mpVarChar.GetPartForKey("de");
        Assert.That(part, Is.Null);
    }

    [Test]
    public void TryGetPartForKeyShouldSucceed()
    {
        var dic = new Dictionary<string, string>
        {
            { "en", "bear" },
            { "fr", "ours" }
        };
        var mpVarChar = new PvWayMpVarChar(dic);

        var res = mpVarChar.FindPartForKey(
            "en", out var part);
        Assert.Multiple(() =>
        {
            Assert.That(res, Is.EqualTo(PvWayMpCharFindPartResult.ExactMatch));
            Assert.That(part, Is.EqualTo("bear"));
        });

        res = mpVarChar.FindPartForKey("fr", out part);
        Assert.Multiple(() =>
        {
            Assert.That(res, Is.EqualTo(PvWayMpCharFindPartResult.ExactMatch));
            Assert.That(part, Is.EqualTo("ours"));
        });

        res = mpVarChar.FindPartForKey("de", out part);
        Assert.Multiple(() =>
        {
            Assert.That(res, Is.EqualTo(PvWayMpCharFindPartResult.FirstValue));
            Assert.That(part, Is.EqualTo("bear"));
        });

        var emptyDic = new Dictionary<string, string>();
        mpVarChar = new PvWayMpVarChar(emptyDic);
        res = mpVarChar.FindPartForKey("de", out part);
        Assert.Multiple(() =>
        {
            Assert.That(res, Is.EqualTo(PvWayMpCharFindPartResult.NotFound));
            Assert.That(part, Is.Null);
        });
    }
}