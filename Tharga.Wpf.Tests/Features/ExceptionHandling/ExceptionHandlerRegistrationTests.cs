using System.Windows;
using Microsoft.Extensions.DependencyInjection;
using Tharga.Wpf.ExceptionHandling;

namespace Tharga.Wpf.Tests.Features.ExceptionHandling;

public class ExceptionHandlerRegistrationTests
{
    [Fact]
    public void A_Registered_Exception_Handler_Service_Is_Resolved_As_IExceptionHandlerService()
    {
        using var provider = BuildProvider(options => options.RegisterExceptionHandler<TestExceptionHandlerService>());

        var service = provider.GetService<IExceptionHandlerService>();

        Assert.IsType<TestExceptionHandlerService>(service);
    }

    [Fact]
    public void A_Registered_Exception_Handler_Service_Can_Still_Be_Resolved_By_Its_Own_Type()
    {
        using var provider = BuildProvider(options => options.RegisterExceptionHandler<TestExceptionHandlerService>());

        Assert.NotNull(provider.GetService<TestExceptionHandlerService>());
    }

    [Fact]
    public void Without_A_Registered_Exception_Handler_Service_None_Is_Resolved()
    {
        using var provider = BuildProvider(_ => { });

        Assert.Null(provider.GetService<IExceptionHandlerService>());
    }

    [Fact]
    public void A_Registered_Typed_Exception_Handler_Is_Resolved_By_Its_Own_Type()
    {
        using var provider = BuildProvider(options => options.RegisterExceptionHandler<TestExceptionHandler, InvalidOperationException>());

        Assert.NotNull(provider.GetService<TestExceptionHandler>());
    }

    private static ServiceProvider BuildProvider(Action<ThargaWpfOptions> configure)
    {
        var options = new ThargaWpfOptions();
        configure(options);
        var services = new ServiceCollection();
        ApplicationBase.RegisterExceptionHandler(options, services);
        return services.BuildServiceProvider();
    }

    private class TestExceptionHandlerService : IExceptionHandlerService
    {
        public Task<bool> HandleExceptionAsync(Exception exception, Window mainWindow) => Task.FromResult(true);
    }

    private class TestExceptionHandler : IExceptionHandler<InvalidOperationException>
    {
        public void Handle(Window mainWindow, InvalidOperationException exception) { }
    }
}
