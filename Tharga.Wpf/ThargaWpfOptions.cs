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

    /// <summary>
    /// If debug is true, the splash will show links to the update location.
    /// </summary>
    public bool Debug { get; set; }

    /// <summary>
    /// The interval between automatic checks for new versions.
    /// Default is to check every hour.
    /// If the provided value is 0, then versions will never be checked.
    /// The application always checks for updates at startup. Updates can also be performed by calling the CheckForUpdate method in IApplicationUpdateStateService.
    /// </summary>
    public TimeSpan? CheckForUpdateInterval { get; set; }

    public void RegisterExceptionHandler<THandler, TException>()
        where THandler : IExceptionHandler<TException>
        where TException : Exception
    {
        _exceptionTypes.TryAdd(typeof(TException), typeof(THandler));
    }

    internal IDictionary<Type, Type> GetExceptionTypes() => _exceptionTypes;
}