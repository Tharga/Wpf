using System.Windows;

namespace Tharga.Wpf.ApplicationUpdate;

public record SplashData
{
    public Window MainWindow { get; init; }
    public string FullName { get; init; }
    public bool FirstRun { get; init; }
    public string EnvironmentName { get; init; }
    public string Version { get; init; }
    public string ExeLocation { get; init; }
    public string EntryMessage { get; init; }
    public required Uri ClientLocation { get; init; }
    public required Uri ClientSourceLocation { get; init; }
    public Action<CloseMethod> SplashClosed { get; init; }
}