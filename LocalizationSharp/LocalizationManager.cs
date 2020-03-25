using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using LocalizationSharp.Core;
using LocalizationSharp.Files;
using Newtonsoft.Json;

namespace LocalizationSharp
{
    public class LocalizationManager
    {
        private static Dictionary<string, Func<string, ILocalizeFile>> _fileLoadFunctions =
            new Dictionary<string, Func<string, ILocalizeFile>>();

        public static LocalizationManager Instance { get; set; }

        static LocalizationManager()
        {
            RegisterFileLoadFunction(".json_lang", data =>
            {
                var jsonData = JsonConvert.DeserializeObject<JsonLocalizeFile.JsonLocalizeFileData>(data,
                    new JsonSerializerSettings
                    {
                        ReferenceLoopHandling = ReferenceLoopHandling.Serialize,
                        TypeNameHandling = TypeNameHandling.All
                    });
                return new JsonLocalizeFile(jsonData);
            });
        }

        public static LocalizationManager CreateSingletonInstance(string localizeFileDirectory, CultureInfo cultureInfo)
        {
            if (Instance != null)
                throw new InvalidOperationException("インスタンスは既に作成されています。");

            Instance = new LocalizationManager(localizeFileDirectory, cultureInfo);

            return Instance;
        }

        public static void RegisterFileLoadFunction(string extension, Func<string, ILocalizeFile> func)
        {
            _fileLoadFunctions[extension] = func;
        }

        private Dictionary<CultureInfo, ILocalizeFile> _files = new Dictionary<CultureInfo, ILocalizeFile>();
        public CultureInfo CultureInfo { get; set; }

        public LocalizationManager()
        {
            CultureInfo = CultureInfo.CurrentCulture;
        }

        public LocalizationManager(string localizeFileDirectory, CultureInfo cultureInfo)
        {
            if (cultureInfo == null)
                cultureInfo = CultureInfo.CurrentCulture;

            CultureInfo = cultureInfo;

            DirectoryInfo directoryInfo = new DirectoryInfo(localizeFileDirectory);
            foreach (FileInfo file in directoryInfo.GetFiles("*.*_lang"))
            {
                LoadFile(file.FullName, file.Extension);
            }
        }

        public ILocalizeFile LoadFile(string filePath, string extension)
        {
            ILocalizeFile file = null;
            string data = File.ReadAllText(filePath, Encoding.UTF8);
            file = _fileLoadFunctions[extension](data);

            if (file == null)
                throw new IOException("ファイルが読み込めませんでした");

            _files.Add(file.CultureInfo, file);

            return file;
        }

        public ILocalizeFile LoadFileFromData(string data, string extension)
        {
            ILocalizeFile file = null;
            file = _fileLoadFunctions[extension](data);

            if (file == null)
                throw new IOException("ファイルが読み込めませんでした");

            _files.Add(file.CultureInfo, file);

            return file;
        }

        public ILocalizeFile GetFile()
        {
            return _files[CultureInfo];
        }

        public ILocalizeFile GetFile(string cultureName)
        {
            return _files[CultureInfo.GetCultureInfo(cultureName)];
        }
    }
}