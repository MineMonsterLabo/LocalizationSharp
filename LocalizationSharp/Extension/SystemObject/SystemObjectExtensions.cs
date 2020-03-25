using LocalizationSharp.Contents;
using LocalizationSharp.Core;

namespace LocalizationSharp.Extension.SystemObject
{
    public static class SystemObjectExtensions
    {
        public static ILocalizeContent<object> Localize(this object value, string key)
        {
            return LocalizationManager.Instance.GetFile()[key];
        }

        public static T Localize<T>(this object value, string key) where T : ILocalizeContent<object>
        {
            return (T) Localize(value, key);
        }

        public static string LocalizeText(this object value, string key)
        {
            return Localize<LocalizeTextContent>(value, key).Content;
        }

        public static T LocalizeContent<T>(this object value, string key)
        {
            return (T) Localize(value, key).Content;
        }
    }
}