using System.Drawing;

namespace Tharga.Wpf.IconTray;

public record TrayMenuItem
{
    public string Text { get; init; }
    public Image Image { get; init; }
    public Action<object, EventArgs> Action { get; init; }
}