using System.Windows;

namespace Tharga.Wpf.WindowLocation;

public record Location
{
    public WindowState WindowState { get; init; }
    public Visibility Visibility { get; init; }
    public int Left { get; init; }
    public int Top { get; init; }
    public int Width { get; init; }
    public int Height { get; init; }
    public readonly Dictionary<string, string> Metadata = new();
}