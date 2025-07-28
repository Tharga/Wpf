using Microsoft.Extensions.DependencyInjection;

namespace Tharga.License;

public interface ISigningService
{
    RsaKeyPair BuildKeyPair();
    string Sign(string data, RsaPrivateKey privateRsaKey);
    bool VerifySignature(string data, string base64Signature, RsaPublicKey rsaPublicKey);
}

public static class ThargaLicenseRegistration
{
    public static void AddThargaLicense(this IServiceCollection services)
    {
        services.AddTransient<ISigningService, SigningService>();
    }
}