using Tharga.Wpf.WindowLocation;

namespace Tharga.Wpf.Framework;

/// <summary>
/// Represents the bounds of a screen/monitor.
/// </summary>
public record ScreenBounds(int Left, int Top, int Width, int Height);

/// <summary>
/// Validates and corrects saved window locations.
/// Pure logic with no UI dependencies — designed for testability.
/// </summary>
public static class LocationValidator
{
    private const int MinWidth = 200;
    private const int MinHeight = 150;

    /// <summary>
    /// Validates a saved location against available screen bounds and returns a corrected location.
    /// </summary>
    /// <param name="location">The saved location to validate.</param>
    /// <param name="screens">Available screen bounds.</param>
    /// <param name="defaultWidth">Default width to use when saved width is invalid.</param>
    /// <param name="defaultHeight">Default height to use when saved height is invalid.</param>
    /// <returns>A corrected location, or the original if valid.</returns>
    public static Location Validate(Location location, IReadOnlyList<ScreenBounds> screens, int defaultWidth = 800, int defaultHeight = 450)
    {
        if (location == null || screens == null || screens.Count == 0)
            return location;

        var width = location.Width;
        var height = location.Height;
        var left = location.Left;
        var top = location.Top;
        var corrected = false;

        if (width < MinWidth)
        {
            width = defaultWidth;
            corrected = true;
        }

        if (height < MinHeight)
        {
            height = defaultHeight;
            corrected = true;
        }

        if (!IsOnAnyScreen(left, top, width, height, screens))
        {
            var primary = screens[0];
            left = primary.Left + (primary.Width - width) / 2;
            top = primary.Top + (primary.Height - height) / 2;
            corrected = true;
        }

        return corrected
            ? location with { Left = left, Top = top, Width = width, Height = height }
            : location;
    }

    private static bool IsOnAnyScreen(int left, int top, int width, int height, IReadOnlyList<ScreenBounds> screens)
    {
        foreach (var screen in screens)
        {
            var overlapLeft = Math.Max(left, screen.Left);
            var overlapTop = Math.Max(top, screen.Top);
            var overlapRight = Math.Min(left + width, screen.Left + screen.Width);
            var overlapBottom = Math.Min(top + height, screen.Top + screen.Height);

            if (overlapRight - overlapLeft > 50 && overlapBottom - overlapTop > 50)
                return true;
        }

        return false;
    }
}
