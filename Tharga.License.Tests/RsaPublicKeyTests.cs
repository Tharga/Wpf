using System.Security.Cryptography;
using Tharga.License;

namespace Tharga.License.Tests;

public class RsaPublicKeyTests
{
    [Fact]
    public void FromParameters_And_ToParameters_RoundTrip()
    {
        using var rsa = RSA.Create(2048);
        var original = rsa.ExportParameters(false);

        var publicKey = RsaPublicKey.FromParameters(original);
        var restored = publicKey.ToParameters();

        Assert.Equal(original.Modulus, restored.Modulus);
        Assert.Equal(original.Exponent, restored.Exponent);
    }

    [Fact]
    public void FromParameters_Stores_Base64_Strings()
    {
        using var rsa = RSA.Create(2048);
        var parameters = rsa.ExportParameters(false);

        var publicKey = RsaPublicKey.FromParameters(parameters);

        Assert.NotNull(publicKey.Modulus);
        Assert.NotNull(publicKey.Exponent);
        // Verify they are valid base64
        Convert.FromBase64String(publicKey.Modulus);
        Convert.FromBase64String(publicKey.Exponent);
    }

    [Fact]
    public void Record_Equality_Works()
    {
        using var rsa = RSA.Create(2048);
        var parameters = rsa.ExportParameters(false);

        var key1 = RsaPublicKey.FromParameters(parameters);
        var key2 = RsaPublicKey.FromParameters(parameters);

        Assert.Equal(key1, key2);
    }
}
