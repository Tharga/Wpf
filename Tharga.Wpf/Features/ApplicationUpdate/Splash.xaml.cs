using System.Diagnostics;
using System.Windows;
using System.Windows.Navigation;

namespace Tharga.Wpf.ApplicationUpdate;

public partial class Splash : ISplash
{
    public Splash(SplashData splashData)
    {
        if (splashData.MainWindow.Visibility == Visibility.Visible) Owner = splashData.MainWindow;
        Topmost = true;

        MouseDown += (_, _) => DragMove();

        InitializeComponent();

        Messages.Items.MoveCurrentToLast();
        //FirstRun.Visibility = firstRun ? Visibility.Visible : Visibility.Collapsed;
        if (!string.IsNullOrEmpty(splashData.EntryMessage)) Messages.Items.Add(splashData.EntryMessage);
        if (!string.IsNullOrEmpty(splashData.EnvironmentName)) Environment.Text = splashData.EnvironmentName;
        if (!string.IsNullOrEmpty(splashData.Version)) Version.Text = splashData.Version;
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
    }

    public void UpdateInfo(string message)
    {
        Messages.Items.Add(message);
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

    //public void SetOwner(Window mainWindow)
    //{
    //    Owner = mainWindow;
    //    Topmost = false;
    //}

    private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
    {
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
}