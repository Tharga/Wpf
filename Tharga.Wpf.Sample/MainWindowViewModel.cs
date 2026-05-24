using System.Windows;
using System.Windows.Input;
using Tharga.Wpf.ApplicationUpdate;
using Tharga.Wpf.TabNavigator;

namespace Tharga.Wpf.Sample;

public class MainWindowViewModel : ViewModelBase
{
    private readonly IApplicationUpdateStateService _applicationUpdateStateService;
    private readonly ITabNavigationStateService _tabNavigationStateService;
    private string _message;

    public MainWindowViewModel(MyService myService, IApplicationUpdateStateService applicationUpdateStateService, ITabNavigationStateService tabNavigationStateService)
    {
        _applicationUpdateStateService = applicationUpdateStateService;
        _tabNavigationStateService = tabNavigationStateService;

        _applicationUpdateStateService.UpdateInfoEvent += (_, e) => { Message = e.Message; };

        tabNavigationStateService.OpenTabAsync<MyTabView>("My Tab");


        ShowSplashCommand = new RelayCommand(_ => { _applicationUpdateStateService.ShowSplashAsync(); }, _ => true);
        CheckForUpdateCommand = new RelayCommand(_ => { _applicationUpdateStateService.CheckForUpdateAsync("manual"); }, _ => true);
        ThrowExceptionCommand = new RelayCommand(_ => throw new InvalidOperationException("Some error."), _ => true);
        NewTabCommand = new OpenTabComamnd<MyTabView>(_tabNavigationStateService /*, _authenticationStateService*/);
        NewTabCommand2 = new RelayCommand(_ => _tabNavigationStateService.OpenTabAsync<MyTabView>("My Tab"), _ => true);
        NewTabCommand3 = new RelayCommand(_ => _tabNavigationStateService.OpenTabAsync<FixedTabView>(), _ => true);
        UpdateCommand = new RelayCommand(_ => { _applicationUpdateStateService.ShowSplashAsync(true, true, false); }, _ => true);
        AboutCommand = new RelayCommand(_ => { _applicationUpdateStateService.ShowSplashAsync(false, true, true); }, _ => true);

        ExitCommand = new RelayCommand(_ => { Application.Current?.MainWindow?.Close(); }, _ => true);
        ExitSoftCommand = new RelayCommand(_ => { ApplicationBase.Close(CloseMode.Soft); }, _ => true);
        HideCommand = new RelayCommand(_ => { ApplicationBase.Hide(); }, _ => true);
        OpenChildWindowCommand = new RelayCommand(_ =>
        {
            var existing = Application.Current.Windows.OfType<ChildWindow>().FirstOrDefault();
            if (existing != null)
            {
                existing.Activate();
                return;
            }

            var child = new ChildWindow { Owner = Application.Current.MainWindow };
            child.Show();
        }, _ => true);
    }

    public ICommand ShowSplashCommand { get; set; }
    public ICommand CheckForUpdateCommand { get; set; }
    public ICommand ThrowExceptionCommand { get; set; }
    public ICommand NewTabCommand { get; set; }
    public ICommand NewTabCommand2 { get; set; }
    public ICommand NewTabCommand3 { get; set; }
    public ICommand UpdateCommand { get; set; }
    public ICommand AboutCommand { get; set; }

    public ICommand ExitCommand { get; set; }
    public ICommand ExitSoftCommand { get; set; }
    public ICommand HideCommand { get; set; }
    public ICommand OpenChildWindowCommand { get; set; }

    public string Message
    {
        get => _message;
        set => SetField(ref _message, value);
    }

    public async Task<bool> CloseTabs()
    {
        if (!await _tabNavigationStateService.CloseAllTabsAsync(ApplicationBase.CloseMode == CloseMode.Force))
        {
            return false;
        }

        return true;
    }

    public void Startup()
    {
        _applicationUpdateStateService.ShowSplashAsync(true, false, true);
    }

}