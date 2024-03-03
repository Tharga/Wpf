using System.Collections.Concurrent;
using Microsoft.Extensions.DependencyInjection;
using Tharga.Wpf.Features.ApplicationUpdate;
using Tharga.Wpf.Framework.Exception;

namespace Tharga.Wpf;

public class ThargaWpfOptions
{
    public string ApplicationShortName { get; set; }
    public string ApplicationFullName { get; set; }
    public Func<SplashData, ISplash> SplashCreator { get; set; }

    private readonly ConcurrentDictionary<Type, Type> _exceptionTypes = new();
    //private readonly IList<Action<IServiceCollection>> _serviceRegistrations = new List<Action<IServiceCollection>>();

    //public void RegisterService(Action<IServiceCollection> serviceCollection)
    //{
    //    _serviceRegistrations.Add(serviceCollection);
    //}

    public void RegisterExceptionHandler<THandler, TException>()
        where THandler : IExceptionHandler<TException>
        where TException : Exception
    {
        _exceptionTypes.TryAdd(typeof(TException), typeof(THandler));
    }

    internal IDictionary<Type, Type> GetExceptionTypes() => _exceptionTypes;
    //internal IEnumerable<Action<IServiceCollection>> GetServiceRegistrations() => _serviceRegistrations;
}