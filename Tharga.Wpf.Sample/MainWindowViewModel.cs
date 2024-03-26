using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using Tharga.Wpf.ApplicationUpdate;
using Tharga.Wpf.TabNavigator;

namespace Tharga.Wpf.Sample;

public class MainWindowViewModel : IViewModel, INotifyPropertyChanged
{
    private readonly IApplicationUpdateStateService _applicationUpdateStateService;
    private readonly ITabNavigationStateService _tabNavigationStateService;
    private string _message;

    public MainWindowViewModel(MyService myService, IApplicationUpdateStateService applicationUpdateStateService, ITabNavigationStateService tabNavigationStateService)
    {
        _applicationUpdateStateService = applicationUpdateStateService;
        _tabNavigationStateService = tabNavigationStateService;

        _applicationUpdateStateService.UpdateInfoEvent += (s, e) =>
        {
            Message = e.Message;
        };

        tabNavigationStateService.OpenTab<MyTabView>("My Tab");
    }

    public ICommand ShowSplashCommand => new RelayCommand(_ => { _applicationUpdateStateService.ShowSplash(); }, _ => true);
    public ICommand CheckForUpdateCommand => new RelayCommand(_ => { _applicationUpdateStateService.CheckForUpdateAsync(); }, _ => true);
    public ICommand ThrowExceptionCommand => new RelayCommand(_ => throw new InvalidOperationException("Some error."), _ => true);
    public ICommand NewTabCommand => new OpenTabComamnd<MyTabView>(_tabNavigationStateService /*, _authenticationStateService*/);
    public ICommand NewTabCommand2 => new RelayCommand(_ => _tabNavigationStateService.OpenTab<MyTabView>("My Tab"), _ => true);
    public ICommand ExitCommand => new RelayCommand(_ => { ApplicationBase.Close(CloseMode.Soft); }, _ => true);

    public string Message
    {
        get => _message;
        set => SetField(ref _message, value);
    }

    public event PropertyChangedEventHandler PropertyChanged;

    protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    protected bool SetField<T>(ref T field, T value, [CallerMemberName] string propertyName = null)
    {
        if (EqualityComparer<T>.Default.Equals(field, value)) return false;
        field = value;
        OnPropertyChanged(propertyName);
        return true;
    }
}