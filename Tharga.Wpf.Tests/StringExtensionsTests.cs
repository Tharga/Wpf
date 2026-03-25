using Tharga.Wpf.Framework;

namespace Tharga.Wpf.Tests;

public class StringExtensionsTests
{
    [Fact]
    public void NullIfEmpty_Returns_Null_For_Null()
    {
        string input = null;

        Assert.Null(input.NullIfEmpty());
    }

    [Fact]
    public void NullIfEmpty_Returns_Null_For_Empty()
    {
        Assert.Null("".NullIfEmpty());
    }

    [Fact]
    public void NullIfEmpty_Returns_Value_For_NonEmpty()
    {
        Assert.Equal("hello", "hello".NullIfEmpty());
    }

    [Fact]
    public void NullIfEmpty_Returns_Whitespace_As_Is()
    {
        // string.IsNullOrEmpty doesn't catch whitespace-only
        Assert.Equal(" ", " ".NullIfEmpty());
    }
}
