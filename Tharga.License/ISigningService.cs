namespace Tharga.License;

public interface ISigningService
{
    RsaKeyPair BuildKeyPair();
    string Sign(string data, RsaPrivateKey privateRsaKey);
    bool VerifySignature(string data, string base64Signature, RsaPublicKey rsaPublicKey);
}