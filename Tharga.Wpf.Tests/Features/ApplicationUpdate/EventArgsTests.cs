using Tharga.Wpf.ApplicationUpdate;

namespace Tharga.Wpf.Tests.Features.ApplicationUpdate;

public class UpdateInfoEventArgsTests
{
    [Fact]
    public void Constructor_Sets_Message()
    {
        var args = new UpdateInfoEventArgs("Update available");

        Assert.Equal("Update available", args.Message);
    }
}

public class LicenseInfoEventArgsTests
{
    [Fact]
    public void Constructor_Sets_Properties()
    {
        var args = new LicenseInfoEventArgs(true, "License valid");

        Assert.True(args.Valid);
        Assert.Equal("License valid", args.Message);
    }

    [Fact]
    public void Invalid_License()
    {
        var args = new LicenseInfoEventArgs(false, "Expired");

        Assert.False(args.Valid);
        Assert.Equal("Expired", args.Message);
    }
}

public class SplashCompleteEventArgsTests
{
    [Fact]
    public void Constructor_Sets_Properties()
    {
        var args = new SplashCompleteEventArgs(CloseMethod.Automatically, true);

        Assert.Equal(CloseMethod.Automatically, args.CloseMethod);
        Assert.True(args.Closed);
    }

    [Fact]
    public void Not_Closed()
    {
        var args = new SplashCompleteEventArgs(CloseMethod.None, false);

        Assert.Equal(CloseMethod.None, args.CloseMethod);
        Assert.False(args.Closed);
    }
}

public class CloseMethodTests
{
    [Theory]
    [InlineData(CloseMethod.None, 0)]
    [InlineData(CloseMethod.Automatically, 1)]
    [InlineData(CloseMethod.Manually, 2)]
    public void CloseMethod_Has_Expected_Values(CloseMethod method, int expected)
    {
        Assert.Equal(expected, (int)method);
    }

    [Fact]
    public void CloseMethod_Has_Three_Members()
    {
        Assert.Equal(3, Enum.GetValues<CloseMethod>().Length);
    }
}
