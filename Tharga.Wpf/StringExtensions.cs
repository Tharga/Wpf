namespace Tharga.Wpf;

public static class StringExtensions
{
    public static string NullIfEmpty(this string item)
    {
        if (string.IsNullOrEmpty(item)) return null;
        return item;
    }
}