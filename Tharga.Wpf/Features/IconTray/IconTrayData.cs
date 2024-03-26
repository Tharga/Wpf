using System.Drawing;

namespace Tharga.Wpf.IconTray;

public record IconTrayData
{
    public Icon Icon { get; init; }
    public TrayMenuItem[] Menu { get; init; }
}