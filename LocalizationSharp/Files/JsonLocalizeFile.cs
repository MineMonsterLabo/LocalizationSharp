using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using LocalizationSharp.Core;
using Newtonsoft.Json;

namespace LocalizationSharp.Files
{
    public class JsonLocalizeFile : ILocalizeFile
    {
        private Dictionary<string, ILocalizeContent<object>> _contents;
        private CultureInfo _cultureInfo;

        public CultureInfo CultureInfo => _cultureInfo;

        public JsonLocalizeFile(CultureInfo cultureInfo)
        {
            _cultureInfo = cultureInfo;
            _contents = new Dictionary<string, ILocalizeContent<object>>();
        }

        public JsonLocalizeFile(JsonLocalizeFileData data)
        {
            _cultureInfo = CultureInfo.GetCultureInfo(data.CultureString);
            _contents = data.Contents;
        }

        public JsonLocalizeFile(CultureInfo cultureInfo, Dictionary<string, ILocalizeContent<object>> contents)
        {
            _cultureInfo = cultureInfo;
            _contents = contents;
        }

        public IEnumerator<KeyValuePair<string, ILocalizeContent<object>>> GetEnumerator()
        {
            return _contents.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void Add(KeyValuePair<string, ILocalizeContent<object>> item)
        {
            _contents.Add(item.Key, item.Value);
        }

        public void Clear()
        {
            _contents.Clear();
        }

        public bool Contains(KeyValuePair<string, ILocalizeContent<object>> item)
        {
            return _contents.ContainsKey(item.Key);
        }

        public void CopyTo(KeyValuePair<string, ILocalizeContent<object>>[] array, int arrayIndex)
        {
            throw new NotImplementedException();
        }

        public bool Remove(KeyValuePair<string, ILocalizeContent<object>> item)
        {
            return _contents.Remove(item.Key);
        }

        public int Count => _contents.Count;
        public bool IsReadOnly => false;

        public void Add(string key, ILocalizeContent<object> value)
        {
            _contents.Add(key, value);
        }

        public bool ContainsKey(string key)
        {
            return _contents.ContainsKey(key);
        }

        public bool Remove(string key)
        {
            return _contents.Remove(key);
        }

        public bool TryGetValue(string key, out ILocalizeContent<object> value)
        {
            return _contents.TryGetValue(key, out value);
        }

        public ILocalizeContent<object> this[string key]
        {
            get => _contents[key];
            set => _contents[key] = value;
        }

        public ICollection<string> Keys => _contents.Keys;
        public ICollection<ILocalizeContent<object>> Values => _contents.Values;

        public void Save(string folderPath)
        {
            JsonLocalizeFileData data = new JsonLocalizeFileData(_cultureInfo, _contents);
            string json = JsonConvert.SerializeObject(data, Formatting.Indented, new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Serialize,
                TypeNameHandling = TypeNameHandling.All
            });

            File.WriteAllText($"{folderPath}/{CultureInfo.Name}.json_lang", json, Encoding.UTF8);
        }

        public class JsonLocalizeFileData
        {
            public string CultureString { get; set; }
            public Dictionary<string, ILocalizeContent<object>> Contents { get; set; }

            public JsonLocalizeFileData()
            {
            }

            public JsonLocalizeFileData(CultureInfo cultureInfo, Dictionary<string, ILocalizeContent<object>> contents)
            {
                CultureString = cultureInfo.Name;
                Contents = contents;
            }
        }
    }
}