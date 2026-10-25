using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Controls;

namespace Tharga.Wpf.TabNavigator;

internal class TabNavigatorViewModel : INotifyPropertyChanged
{
    private readonly ITabNavigationStateService _tabNavigationService;

    public TabNavigatorViewModel(ITabNavigationStateService tabNavigationService)
    {
        _tabNavigationService = tabNavigationService;
    }

    public ObservableCollection<TabItem> TabItems => _tabNavigationService.TabItems;

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