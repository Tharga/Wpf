using Tharga.License;

namespace Tharga.License.Tests;

public class FingerprintTests
{
    [Fact]
    public void Construction_With_Required_Properties()
    {
        var fingerprint = new Fingerprint { Name = "Machine", Value = "ABC123" };

        Assert.Equal("Machine", fingerprint.Name);
        Assert.Equal("ABC123", fingerprint.Value);
    }

    [Fact]
    public void Record_Equality_Works()
    {
        var fp1 = new Fingerprint { Name = "Machine", Value = "ABC123" };
        var fp2 = new Fingerprint { Name = "Machine", Value = "ABC123" };

        Assert.Equal(fp1, fp2);
    }

    [Fact]
    public void Record_Inequality_For_Different_Values()
    {
        var fp1 = new Fingerprint { Name = "Machine", Value = "ABC123" };
        var fp2 = new Fingerprint { Name = "Machine", Value = "DEF456" };

        Assert.NotEqual(fp1, fp2);
    }
}
