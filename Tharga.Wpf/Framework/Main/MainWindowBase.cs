using Microsoft.Extensions.Configuration;
using Tharga.Wpf.Framework.Exception;

namespace Tharga.Wpf.Framework.Main;

public abstract class MainWindowBase : Fluent.RibbonWindow
{
    protected MainWindowBase(IConfiguration configuration, IExceptionStateService exceptionStateService)
    {
        //_windowLocationService = windowLocationService;

        try
        {
            exceptionStateService.AttachMainWindow(this);

            //    _configuration = configuration;
            //    _apiHealthService = apiHealthService;
            //    _tabNavigationService = tabNavigationService;
            //    _applicationUpdateService = applicationUpdateService;
            //    _authenticationStateService = authenticationStateService;

            //    //TODO: Replace with check. If the application was closed with this window opened, then open it here. (Save with same mechanism as the windows size position and column width)
            //    //if (Debugger.IsAttached) new CommunicationToolWindow().Show();

            //    DataContextChanged += OnDataContextChanged;
            //    Closing += OnClosing;
            //    Activated += (_, _) =>
            //    {
            //        if (!_activated) _applicationUpdateService.AttachMainWindow(this);
            //        _activated = true;
            //    };

            //    _windowLocationService.Monitor(nameof(MainWindow), this);

            //    authenticationStateService.UserChangedEvent += OnUserChanged;
            //    authenticationStateService.AuthenticationEvent += OnAuthentication;
            //    authenticationStateService.AddBeforeSignoutAction(async () => await _tabNavigationService.CloseAllTabsAsync());
        }
        catch (System.Exception e)
        {
            exceptionStateService.FallbackHandler(e);
        }
    }
}