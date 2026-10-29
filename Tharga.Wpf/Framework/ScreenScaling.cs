using System.Drawing;
using System.Runtime.InteropServices;

namespace Tharga.Wpf.Framework;

/// <summary>
/// Converts monitor work areas from the coordinates Windows reports to this process into WPF device-independent units (1/96 inch),
/// the unit of <c>Window.Left</c>, <c>Top</c>, <c>Width</c> and <c>Height</c> and of saved window locations.
/// </summary>
internal static class ScreenScaling
{
    internal const double StandardDpi = 96;

    private const uint MonitorDefaultToNearest = 2;
    private const int EffectiveDpi = 0;
    private const int Ok = 0;

    /// <summary>
    /// Divides the work area by the monitor's scale factor. The result is rounded inwards, so it never extends beyond the monitor.
    /// </summary>
    /// <param name="bounds">The work area as Windows reports it.</param>
    /// <param name="dpi">The monitor's DPI as Windows reports it to this process. A value that is not positive is treated as 96 (100%).</param>
    public static ScreenBounds ToDeviceIndependent(ScreenBounds bounds, double dpi)
    {
        var scale = dpi > 0 ? dpi / StandardDpi : 1;
        var left = (int)Math.Ceiling(bounds.Left / scale);
        var top = (int)Math.Ceiling(bounds.Top / scale);
        var right = (int)Math.Floor((bounds.Left + bounds.Width) / scale);
        var bottom = (int)Math.Floor((bounds.Top + bounds.Height) / scale);

        return bounds with { Left = left, Top = top, Width = right - left, Height = bottom - top };
    }

    /// <summary>
    /// Gets the effective DPI of the monitor that contains the centre of <paramref name="monitorBounds"/>.
    /// Windows reports it for this process's DPI awareness: the monitor's own DPI when per-monitor aware, the system DPI when system aware, and 96 when not DPI aware.
    /// That is the factor between the coordinates this process sees and device-independent units. Returns 96 when it cannot be read.
    /// </summary>
    public static double GetDpi(Rectangle monitorBounds)
    {
        try
        {
            var centre = new NativePoint { X = monitorBounds.Left + monitorBounds.Width / 2, Y = monitorBounds.Top + monitorBounds.Height / 2 };
            var monitor = MonitorFromPoint(centre, MonitorDefaultToNearest);
            if (monitor == nint.Zero) return StandardDpi;

            return GetDpiForMonitor(monitor, EffectiveDpi, out var dpiX, out _) == Ok && dpiX > 0 ? dpiX : StandardDpi;
        }
        catch (Exception e) when (e is DllNotFoundException or EntryPointNotFoundException)
        {
            return StandardDpi;
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    private struct NativePoint
    {
        public int X;
        public int Y;
    }

    [DllImport("user32.dll")]
    private static extern nint MonitorFromPoint(NativePoint pt, uint flags);

    [DllImport("shcore.dll")]
    private static extern int GetDpiForMonitor(nint monitor, int dpiType, out uint dpiX, out uint dpiY);
}
