namespace LocalizationSharp.Core
{
    public interface ILocalizeContent<out T>
    {
        T Content { get; }
    }
}