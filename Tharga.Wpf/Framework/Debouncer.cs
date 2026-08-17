namespace Tharga.Wpf.Framework;

internal sealed class Debouncer : IDisposable
{
    private readonly TimeSpan _delay;
    private readonly Timer _timer;

    public Debouncer(TimeSpan delay, Action action)
    {
        _delay = delay;
        _timer = new Timer(_ => action(), null, Timeout.InfiniteTimeSpan, Timeout.InfiniteTimeSpan);
    }

    public void Trigger()
    {
        _timer.Change(_delay, Timeout.InfiniteTimeSpan);
    }

    public void Dispose()
    {
        _timer.Dispose();
    }
}
