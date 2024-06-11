using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;

namespace Tharga.Wpf.Dialog;

internal static class WindowExtensions
{
    // from winuser.h
    private const int GWL_STYLE = -16, WS_MAXIMIZEBOX = 0x10000, WS_MINIMIZEBOX = 0x20000;

    [DllImport("user32.dll")]
    private static extern int GetWindowLong(IntPtr hwnd, int index);

    [DllImport("user32.dll")]
    private static extern int SetWindowLong(IntPtr hwnd, int index, int value);

    internal static void HideMinimizeAndMaximizeButtons(this Window window)
    {
        var hwnd = new WindowInteropHelper(window).Handle;
        var currentStyle = GetWindowLong(hwnd, GWL_STYLE);

        SetWindowLong(hwnd, GWL_STYLE, currentStyle & ~WS_MAXIMIZEBOX & ~WS_MINIMIZEBOX);
    }
}