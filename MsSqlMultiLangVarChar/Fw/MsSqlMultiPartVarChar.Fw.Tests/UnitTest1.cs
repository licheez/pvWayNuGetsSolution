using System.Collections.Generic;
using NUnit.Framework;
using pvWay.MsSqlMultiPartVarChar.Fw;

namespace MsSqlMultiPartVarChar.Fw.Tests
{
    public class Tests
    {

        [Test]
        public void SerializeShouldSucceed()
        {
            var dic = new Dictionary<string, string>
            {
                {"en", "bear"},
                { "fr", "ours"}
            };
            var mpVarChar = new MpVarChar(dic);
            var mpString = mpVarChar.ToString();
            Assert.IsTrue(mpString == "<en>bear</en><fr>ours</fr>");
        }

        [Test]
        public void EscapingShouldSucceed()
        {
            var dic = new Dictionary<string, string>
            {
                {"en", "five > four < six"}
            };
            var mpVarChar = new MpVarChar(dic);
            var mpString = mpVarChar.ToString();
            Assert.IsTrue(mpString == "<en>five &gt; four &lt; six</en>");
        }

        [Test]
        public void KeepGtLtCodeShouldSucceed()
        {
            var dic = new Dictionary<string, string>
            {
                {"en", "five &gt; four &lt; six"}
            };
            var mpVarChar = new MpVarChar(dic);
            var mpString = mpVarChar.ToString();
            Assert.IsTrue(mpString == "<en>five ;tg& four ;tl& six</en>");
        }


        [Test]
        public void DeserializeShouldSucceed()
        {
            const string mpString = "<en>bear</en><fr>ours</fr>";
            var ok = MpVarChar.TryDeserialize(mpString, out var mpVarChar, out var res);
            Assert.IsTrue(ok);
            Assert.AreEqual(mpString, mpVarChar.ToString());
            Assert.AreEqual(res, "Ok");
            Assert.IsTrue(mpVarChar.MpDic.Count == 2);
            Assert.IsTrue(mpVarChar.MpDic["en"] == "bear");
            Assert.IsTrue(mpVarChar.MpDic["fr"] == "ours");
        }

        [Test]
        public void DeserializeEscapingShouldSucceed()
        {
            const string mpString = "<en>five &gt; four &lt; six</en>";
            var ok = MpVarChar.TryDeserialize(mpString, out var mpVarChar, out var res);
            Assert.IsTrue(ok);
            Assert.AreEqual(mpString, mpVarChar.ToString());
            Assert.AreEqual(res, "Ok");
            Assert.IsTrue(mpVarChar.MpDic.Count == 1);
            Assert.IsTrue(mpVarChar.MpDic["en"] == "five > four < six");
        }

        [Test]
        public void DeserializeGtLtShouldSucceed()
        {
            const string mpString = "<en>five ;tg& four ;tl& six</en>";
            var ok = MpVarChar.TryDeserialize(mpString, out var mpVarChar, out var res);
            Assert.IsTrue(ok);
            Assert.AreEqual(mpString, mpVarChar.ToString());
            Assert.AreEqual(res, "Ok");
            Assert.IsTrue(mpVarChar.MpDic.Count == 1);
            Assert.IsTrue(mpVarChar.MpDic["en"] == "five &gt; four &lt; six");
        }


        [Test]
        public void DeserializeNullOrEmptyShouldSucceed()
        {
            var ok = MpVarChar.TryDeserialize(null, out var mpVarChar, out var res);
            Assert.IsTrue(ok);
            Assert.IsTrue(mpVarChar.ToString() == "");
            Assert.AreEqual(res, "Empty");
            Assert.IsTrue(mpVarChar.MpDic.Count == 0);
        }

        [Test]
        public void DeserializeShouldFail()
        {
            const string mpString =
                "invalid string";
            var ok = MpVarChar.TryDeserialize(mpString, out var mpVarChar, out var res);
            Assert.IsFalse(ok);
            Assert.IsNull(mpVarChar);
            Assert.IsNotEmpty(res);
        }

        [Test]
        public void GetPartForKeyShouldSucceed()
        {
            var dic = new Dictionary<string, string>
            {
                {"en", "bear"},
                { "fr", "ours"}
            };
            var mpVarChar = new MpVarChar(dic);

            var part = mpVarChar.GetPartForKey("en");
            Assert.IsTrue(part == "bear");
            part = mpVarChar.GetPartForKey("fr");
            Assert.IsTrue(part == "ours");
            part = mpVarChar.GetPartForKey("de");
            Assert.IsNull(part);
        }

        [Test]
        public void TryGetPartForKeyShouldSucceed()
        {
            var dic = new Dictionary<string, string>
            {
                {"en", "bear"},
                { "fr", "ours"}
            };
            var mpVarChar = new MpVarChar(dic);

            var found = mpVarChar.TryGetPartForKey("en", out var part);
            Assert.IsTrue(found);
            Assert.IsTrue(part == "bear");

            found = mpVarChar.TryGetPartForKey("fr", out part);
            Assert.IsTrue(found);
            Assert.IsTrue(part == "ours");

            found = mpVarChar.TryGetPartForKey("de", out part);
            Assert.IsFalse(found);
            Assert.IsTrue(part == "bear");
        }

        [Test]
        public void GetCreateScriptShouldSucceed()
        {
            var createScript = MpVarChar.CreateFunctionScript("dbo", "FnGetMpPart");
            Assert.IsNotEmpty(createScript);
        }

    }


}
