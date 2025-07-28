namespace Tharga.Wpf.ApplicationUpdate;

public class LicenseInfoEventArgs : EventArgs
{
    public LicenseInfoEventArgs(bool valid, string message)
    {
        Valid = valid;
        Message = message;
    }

    public bool Valid { get; }
    public string Message { get; }
}