using System;
using System.Globalization;
using System.IO;
using LocalizationSharp.Extension.SystemObject;
using NUnit.Framework;

namespace LocalizationSharp.Tests
{
    [TestFixture]
    public class UseLocalizeFileTests
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

            try
            {
                LocalizationManager.CreateSingletonInstance("LocalizeFiles", new CultureInfo("ja-JP"));
            }
            catch
            {
            }

            Assert.True(this.LocalizeText("testLocalize.text") == "Hello LocalizationSharp!");
        }
    }
}