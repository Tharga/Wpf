using System.Collections.Concurrent;
using Microsoft.Extensions.Configuration;
using Tharga.Wpf.ApplicationUpdate;
using Tharga.Wpf.ExceptionHandling;

namespace Tharga.Wpf;

public class ThargaWpfOptions
{
    private readonly ConcurrentDictionary<Type, Type> _exceptionTypes = new();

    public string ApplicationShortName { get; set; }
    public string ApplicationFullName { get; set; }
    public Func<SplashData, ISplash> SplashCreator { get; set; }
    public Func<IConfiguration, Uri> ApplicationDownloadLocationLoader { get; set; }
    public bool AllowTabsWithSameTitles { get; set; }

    public void RegisterExceptionHandler<THandler, TException>()
        where THandler : IExceptionHandler<TException>
        where TException : Exception
    {
        _exceptionTypes.TryAdd(typeof(TException), typeof(THandler));
    }

    internal IDictionary<Type, Type> GetExceptionTypes() => _exceptionTypes;
}