namespace Tharga.Wpf.ApplicationUpdate;

public class SplashClosedEventArgs : EventArgs
{
    public SplashClosedEventArgs(CloseMethod closeMethod)
    {
        CloseMethod = closeMethod;
    }

    public CloseMethod CloseMethod { get; }
}

public enum CloseMethod
{
    Automatically,
    Manyally
}