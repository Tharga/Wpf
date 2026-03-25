using Tharga.Wpf.ApplicationUpdate;

namespace Tharga.Wpf.Tests.Features.ApplicationUpdate;

public class SplashImageLibraryTests
{
    [Theory]
    [InlineData(nameof(SplashImageLibrary.Blue), "blue.png")]
    [InlineData(nameof(SplashImageLibrary.Green), "green.png")]
    [InlineData(nameof(SplashImageLibrary.GreenTransparent), "green-t.png")]
    [InlineData(nameof(SplashImageLibrary.Orange), "orange.png")]
    [InlineData(nameof(SplashImageLibrary.Red), "red.png")]
    [InlineData(nameof(SplashImageLibrary.RedTransparent), "red-t.png")]
    [InlineData(nameof(SplashImageLibrary.Teal), "teal.png")]
    [InlineData(nameof(SplashImageLibrary.TealTransparent), "teal-t.png")]
    [InlineData(nameof(SplashImageLibrary.White), "white.png")]
    [InlineData(nameof(SplashImageLibrary.Yellow), "yellow.png")]
    public void Image_Paths_Are_Valid_Pack_Uris(string fieldName, string expectedFileName)
    {
        var field = typeof(SplashImageLibrary).GetField(fieldName);
        var value = (string)field!.GetValue(null)!;

        Assert.StartsWith("pack://application:,,,/Tharga.Wpf;component/Images/Application/", value);
        Assert.EndsWith(expectedFileName, value);
    }

    [Fact]
    public void All_Image_Paths_Are_Not_Null()
    {
        Assert.NotNull(SplashImageLibrary.Blue);
        Assert.NotNull(SplashImageLibrary.Green);
        Assert.NotNull(SplashImageLibrary.GreenTransparent);
        Assert.NotNull(SplashImageLibrary.Orange);
        Assert.NotNull(SplashImageLibrary.Red);
        Assert.NotNull(SplashImageLibrary.RedTransparent);
        Assert.NotNull(SplashImageLibrary.Teal);
        Assert.NotNull(SplashImageLibrary.TealTransparent);
        Assert.NotNull(SplashImageLibrary.White);
        Assert.NotNull(SplashImageLibrary.Yellow);
    }
}
