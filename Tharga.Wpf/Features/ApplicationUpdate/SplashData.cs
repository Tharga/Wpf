using System.Windows;

namespace Tharga.Wpf.ApplicationUpdate;

/// <summary>
/// Data provided to the splash screen for display during application startup.
/// </summary>
public record SplashData
{
    /// <summary>The application's main window.</summary>
    public Window MainWindow { get; init; }

    /// <summary>The full name of the application.</summary>
    public string FullName { get; init; }

    /// <summary>Whether this is the first run after installation or update.</summary>
    public bool FirstRun { get; init; }

    /// <summary>The hosting environment name (e.g. "Production", "Development").</summary>
    public string EnvironmentName { get; init; }

    /// <summary>The application version string.</summary>
    public string Version { get; init; }

    /// <summary>The path to the application executable.</summary>
    public string ExeLocation { get; init; }

    /// <summary>An initial message to display on the splash screen.</summary>
    public string EntryMessage { get; init; }

    /// <summary>The URI where the application client can be downloaded.</summary>
    public required Uri ClientLocation { get; init; }

    /// <summary>The URI of the source location for the application client.</summary>
    public required Uri ClientSourceLocation { get; init; }

    /// <summary>Callback invoked when the splash screen is closed.</summary>
    public Action<CloseMethod> SplashClosed { get; init; }

    /// <summary>The path to a custom splash screen background image.</summary>
    public string ImagePath { get; init; }
}