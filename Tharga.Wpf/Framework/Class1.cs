using Microsoft.Extensions.DependencyInjection;

namespace Tharga.Wpf.Framework;

public interface IAbstractFactory<T>
{
    T Create();
}

public class AbstractFactory<T> : IAbstractFactory<T>
{
    private readonly Func<T> _factory;

    public AbstractFactory(Func<T> factory)
    {
        _factory = factory;
    }

    public T Create()
    {
        return _factory();
    }
}

public static class ServiceExtensions
{
    public static void AddFormFactory<T>(this IServiceCollection services)
        where T : class
    {
        services.AddTransient<T>();
        services.AddSingleton<Func<T>>(x => () => x.GetService<T>());
        services.AddSingleton<IAbstractFactory<T>, AbstractFactory<T>>();
    }
}