﻿using System;
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
                {"testLocalize.text", new LocalizeTextContent("Hello LocalizationSharp!")}
            };
            file.Save($"LocalizeFiles/{file.CultureInfo.Name}.json_lang");
        }
    }
}