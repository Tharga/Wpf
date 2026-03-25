using System.Reflection;
using Tharga.Wpf;
using Tharga.Wpf.ExceptionHandling;

namespace Tharga.Wpf.Tests;

public class ThargaWpfOptionsTests
{
    [Fact]
    public void Default_AllowTabsWithSameTitles_Is_True()
    {
        var options = new ThargaWpfOptions();

        Assert.True(options.AllowTabsWithSameTitles);
    }

    [Fact]
    public void Default_AllowMultipleApplications_Is_True()
    {
        var options = new ThargaWpfOptions();

        Assert.True(options.AllowMultipleApplications);
    }

    [Fact]
    public void Default_Debug_Is_False()
    {
        var options = new ThargaWpfOptions();

        Assert.False(options.Debug);
    }

    [Fact]
    public void Default_UpdateSystem_Is_None()
    {
        var options = new ThargaWpfOptions();

        Assert.Equal(UpdateSystem.None, options.UpdateSystem);
    }

    [Fact]
    public void Can_Set_CompanyName()
    {
        var options = new ThargaWpfOptions { CompanyName = "Thargelion" };

        Assert.Equal("Thargelion", options.CompanyName);
    }

    [Fact]
    public void Can_Set_ApplicationNames()
    {
        var options = new ThargaWpfOptions
        {
            ApplicationShortName = "Short",
            ApplicationFullName = "Full Name"
        };

        Assert.Equal("Short", options.ApplicationShortName);
        Assert.Equal("Full Name", options.ApplicationFullName);
    }

    [Fact]
    public void UseAssembly_Adds_Assembly()
    {
        var options = new ThargaWpfOptions();
        var assembly = Assembly.GetExecutingAssembly();

        options.UseAssembly(assembly);

        Assert.Contains(assembly, options.GetAssemblies().Keys);
    }

    [Fact]
    public void UseAssembly_Same_Assembly_Twice_Does_Not_Duplicate()
    {
        var options = new ThargaWpfOptions();
        var assembly = Assembly.GetExecutingAssembly();

        options.UseAssembly(assembly);
        options.UseAssembly(assembly);

        Assert.Single(options.GetAssemblies());
    }

    [Fact]
    public void RegisterExceptionHandler_Registers_Type_Pair()
    {
        var options = new ThargaWpfOptions();

        options.RegisterExceptionHandler<TestExceptionHandler, InvalidOperationException>();

        var types = options.GetExceptionTypes();
        Assert.True(types.ContainsKey(typeof(InvalidOperationException)));
        Assert.Equal(typeof(TestExceptionHandler), types[typeof(InvalidOperationException)]);
    }

    [Fact]
    public void RegisterExceptionHandler_Service_Registers_Type()
    {
        var options = new ThargaWpfOptions();

        options.RegisterExceptionHandler<TestExceptionHandlerService>();

        var services = options.GetExceptionHandlerServices();
        Assert.True(services.ContainsKey(typeof(TestExceptionHandlerService)));
    }

    private class TestExceptionHandler : IExceptionHandler<InvalidOperationException>
    {
        public void Handle(System.Windows.Window window, InvalidOperationException exception) { }
    }

    private class TestExceptionHandlerService : IExceptionHandlerService
    {
        public Task<bool> HandleExceptionAsync(Exception exception, System.Windows.Window window) => Task.FromResult(false);
    }
}
