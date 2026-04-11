using System.Linq;
using System.Windows;
using Tharga.Wpf.ApplicationUpdate;
using Tharga.Wpf.TabNavigator;
using Tharga.Wpf.WindowLocation;

namespace Tharga.Wpf;

/// <summary>
/// Extension methods for <see cref="ApplicationBase"/> to simplify main window startup.
/// </summary>
public static class AppExtensions
{
    /// <summary>
    /// Creates, registers, and shows the main window with optional splash screen.
    /// Handles the full window lifecycle including hide-on-close, tab closing, and startup visibility.
    /// </summary>
    /// <typeparam name="T">The main window type.</typeparam>
    /// <param name="app">The application instance.</param>
    /// <param name="showTrayIcon">If true, creates a system tray icon.</param>
    /// <param name="showSplash">If true, shows a splash screen during startup.</param>
    /// <param name="checkForUpdates">If true, checks for application updates during splash.</param>
    /// <param name="checkForLicense">If true, checks license validity during splash.</param>
    public static void StartMainWindow<T>(this ApplicationBase app, bool showTrayIcon = false, bool showSplash = true, bool checkForUpdates = true, bool checkForLicense = false)
        where T : Window, new()
    {
        var mainWindow = new T();
        mainWindow.RegisterMainWindow(showTrayIcon);

        mainWindow.Closing += async (_, e) =>
        {
            if (ApplicationBase.HandleClose(e)) return;

            var force = ApplicationBase.CloseMode == CloseMode.Force;

            // Try closing owned windows first — if any refuses, cancel (unless force).
            var ownedWindows = mainWindow.OwnedWindows.Cast<Window>().ToList();
            foreach (var owned in ownedWindows)
            {
                owned.Close();
                if (!force && owned.IsLoaded)
                {
                    var windowName = owned.Title ?? owned.GetType().Name;
                    MessageBox.Show(
                        $"Cannot close because '{windowName}' prevented closing.",
                        "Close blocked",
                        MessageBoxButton.OK,
                        MessageBoxImage.Information);
                    e.Cancel = true;
                    return;
                }
            }

            var tabNavigationStateService = ApplicationBase.GetService<ITabNavigationStateService>();
            if (!await tabNavigationStateService.CloseAllTabsAsync(force))
            {
                e.Cancel = true;
            }
        };

        Application.Current.Dispatcher.InvokeAsync(async () =>
        {
            if (showSplash)
            {
                var updateService = ApplicationBase.GetService<IApplicationUpdateStateService>();
                await updateService.ShowSplashAsync(checkForUpdates, checkForLicense: checkForLicense);
            }

            var locationService = ApplicationBase.GetService<IWindowLocationService>();
            var name = string.IsNullOrEmpty(mainWindow.Name) ? mainWindow.GetType().Name : mainWindow.Name;
            if (((WindowLocationService)locationService).ShouldShowOnStartup(name))
            {
                mainWindow.Show();
            }
        });
    }
}