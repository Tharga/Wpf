using System.Diagnostics;
using System.Text;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;

namespace Tharga.Wpf.ApplicationUpdate;

public partial class Splash : ISplash
{
    private readonly Action<CloseMethod> _splashClosed;
    private CloseMethod _closeMethod = CloseMethod.Automatically;

    public Splash(SplashData splashData)
    {
        if (splashData.MainWindow.Visibility == Visibility.Visible)
        {
            Owner = splashData.MainWindow;
            WindowStartupLocation = WindowStartupLocation.CenterOwner;
        }
        else
        {
            WindowStartupLocation = WindowStartupLocation.CenterScreen;
        }
        //Topmost = true;

        MouseDown += (_, _) => DragMove();

        InitializeComponent();

        if (!string.IsNullOrEmpty(splashData.ImagePath))
        {
            Image.Source = new BitmapImage(new Uri(splashData.ImagePath));
        }

        Messages.Items.MoveCurrentToLast();
        //FirstRun.Visibility = firstRun ? Visibility.Visible : Visibility.Collapsed;
        if (!string.IsNullOrEmpty(splashData.EntryMessage)) Messages.Items.Add(splashData.EntryMessage);
        if (!string.IsNullOrEmpty(splashData.EnvironmentName)) Environment.Text = splashData.EnvironmentName;
        if (!string.IsNullOrEmpty(splashData.Version)) Version.Text = splashData.Version;
        if (!string.IsNullOrEmpty(splashData.ExeLocation))
        {
            ExeLocation.Text = System.IO.Path.GetFileName(splashData.ExeLocation);
            ExeLocation.ToolTip = splashData.ExeLocation;
            ExeLocation.Tag = splashData.ExeLocation;
        }
        if (!string.IsNullOrEmpty(splashData.FullName)) FullName.Text = splashData.FullName;

        if (splashData.ClientLocation != null)
        {
            ClientLocation.NavigateUri = splashData.ClientLocation;
        }
        else
        {
            Client.Visibility = Visibility.Collapsed;
        }

        if (splashData.ClientSourceLocation != null)
        {
            ClientSourceLocation.NavigateUri = splashData.ClientSourceLocation;
        }
        else
        {
            ClientSource.Visibility = Visibility.Collapsed;
        }

        _splashClosed = splashData.SplashClosed;
    }

    public void UpdateInfo(string message)
    {
        Application.Current.Dispatcher.Invoke(() => Messages.Items.Add(message));
    }

    public void SetErrorMessage(string message)
    {
        ErrorMessage.Text = message;
        ErrorMessage.Visibility = Visibility.Visible;
    }

    public void ShowCloseButton()
    {
        CloseButton.Visibility = Visibility.Visible;
    }

    public bool IsCloseButtonVisible => CloseButton.Visibility == Visibility.Visible;

    public void ClearMessages()
    {
        Messages.Items.Clear();
        ErrorMessage.Visibility = Visibility.Collapsed;
    }

    private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
    {
        _closeMethod = CloseMethod.Manually;
        Close();
    }

    private void Client_RequestNavigate(object sender, RequestNavigateEventArgs e)
    {
        Process.Start(new ProcessStartInfo("cmd", $"/c start {e.Uri.AbsoluteUri}/RELEASES") { CreateNoWindow = true });
        e.Handled = true;
    }

    private void ClientSource_RequestNavigate(object sender, RequestNavigateEventArgs e)
    {
        Process.Start(new ProcessStartInfo("cmd", $"/c start {e.Uri.AbsoluteUri}") { CreateNoWindow = true });
        e.Handled = true;
    }

    private void ExeLocation_OnMouseDown(object sender, MouseButtonEventArgs e)
    {
        var sb = new StringBuilder();
        sb.AppendLine($"{ExeLocation.ToolTip}");
        foreach (var log in ApplicationUpdateStateService.UpdateLog)
        {
            sb.AppendLine(log);
        }
        Clipboard.SetText(sb.ToString());
    }

    private void Splash_OnClosed(object sender, EventArgs e)
    {
        _splashClosed?.Invoke(_closeMethod);
    }
}