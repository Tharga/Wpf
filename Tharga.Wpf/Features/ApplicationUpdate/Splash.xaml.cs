using Microsoft.Extensions.Logging;
using System.Diagnostics;
using System.Text;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;

namespace Tharga.Wpf.ApplicationUpdate;

/// <summary>
/// Built-in splash screen window displaying startup progress and update information.
/// </summary>
public partial class Splash : ISplash
{
    private readonly Action<CloseMethod> _splashClosed;
    private CloseMethod _closeMethod = CloseMethod.Automatically;
    private readonly ILogger<Splash> _logger;

    private void DispatchIfRequired(Action action)
    {
        if (Application.Current.Dispatcher.CheckAccess())
            action();
        else
            Application.Current.Dispatcher.Invoke(action);
    }

    private T DispatchIfRequired<T>(Func<T> func)
    {
        if (Application.Current.Dispatcher.CheckAccess())
            return func();
        return Application.Current.Dispatcher.Invoke(func);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Splash"/> class.
    /// </summary>
    /// <param name="splashData">The data to display on the splash screen.</param>
    public Splash(SplashData splashData)
    {
        _logger = ApplicationBase.GetService<ILogger<Splash>>();

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

        MouseDown += (_, e) => { if (e.LeftButton == MouseButtonState.Pressed) DragMove(); };

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

    /// <inheritdoc />
    public void UpdateInfo(string message)
    {
        _logger.LogInformation(message);
        DispatchIfRequired(() => Messages.Items.Add(message));
    }

    /// <inheritdoc />
    void ISplash.Show()
    {
        _logger.LogInformation("Splash window action {action}.", nameof(Show));
        DispatchIfRequired(Show);
    }

    /// <inheritdoc />
    void ISplash.Hide()
    {
        _logger.LogInformation("Splash window action {action}.", nameof(Hide));
        DispatchIfRequired(Hide);
    }

    /// <inheritdoc />
    void ISplash.Close()
    {
        _logger.LogInformation("Splash window action {action}.", nameof(Close));
        DispatchIfRequired(Close);
    }

    /// <inheritdoc />
    public void SetErrorMessage(string message)
    {
        _logger.LogError(message);
        DispatchIfRequired(() =>
        {
            ErrorMessage.Text = message;
            ErrorMessage.Visibility = Visibility.Visible;
        });
    }

    /// <inheritdoc />
    public void ShowCloseButton()
    {
        _logger.LogInformation("Splash window action {action}.", nameof(ShowCloseButton));
        DispatchIfRequired(() => CloseButton.Visibility = Visibility.Visible);
    }

    /// <inheritdoc />
    public void HideCloseButton()
    {
        _logger.LogInformation("Splash window action {action}.", nameof(HideCloseButton));
        DispatchIfRequired(() => CloseButton.Visibility = Visibility.Collapsed);
    }

    /// <inheritdoc />
    public void ShowProgress()
    {
        _logger.LogInformation("Splash window action {action}.", nameof(ShowProgress));
        DispatchIfRequired(() => UpdateProgressBar.Visibility = Visibility.Visible);
    }

    /// <inheritdoc />
    public void HideProgress()
    {
        _logger.LogInformation("Splash window action {action}.", nameof(HideProgress));
        DispatchIfRequired(() => UpdateProgressBar.Visibility = Visibility.Collapsed);
    }

    /// <inheritdoc />
    public bool IsCloseButtonVisible => DispatchIfRequired(() => CloseButton.Visibility == Visibility.Visible);

    /// <inheritdoc />
    public void ClearMessages()
    {
        _logger.LogInformation("Splash window action {action}.", nameof(ClearMessages));
        DispatchIfRequired(() =>
        {
            Messages.Items.Clear();
            ErrorMessage.Visibility = Visibility.Collapsed;
        });
    }

    private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
    {
        _closeMethod = CloseMethod.Manually;
        Close();
    }

    private void Client_RequestNavigate(object sender, RequestNavigateEventArgs e)
    {
        _logger.LogInformation("{action} Start process /c start {uri}/RELEASES", nameof(Client_RequestNavigate), e.Uri.AbsoluteUri);
        Process.Start(new ProcessStartInfo("cmd", $"/c start {e.Uri.AbsoluteUri}/RELEASES") { CreateNoWindow = true });
        e.Handled = true;
    }

    private void ClientSource_RequestNavigate(object sender, RequestNavigateEventArgs e)
    {
        _logger.LogInformation("{action} Start process /c start {uri}", nameof(ClientSource_RequestNavigate), e.Uri.AbsoluteUri);
        Process.Start(new ProcessStartInfo("cmd", $"/c start {e.Uri.AbsoluteUri}") { CreateNoWindow = true });
        e.Handled = true;
    }

    private void Splash_OnClosed(object sender, EventArgs e)
    {
        _splashClosed?.Invoke(_closeMethod);
    }

    private void Messages_OnMouseDoubleClick(object sender, MouseButtonEventArgs e)
    {
        var sb = new StringBuilder();
        sb.AppendLine($"{ExeLocation.ToolTip}");
        foreach (var log in ApplicationUpdateStateServiceBase.UpdateLog)
        {
            sb.AppendLine(log);
        }
        Clipboard.SetText(sb.ToString());
    }
}