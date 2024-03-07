namespace Tharga.Wpf.Framework;

public interface IAbstractFactory<out T>
{
    T Create();
}