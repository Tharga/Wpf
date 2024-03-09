using System.Reflection;

namespace Tharga.Wpf.Framework.TypeRegistration;

/// <summary>
/// This code is copied from Tharga.Toolkit.TypeService.AssemblyService since we do not want any direct references there.
/// This class should not be exposed externally, use Tharga.Toolkit from external assemblies instead.
/// </summary>
internal static class TypeHelper
{
    public static IEnumerable<Type> GetTypesBasedOn<T>()
    {
        var asms = GetAssemblies();
        var types = GetTypes(x => !x.IsAbstract && x.IsOfType(typeof(T)), asms);
        return types;
    }

    private static IEnumerable<Assembly> GetAssemblies()
    {
        var current = Assembly.GetEntryAssembly() ?? Assembly.GetExecutingAssembly();
        var name = current.GetName().Name?.Split('.').First();

        var appDomainAssemblies = AppDomain.CurrentDomain.GetAssemblies()
            .Where(x => name != null && x.FullName != null && x.FullName.Contains(name))
            .ToArray();

        return new[] { current }.Union(appDomainAssemblies).ToArray();
    }

    private static IEnumerable<TypeInfo> GetTypes(Func<TypeInfo, bool> filter = null, IEnumerable<Assembly> assemblies = null)
    {
        var assms = GetAssemblies(assemblies);
        var types = assms.SelectMany(x => x.DefinedTypes)
            .Where(x => filter?.Invoke(x) ?? true);
        return types;
    }

    private static IEnumerable<Assembly> GetAssemblies(IEnumerable<Assembly> assemblies)
    {
        if (assemblies != null) return assemblies.ToArray();

        var current = Assembly.GetEntryAssembly() ?? Assembly.GetExecutingAssembly();
        var name = current.GetName().Name?.Split('.').First();

        var appDomainAssemblies = AppDomain.CurrentDomain.GetAssemblies()
            .Where(x => name != null && x.FullName != null && x.FullName.Contains(name))
            .ToArray();

        return new[] { current }.Union(appDomainAssemblies).ToArray();
    }

    private static bool IsOfType(this Type item, Type type)
    {
        if (item == null) return false;
        if (item == type) return true;
        if (item.ToString().Split('[')[0] == type.ToString().Split('[')[0]) return true;

        if (type.IsAssignableFrom(item))
        {
            return true;
        }

        if (item.BaseType != null && item.BaseType != typeof(object))
        {
            return item.BaseType.IsOfType(type);
        }

        foreach (var inf in item.GetInterfaces())
        {
            var r = inf.IsOfType(type);
            if (r) return true;
        }

        return false;
    }
}