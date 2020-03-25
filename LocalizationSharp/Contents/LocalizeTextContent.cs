using LocalizationSharp.Core;

namespace LocalizationSharp.Contents
{
    public class LocalizeTextContent : ILocalizeContent<string>
    {
        public string Content { get; set; }

        public LocalizeTextContent()
        {
        }

        public LocalizeTextContent(string text)
        {
            Content = text;
        }

        public string Format(params object[] args)
        {
            return string.Format(Content, args);
        }
    }
}