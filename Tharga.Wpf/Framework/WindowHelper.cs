using System.Diagnostics;
using System.Runtime.InteropServices;

namespace Tharga.Wpf.Framework;

internal static class WindowHelper
{
    [DllImport("user32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool SetForegroundWindow(nint hWnd);

    public static void FocusWindowByProcessId(int processId)
    {
        var process = Process.GetProcessById(processId);
        var mainWindowHandle = process.MainWindowHandle;
        if (mainWindowHandle != nint.Zero)
        {
            SetForegroundWindow(mainWindowHandle);
        }
    }
}