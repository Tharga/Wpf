﻿//using Microsoft.Extensions.DependencyInjection;

//namespace Tharga.Wpf.Framework;

//public static class ServiceExtensions
//{
//    public static void AddFormFactory<T>(this IServiceCollection services)
//        where T : class
//    {
//        services.AddTransient<T>();
//        services.AddSingleton<Func<T>>(x => x.GetService<T>);
//        services.AddSingleton<IAbstractFactory<T>, AbstractFactory<T>>();
//    }
//}