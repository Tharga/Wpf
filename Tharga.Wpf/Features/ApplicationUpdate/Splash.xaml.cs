using System.Windows;

namespace Tharga.Wpf.Features.ApplicationUpdate;

public partial class Splash
{
    public Splash(Window mainWindow, bool firstRun, string environmentName, string version, string entryMessage)
    {
        Owner = mainWindow;
        Topmost = true;

        MouseDown += (s, e) =>
        {
            DragMove();
        };

        InitializeComponent();

        Messages.Items.MoveCurrentToLast();
        //FirstRun.Visibility = firstRun ? Visibility.Visible : Visibility.Collapsed;
        if (!string.IsNullOrEmpty(entryMessage)) Messages.Items.Add(entryMessage);
        if (!string.IsNullOrEmpty(environmentName)) Environment.Text = environmentName;
        if (!string.IsNullOrEmpty(environmentName)) Version.Text = version;
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