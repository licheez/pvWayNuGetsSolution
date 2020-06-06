using NUnit.Framework;
using pvWay.MsSqlMultiPartVarChar.Core;

namespace MsSqlMultiPartVarChar.Core.Tests
{
    public class Tests
    {

        [Test]
        public void DeserializeShouldSucceed()
        {
            const string mpString =
                "en::english text::fr::texte en français avec le caractère \\: au milieu::nl::nederlandse tekst::";
            var ok = MpVarChar.TryDeserialize(mpString, out var mpVarChar, out var res);
            Assert.IsTrue(ok);
            Assert.AreEqual(mpString, mpVarChar.ToString());
            Assert.AreEqual(res, "Ok");
            Assert.IsTrue(mpVarChar.MpDic.Count == 3);
            Assert.IsTrue(mpVarChar.MpDic["en"] == "english text");
            Assert.IsTrue(mpVarChar.MpDic["fr"] == "texte en français avec le caractère : au milieu");
            Assert.IsTrue(mpVarChar.MpDic["nl"] == "nederlandse tekst");
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
            const string mpString =
                "en::english text::fr::texte en français avec le caractère \\: au milieu::nl::nederlandse tekst::";
            var ok = MpVarChar.TryDeserialize(mpString, out var mpVarChar, out var _);
            Assert.IsTrue(ok);

            var part = mpVarChar.GetPartForKey("en");
            Assert.IsTrue(part == "english text");
            part = mpVarChar.GetPartForKey("fr");
            Assert.IsTrue(part == "texte en français avec le caractère : au milieu");
            part = mpVarChar.GetPartForKey("de");
            Assert.IsNull(part);
        }

        [Test]
        public void TryGetPartForKeyShouldSucceed()
        {
            const string mpString =
                "en::english text::fr::texte en français avec le caractère \\: au milieu::nl::nederlandse tekst::";
            var ok = MpVarChar.TryDeserialize(mpString, out var mpVarChar, out var _);
            Assert.IsTrue(ok);

            var found = mpVarChar.TryGetPartForKey("en", out var part);
            Assert.IsTrue(found);
            Assert.IsTrue(part == "english text");

            found = mpVarChar.TryGetPartForKey("fr", out part);
            Assert.IsTrue(found);
            Assert.IsTrue(part == "texte en français avec le caractère : au milieu");

            found = mpVarChar.TryGetPartForKey("de", out part);
            Assert.IsFalse(found);
            Assert.IsTrue(part == "english text");
        }

        [Test]
        public void GetCreateScriptShouldSucceed()
        {
            var createScript = MpVarChar.CreateFunctionScript;
            Assert.IsNotEmpty(createScript);
        }

    }
}