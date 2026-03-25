using System.Security.Cryptography;
using Tharga.License;

namespace Tharga.License.Tests;

public class RsaKeyPairTests
{
    [Fact]
    public void Construction_With_Required_Properties()
    {
        using var rsa = RSA.Create(2048);
        var publicKey = RsaPublicKey.FromParameters(rsa.ExportParameters(false));
        var privateKey = RsaPrivateKey.FromParameters(rsa.ExportParameters(true));

        var keyPair = new RsaKeyPair { Public = publicKey, Private = privateKey };

        Assert.Equal(publicKey, keyPair.Public);
        Assert.Equal(privateKey, keyPair.Private);
    }

    [Fact]
    public void Record_Equality_Works()
    {
        using var rsa = RSA.Create(2048);
        var publicKey = RsaPublicKey.FromParameters(rsa.ExportParameters(false));
        var privateKey = RsaPrivateKey.FromParameters(rsa.ExportParameters(true));

        var kp1 = new RsaKeyPair { Public = publicKey, Private = privateKey };
        var kp2 = new RsaKeyPair { Public = publicKey, Private = privateKey };

        Assert.Equal(kp1, kp2);
    }
}
