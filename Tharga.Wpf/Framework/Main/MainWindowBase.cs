using System.Windows;
using Tharga.Wpf.Features.ApplicationUpdate;
using Tharga.Wpf.Features.WindowLocation;
using Tharga.Wpf.Framework.Exception;

namespace Tharga.Wpf.Framework.Main;

public abstract class MainWindowBase : Fluent.RibbonWindow
{
    private readonly IApplicationUpdateStateService _applicationUpdateService;
    private bool _activated;

    protected MainWindowBase()
    {
        try
        {
            _applicationUpdateService = ApplicationBase.GetService<IApplicationUpdateStateService>();
            var windowLocationService = ApplicationBase.GetService<IWindowLocationService>();
            var exceptionStateService = ApplicationBase.GetService<IExceptionStateService>();

            exceptionStateService.AttachMainWindow(this);

            Closing += OnClosing;
            Activated += (_, _) =>
            {
                if (!_activated)
                {
                    _applicationUpdateService.AttachMainWindow(this);
                    _ = CheckForClientUpdate();
                }
                _activated = true;
            };

            windowLocationService.Monitor(nameof(MainWindowBase), this);

            //authenticationStateService.UserChangedEvent += OnUserChanged;
            //authenticationStateService.AuthenticationEvent += OnAuthentication;
            //authenticationStateService.AddBeforeSignoutAction(async () => await _tabNavigationService.CloseAllTabsAsync());
        }
        catch (System.Exception e)
        {
            e.FallbackHandler(this);
        }
    }

    private async Task CheckForClientUpdate() //MainWindowViewModel dataContext, StatusBarModel statusBarModel)
    {
        await _applicationUpdateService.StartUpdateLoop();
    }

    private async void OnClosing(object sender, System.ComponentModel.CancelEventArgs e)
    {
        e.Cancel = true;
        await RequestClose();
    }

    //NOTE: For some reason we cannot check this directly in the OnClosing function. If there is a delay the application will not complete all exit checks before it terminates.
    private async Task RequestClose()
    {
        //var allowClose = await _tabNavigationService.CloseAllTabsAsync();
        //if (allowClose)
        //{
            await Task.Delay(200); //This is needed, or we will get an error message on close.
            Closing -= OnClosing;
            Application.Current?.MainWindow?.Close();
        //}
    }
}