using System.Text;

namespace Tharga.Wpf.Framework;

internal static class StringExtensions
{
    public static string NullIfEmpty(this string item)
    {
        if (string.IsNullOrEmpty(item)) return null;
        return item;
    }
}