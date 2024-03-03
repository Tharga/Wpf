using Microsoft.Extensions.Configuration;
using System.Windows;
using Tharga.Wpf.Features.ApplicationUpdate;
using Tharga.Wpf.Framework.Exception;

namespace Tharga.Wpf.Framework.Main;

public abstract class MainWindowBase : Fluent.RibbonWindow
{
    private readonly IConfiguration _configuration;
    private readonly IApplicationUpdateStateService _applicationUpdateService;
    private readonly IWindowLocationService _windowLocationService;
    private bool _activated;

    protected MainWindowBase(IConfiguration configuration, IExceptionStateService exceptionStateService, IApplicationUpdateStateService applicationUpdateService, IWindowLocationService windowLocationService)
    {
        _applicationUpdateService = applicationUpdateService;
        _windowLocationService = windowLocationService;

        try
        {
            exceptionStateService.AttachMainWindow(this);

            _configuration = configuration;
            //    _apiHealthService = apiHealthService;
            //    _tabNavigationService = tabNavigationService;
            _applicationUpdateService = applicationUpdateService;
            //    _authenticationStateService = authenticationStateService;

            //    //TODO: Replace with check. If the application was closed with this window opened, then open it here. (Save with same mechanism as the windows size position and column width)
            //    //if (Debugger.IsAttached) new CommunicationToolWindow().Show();

            DataContextChanged += OnDataContextChanged;
            Closing += OnClosing;
            Activated += (_, _) =>
            {
                if (!_activated) _applicationUpdateService.AttachMainWindow(this);
                _activated = true;
            };

            _windowLocationService.Monitor(nameof(MainWindowBase), this);

            //    authenticationStateService.UserChangedEvent += OnUserChanged;
            //    authenticationStateService.AuthenticationEvent += OnAuthentication;
            //    authenticationStateService.AddBeforeSignoutAction(async () => await _tabNavigationService.CloseAllTabsAsync());
        }
        catch (System.Exception e)
        {
            e.FallbackHandler(this);
        }
    }

    protected abstract void OnDataContextChanged(object dataContext);

    private async void OnDataContextChanged(object sender, DependencyPropertyChangedEventArgs ev)
    {
        OnDataContextChanged(DataContext); //_dataContext = (MainWindowViewModel)DataContext;

        //var statusBarModel = UpdateClientInfo(_dataContext);
        //statusBarModel = await UpdateServerInfo(_dataContext, statusBarModel);
        await CheckForClientUpdate(); //_dataContext, statusBarModel);
        //_dataContext.LoginCommand.Execute(null);

        //_windowLocationService.AttachProperty(nameof(MainWindow), _dataContext, nameof(MainWindowViewModel.UiScale));
        //_windowLocationService.AttachProperty(nameof(MainWindow), _dataContext, nameof(MainWindowViewModel.TabUiScale));
    }

    private async Task CheckForClientUpdate() //MainWindowViewModel dataContext, StatusBarModel statusBarModel)
    {
        //_applicationUpdateService.UpdateInfoEvent += (_, e) =>
        //{
        //    dataContext.OnStatusBar(statusBarModel with { ClientUpdateInfo = e.Message });
        //};
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