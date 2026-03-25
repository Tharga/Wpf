using Microsoft.Extensions.DependencyInjection;
using Tharga.License;

namespace Tharga.License.Tests;

public class ThargaLicenseRegistrationTests
{
    [Fact]
    public void AddThargaLicense_Registers_ISigningService()
    {
        var services = new ServiceCollection();

        services.AddThargaLicense();

        var provider = services.BuildServiceProvider();
        var signingService = provider.GetService<ISigningService>();

        Assert.NotNull(signingService);
    }

    [Fact]
    public void AddThargaLicense_Registers_As_Transient()
    {
        var services = new ServiceCollection();

        services.AddThargaLicense();

        var provider = services.BuildServiceProvider();
        var instance1 = provider.GetService<ISigningService>();
        var instance2 = provider.GetService<ISigningService>();

        Assert.NotSame(instance1, instance2);
    }
}
