using System.Windows;

namespace Tharga.Wpf.Features.ApplicationUpdate;

public partial class Splash : ISplash
{
    public Splash(SplashData splashData)
    {
        Owner = splashData.MainWindow;
        Topmost = true;

        MouseDown += (s, e) =>
        {
            DragMove();
        };

        InitializeComponent();

        Messages.Items.MoveCurrentToLast();
        //FirstRun.Visibility = firstRun ? Visibility.Visible : Visibility.Collapsed;
        if (!string.IsNullOrEmpty(splashData.EntryMessage)) Messages.Items.Add(splashData.EntryMessage);
        if (!string.IsNullOrEmpty(splashData.EnvironmentName)) Environment.Text = splashData.EnvironmentName;
        if (!string.IsNullOrEmpty(splashData.Version)) Version.Text = splashData.Version;
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

    public void SetOwner(Window mainWindow)
    {
        Owner = mainWindow;
        Topmost = false;
    }

    private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
    {
        Close();
    }
}