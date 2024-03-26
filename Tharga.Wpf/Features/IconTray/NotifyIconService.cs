using System.Drawing;
using System.Windows;
using System.Windows.Forms;
using Tharga.Wpf.Properties;
using Tharga.Wpf.WindowLocation;

namespace Tharga.Wpf.IconTray;

public class NotifyIconService : INotifyIconService
{
    private readonly NotifyIcon _notifyIcon;

    public NotifyIconService(ThargaWpfOptions thargaWpfOptions)
    {
        _notifyIcon = new NotifyIcon
        {
            Text = thargaWpfOptions.ApplicationFullName,
            Icon = thargaWpfOptions.IconTray?.Icon ?? Resources.thargelion,
            Visible = true
        };

        _notifyIcon.ContextMenuStrip = new ContextMenuStrip();

        if (thargaWpfOptions.IconTray?.Menu?.Any() ?? false)
        {
            foreach (var item in thargaWpfOptions.IconTray.Menu)
            {
                _notifyIcon.ContextMenuStrip.Items.Add(item.Text, item.Image, (s, e) => item.Action?.Invoke(s, e));
            }
            _notifyIcon.ContextMenuStrip.Items.Add(new ToolStripSeparator());
        }

        _notifyIcon.ContextMenuStrip.Items.Add("Exit", Resources.Close, (_, _) => { ApplicationBase.Close(CloseMode.Soft); });
    }

    public void Create(Window window, string name, IWindowLocationService windowLocationService)
    {
        window.Closing += (_, _) => { _notifyIcon.Dispose(); };
        _notifyIcon.DoubleClick += (_, _) =>
        {
            window.Show();
            windowLocationService.SetVisibility(name, window.Visibility);
            if (window.WindowState == WindowState.Minimized) window.WindowState = WindowState.Normal;
        };
    }
}