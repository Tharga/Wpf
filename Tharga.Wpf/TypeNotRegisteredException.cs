namespace Tharga.Wpf;

internal class TypeNotRegisteredException : Exception
{
    public TypeNotRegisteredException(Type type, string extraMessage)
        : base($"Cannot find tyoe '{type.Name}'. Perhaps it has not been registered in the IOC. Types that implements the interface '{nameof(IViewModel)}' is automatically registered.{extraMessage}")
    {
    }
}