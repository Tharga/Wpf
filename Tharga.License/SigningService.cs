using System.Security.Cryptography;
using System.Text;

namespace Tharga.License;

internal class SigningService : ISigningService
{
    public RsaKeyPair BuildKeyPair()
    {
        // Generate a key pair
        var rsa = RSA.Create(2048);
        var privateParams = rsa.ExportParameters(true);
        var publicParams = rsa.ExportParameters(false);

        // Convert to models
        var publicKey = RsaPublicKey.FromParameters(publicParams);
        var privateKey = RsaPrivateKey.FromParameters(privateParams);

        return new RsaKeyPair { Public = publicKey, Private = privateKey };
    }

    public string Sign(string data, RsaPrivateKey privateRsaKey)
    {
        using var rsa = RSA.Create();
        rsa.ImportParameters(privateRsaKey.ToParameters());

        var bytes = Encoding.UTF8.GetBytes(data);
        var signature = rsa.SignData(bytes, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);

        return Convert.ToBase64String(signature);
    }

    public bool VerifySignature(string data, string base64Signature, RsaPublicKey rsaPublicKey)
    {
        using var rsa = RSA.Create();
        rsa.ImportParameters(rsaPublicKey.ToParameters());

        var bytes = Encoding.UTF8.GetBytes(data);
        var signature = Convert.FromBase64String(base64Signature);

        return rsa.VerifyData(bytes, signature, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);
    }
}