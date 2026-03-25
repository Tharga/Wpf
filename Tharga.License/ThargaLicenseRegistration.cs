using Microsoft.Extensions.DependencyInjection;

namespace Tharga.License;

/// <summary>
/// Extension methods for registering Tharga.License services with the dependency injection container.
/// </summary>
public static class ThargaLicenseRegistration
{
    /// <summary>
    /// Registers the <see cref="ISigningService"/> implementation as a transient service.
    /// </summary>
    /// <param name="services">The service collection to register with.</param>
    public static void AddThargaLicense(this IServiceCollection services)
    {
        services.AddTransient<ISigningService, SigningService>();
    }
}