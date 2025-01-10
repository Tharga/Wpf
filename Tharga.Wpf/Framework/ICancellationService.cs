namespace Tharga.Wpf.Framework;

public interface ICancellationService
{
    CancellationToken Token { get; }
}