using System.Collections.Generic;
using System.Globalization;

namespace LocalizationSharp.Core
{
    public interface ILocalizeFile : IDictionary<string, ILocalizeContent<object>>
    {
        CultureInfo CultureInfo { get; }

        void Save(string filePath);
    }
}