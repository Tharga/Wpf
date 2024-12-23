namespace Tharga.Wpf.Framework;

internal class CancellationService : ICancellationService
{
    private readonly CancellationTokenSource _cts = new();

    public CancellationToken Token => _cts.Token;

    public Task CancelAsync()
    {
        return _cts.CancelAsync();
    }
}