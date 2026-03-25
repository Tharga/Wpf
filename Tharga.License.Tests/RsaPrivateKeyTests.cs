using System.Security.Cryptography;
using Tharga.License;

namespace Tharga.License.Tests;

public class RsaPrivateKeyTests
{
    [Fact]
    public void FromParameters_And_ToParameters_RoundTrip()
    {
        using var rsa = RSA.Create(2048);
        var original = rsa.ExportParameters(true);

        var privateKey = RsaPrivateKey.FromParameters(original);
        var restored = privateKey.ToParameters();

        Assert.Equal(original.Modulus, restored.Modulus);
        Assert.Equal(original.Exponent, restored.Exponent);
        Assert.Equal(original.D, restored.D);
        Assert.Equal(original.P, restored.P);
        Assert.Equal(original.Q, restored.Q);
        Assert.Equal(original.DP, restored.DP);
        Assert.Equal(original.DQ, restored.DQ);
        Assert.Equal(original.InverseQ, restored.InverseQ);
    }

    [Fact]
    public void FromParameters_Stores_All_Components_As_Base64()
    {
        using var rsa = RSA.Create(2048);
        var parameters = rsa.ExportParameters(true);

        var privateKey = RsaPrivateKey.FromParameters(parameters);

        Assert.NotNull(privateKey.Modulus);
        Assert.NotNull(privateKey.Exponent);
        Assert.NotNull(privateKey.D);
        Assert.NotNull(privateKey.P);
        Assert.NotNull(privateKey.Q);
        Assert.NotNull(privateKey.DP);
        Assert.NotNull(privateKey.DQ);
        Assert.NotNull(privateKey.InverseQ);

        // All should be valid base64
        Convert.FromBase64String(privateKey.Modulus);
        Convert.FromBase64String(privateKey.D);
        Convert.FromBase64String(privateKey.P);
    }

    [Fact]
    public void Record_Equality_Works()
    {
        using var rsa = RSA.Create(2048);
        var parameters = rsa.ExportParameters(true);

        var key1 = RsaPrivateKey.FromParameters(parameters);
        var key2 = RsaPrivateKey.FromParameters(parameters);

        Assert.Equal(key1, key2);
    }
}
