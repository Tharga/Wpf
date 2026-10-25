using Tharga.Wpf.Properties;

namespace Tharga.Wpf.Tests.Properties;

public class ResourcesTests
{
    [Fact]
    public void Close_Bitmap_Loads_From_The_Embedded_Resources()
    {
        var bitmap = Resources.Close;

        Assert.NotNull(bitmap);
        Assert.True(bitmap.Width > 0);
        Assert.True(bitmap.Height > 0);
    }

    [Fact]
    public void Thargelion_Icon_Loads_From_The_Embedded_Resources()
    {
        var icon = Resources.thargelion;

        Assert.NotNull(icon);
        Assert.True(icon.Width > 0);
        Assert.True(icon.Height > 0);
    }
}
