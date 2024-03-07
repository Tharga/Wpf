﻿using System.Windows;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Tharga.Wpf.Framework.Exception;

namespace Tharga.Wpf.Sample;

public partial class App
{
    protected override void Register(HostBuilderContext context, IServiceCollection services)
    {
        //TODO: AAA: Auto register view models
        services.AddTransient<MainWindowModel>();

        services.AddTransient<MyService>();
    }

    protected override void Options(ThargaWpfOptions thargaWpfOptions)
    {
        thargaWpfOptions.ApplicationShortName = "Sample";
        thargaWpfOptions.ApplicationFullName = "Tharga Wpf Sample Application";

        thargaWpfOptions.RegisterExceptionHandler<InvalidOperationExceptionHandler, InvalidOperationException>();

        //thargaWpfOptions.ApplicationDownloadLocationLoader += (c) =>
        //{
        //    var baseUri = new Uri(c.GetSection("AggregatorUri").Get<string>());
        //    Uri.TryCreate(baseUri, "api/Agent", out var uri);
        //    return uri;
        //};
    }
}

public class InvalidOperationExceptionHandler : IExceptionHandler<InvalidOperationException>
{
    public void Handle(Window mainWindow, InvalidOperationException exception)
    {
        MessageBox.Show(mainWindow, $"This is a custom error handler for '{exception.Message}'.", exception.GetType().Name);
    }
}

public class MyService
{
}