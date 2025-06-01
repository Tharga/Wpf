using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;

namespace Tharga.Wpf.Framework;

internal static class WindowHelper
{
    private const int SW_RESTORE = 9;

    [DllImport("user32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool SetForegroundWindow(nint hWnd);

    [DllImport("user32.dll")]
    private static extern bool IsIconic(IntPtr hWnd);

    [DllImport("user32.dll")]
    private static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

    public static void FocusWindowByProcessId(int processId)
    {
        var process = Process.GetProcessById(processId);
        var mainWindowHandle = process.MainWindowHandle;
        if (mainWindowHandle != nint.Zero)
        {
            if (IsIconic(mainWindowHandle))
            {
                ShowWindow(mainWindowHandle, SW_RESTORE);
            }

            SetForegroundWindow(mainWindowHandle);
        }
    }
}

internal static class StaticExceptionHandler
{
    public static event EventHandler<StaticErrorEventArgs> ErrorEvent;

    public static void Handle(Exception e, object sender = default)
    {
        ErrorEvent?.Invoke(sender, new StaticErrorEventArgs(e));
    }
}

internal class StaticErrorEventArgs : EventArgs
{
    public StaticErrorEventArgs(Exception exception)
    {
        Exception = exception;
    }

    public Exception Exception { get; }
}