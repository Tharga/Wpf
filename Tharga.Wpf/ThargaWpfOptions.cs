using System.Collections.Concurrent;
using Tharga.Wpf.Features.ApplicationUpdate;
using Tharga.Wpf.Framework.Exception;

namespace Tharga.Wpf;

public class ThargaWpfOptions
{
    private readonly ConcurrentDictionary<Type, Type> _exceptionTypes = new();

    public string ApplicationShortName { get; set; }
    public string ApplicationFullName { get; set; }
    public Func<SplashData, ISplash> SplashCreator { get; set; }

    public void RegisterExceptionHandler<THandler, TException>()
        where THandler : IExceptionHandler<TException>
        where TException : Exception
    {
        _exceptionTypes.TryAdd(typeof(TException), typeof(THandler));
    }

    internal IDictionary<Type, Type> GetExceptionTypes() => _exceptionTypes;
}