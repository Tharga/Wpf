namespace Tharga.Wpf.ApplicationUpdate;

public class SplashCompleteEventArgs : EventArgs
{
    public SplashCompleteEventArgs(CloseMethod closeMethod, bool closed)
    {
        CloseMethod = closeMethod;
        Closed = closed;
    }

    public CloseMethod CloseMethod { get; }
    public bool Closed { get; }
}