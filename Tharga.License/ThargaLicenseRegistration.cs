using Microsoft.Extensions.DependencyInjection;

namespace Tharga.License;

public static class ThargaLicenseRegistration
{
    public static void AddThargaLicense(this IServiceCollection services)
    {
        services.AddTransient<ISigningService, SigningService>();
    }
}