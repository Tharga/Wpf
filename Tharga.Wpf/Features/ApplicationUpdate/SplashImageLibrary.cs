namespace Tharga.Wpf.ApplicationUpdate;

/// <summary>
/// Provides pack URI paths to built-in splash screen background images.
/// Dark images (Colours, Fire, Mosaik, Prism, Silicon) typically need a light
/// foreground brush — set <c>ThargaWpfOptions.SplashForeground</c> to a bright
/// brush (e.g. <c>Brushes.White</c>) so the overlaid text remains readable.
/// </summary>
public record SplashImageLibrary
{
    /// <summary>Blue background image.</summary>
    public static string Blue = "pack://application:,,,/Tharga.Wpf;component/Images/Application/blue.png";
    /// <summary>Dark abstract multi-colour background image. Needs a light foreground brush.</summary>
    public static string Colours = "pack://application:,,,/Tharga.Wpf;component/Images/Application/colours.png";
    /// <summary>Dark fire background image. Needs a light foreground brush.</summary>
    public static string Fire = "pack://application:,,,/Tharga.Wpf;component/Images/Application/fire.png";
    /// <summary>Green background image.</summary>
    public static string Green = "pack://application:,,,/Tharga.Wpf;component/Images/Application/green.png";
    /// <summary>Green transparent background image.</summary>
    public static string GreenTransparent = "pack://application:,,,/Tharga.Wpf;component/Images/Application/green-t.png";
    /// <summary>Dark mosaic-fractal background image. Needs a light foreground brush.</summary>
    public static string Mosaik = "pack://application:,,,/Tharga.Wpf;component/Images/Application/mosaik.png";
    /// <summary>Orange background image.</summary>
    public static string Orange = "pack://application:,,,/Tharga.Wpf;component/Images/Application/orange.png";
    /// <summary>Dark prism background image. Needs a light foreground brush.</summary>
    public static string Prism = "pack://application:,,,/Tharga.Wpf;component/Images/Application/prism.png";
    /// <summary>Red background image.</summary>
    public static string Red = "pack://application:,,,/Tharga.Wpf;component/Images/Application/red.png";
    /// <summary>Red transparent background image.</summary>
    public static string RedTransparent = "pack://application:,,,/Tharga.Wpf;component/Images/Application/red-t.png";
    /// <summary>Dark silicon-chip background image. Needs a light foreground brush.</summary>
    public static string Silicon = "pack://application:,,,/Tharga.Wpf;component/Images/Application/silicon.png";
    /// <summary>Teal background image.</summary>
    public static string Teal = "pack://application:,,,/Tharga.Wpf;component/Images/Application/teal.png";
    /// <summary>Teal transparent background image.</summary>
    public static string TealTransparent = "pack://application:,,,/Tharga.Wpf;component/Images/Application/teal-t.png";
    /// <summary>White background image.</summary>
    public static string White = "pack://application:,,,/Tharga.Wpf;component/Images/Application/white.png";
    /// <summary>Yellow background image.</summary>
    public static string Yellow = "pack://application:,,,/Tharga.Wpf;component/Images/Application/yellow.png";
}
