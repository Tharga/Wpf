using System.Collections.Concurrent;
using System.Reflection;
using Microsoft.Extensions.Configuration;
using Tharga.Wpf.ApplicationUpdate;
using Tharga.Wpf.ExceptionHandling;
using Tharga.Wpf.IconTray;

namespace Tharga.Wpf;

public class ThargaWpfOptions
{
    private readonly ConcurrentDictionary<Type, Type> _exceptionTypes = new();
    private readonly ConcurrentDictionary<Type, Type> _exceptionHandlerServices = new();
    private readonly ConcurrentDictionary<Assembly, Assembly> _assemblies = new();

    /// <summary>
    /// Used for folder names when storing window locations.
    /// </summary>
    public string CompanyName { get; set; }

    /// <summary>
    /// Used for folder names and where a brief name is to be used.
    /// </summary>
    public string ApplicationShortName { get; set; }

    /// <summary>
    /// Used on the Splash Screen and as shortcut description.
    /// </summary>
    public string ApplicationFullName { get; set; }

    /// <summary>
    /// Invoke a custom splash window. If this method is not implemented the built-in version will be used.
    /// </summary>
    public Func<SplashData, ISplash> SplashCreator { get; set; }

    /// <summary>
    /// If set to true, multiple tabs with the same title is allowed.
    /// The property AllowTabsWithSameTitles that can be overridden on tab-level controls if tabs of the same type can be opened multiple times.
    /// </summary>
    public bool AllowTabsWithSameTitles { get; set; } = true;

    /// <summary>
    /// If debug is true, the splash will show links to the update location.
    /// </summary>
    public bool Debug { get; set; }

    /// <summary>
    /// The interval between automatic checks for new versions.
    /// If the provided value is TimeSpan.Zero (0), then versions will never be checked, this is default.
    /// Updates can also be performed by calling the CheckForUpdate method in IApplicationUpdateStateService.
    /// </summary>
    public TimeSpan? UpdateIntervalCheck { get; set; }

    /// <summary>
    /// Select the type of system should be used for update. None, Squirrel or Velopack.
    /// </summary>
    public UpdateSystem UpdateSystem { get; set; }

    /// <summary>
    /// Loader for providing the location of the application update location.
    /// </summary>
    public Func<IConfiguration, string> UpdateLocation { get; set; }

    /// <summary>
    /// True if the application can be started several times. No for single application executions where the running application will be focused instead.
    /// </summary>
    public bool AllowMultipleApplications { get; set; } = true;

    /// <summary>
    /// Set this object to specify the behaviour of the icon tray when using INotifyIconService.
    /// </summary>
    public IconTrayData IconTray {get; set;}

    /// <summary>
    /// Registration of customer exception handlers.
    /// </summary>
    /// <typeparam name="THandler"></typeparam>
    /// <typeparam name="TException"></typeparam>
    public void RegisterExceptionHandler<THandler, TException>()
        where THandler : IExceptionHandler<TException>
        where TException : Exception
    {
        _exceptionTypes.TryAdd(typeof(TException), typeof(THandler));
    }

    public void RegisterExceptionHandler<TExceptionHandlerService>()
        where TExceptionHandlerService : IExceptionHandlerService
    {
        _exceptionHandlerServices.TryAdd(typeof(TExceptionHandlerService), typeof(TExceptionHandlerService));
    }

    /// <summary>
    /// Add assemblies where types, like IViewModel, can be found
    /// </summary>
    /// <param name="assembly"></param>
    /// <exception cref="NotImplementedException"></exception>
    public void UseAssembly(Assembly assembly)
    {
        _assemblies.TryAdd(assembly, assembly);
    }

    internal IDictionary<Type, Type> GetExceptionTypes() => _exceptionTypes;
    internal IDictionary<Type, Type> GetExceptionHandlerServices() => _exceptionHandlerServices;
    internal IDictionary<Assembly, Assembly> GetAssemblies() => _assemblies;
}