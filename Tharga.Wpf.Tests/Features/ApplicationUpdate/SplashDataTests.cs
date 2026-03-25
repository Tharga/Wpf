using Tharga.Wpf.ApplicationUpdate;

namespace Tharga.Wpf.Tests.Features.ApplicationUpdate;

public class SplashDataTests
{
    [Fact]
    public void Construction_With_Required_Properties()
    {
        var data = new SplashData
        {
            ClientLocation = new Uri("https://example.com/client"),
            ClientSourceLocation = new Uri("https://example.com/source")
        };

        Assert.Equal(new Uri("https://example.com/client"), data.ClientLocation);
        Assert.Equal(new Uri("https://example.com/source"), data.ClientSourceLocation);
    }

    [Fact]
    public void Optional_Properties_Default_To_Null_Or_False()
    {
        var data = new SplashData
        {
            ClientLocation = new Uri("https://example.com"),
            ClientSourceLocation = new Uri("https://example.com")
        };

        Assert.Null(data.FullName);
        Assert.False(data.FirstRun);
        Assert.Null(data.EnvironmentName);
        Assert.Null(data.Version);
        Assert.Null(data.ExeLocation);
        Assert.Null(data.EntryMessage);
        Assert.Null(data.SplashClosed);
        Assert.Null(data.ImagePath);
    }

    [Fact]
    public void Can_Set_All_Properties()
    {
        CloseMethod? closedWith = null;

        var data = new SplashData
        {
            ClientLocation = new Uri("https://example.com/client"),
            ClientSourceLocation = new Uri("https://example.com/source"),
            FullName = "Test App",
            FirstRun = true,
            EnvironmentName = "Production",
            Version = "1.2.3",
            ExeLocation = "C:\\app.exe",
            EntryMessage = "Welcome",
            SplashClosed = m => closedWith = m,
            ImagePath = "/images/splash.png"
        };

        Assert.Equal("Test App", data.FullName);
        Assert.True(data.FirstRun);
        Assert.Equal("Production", data.EnvironmentName);
        Assert.Equal("1.2.3", data.Version);

        data.SplashClosed(CloseMethod.Manually);
        Assert.Equal(CloseMethod.Manually, closedWith);
    }
}
