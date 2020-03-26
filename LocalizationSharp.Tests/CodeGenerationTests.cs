using System;
using System.Globalization;
using System.IO;
using LocalizationSharp.CodeGenerator;
using LocalizationSharp.Extension.SystemObject;
using NUnit.Framework;

namespace LocalizationSharp.Tests
{
    [TestFixture]
    public class CodeGenerationTests
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

            LocalizationManager.CreateSingletonInstance("LocalizeFiles", new CultureInfo("ja-JP"));
            Assert.True(this.LocalizeText("testLocalize.text") == "Hello LocalizationSharp!");

            Console.WriteLine(LocalizationCodeGenerator.Generate(new LocalizationCodeGenerator.CodeGenerateOptions
            {
                GenerateMemberMode = LocalizationCodeGenerator.GenerateMemberMode.ExtensionMethod,
                TypeAccessMode = LocalizationCodeGenerator.TypeAccessMode.QuickAccess,
                UseFirstAtChar = false
            }));
        }
    }
}