using System;
using System.Globalization;
using System.IO;
using LocalizationSharp.Contents;
using LocalizationSharp.Files;
using NUnit.Framework;

namespace LocalizationSharp.Tests
{
    [TestFixture]
    public class CreateLocalizeFileTests
    {
        [SetUp]
        public void Setup()
        {
            Environment.CurrentDirectory = TestContext.CurrentContext.TestDirectory;
        }

        [Test]
        public void CreateFile()
        {
            Directory.CreateDirectory("LocalizeFiles");

            JsonLocalizeFile file = new JsonLocalizeFile(new CultureInfo("ja-JP"))
            {
                {"testLocalize.text", new LocalizeTextContent("Hello LocalizationSharp!")},
                {"testLocalize.text2", new LocalizeTextContent("Hello LocalizationSharp!")},
                {"testLocalize.text3", new LocalizeTextContent("Hello LocalizationSharp!")},
                {"testLocalize.text4", new LocalizeTextContent("Hello LocalizationSharp!")},
                {"testLocalize.text5", new LocalizeTextContent("Hello LocalizationSharp!")},
                {"testLocalize.text6", new LocalizeTextContent("Hello LocalizationSharp!")},
                {"testLocalize.text7", new LocalizeTextContent("Hello LocalizationSharp!")},
                {"testLocalize.text8", new LocalizeTextContent("Hello LocalizationSharp!")},
                {"testLocalize.text9", new LocalizeTextContent("Hello LocalizationSharp!")},
                {"testLocalize.text10", new LocalizeTextContent("Hello LocalizationSharp!")},
            };
            file.Save($"LocalizeFiles/{file.CultureInfo.Name}.json_lang");
        }
    }
}