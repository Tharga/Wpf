using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Extensions.Configuration;

namespace Tharga.Wpf.Features.TabNavigator;

internal class TabNavigatorViewModel : INotifyPropertyChanged
{
    private readonly ITabNavigationStateService _tabNavigationService;

    public TabNavigatorViewModel(ITabNavigationStateService tabNavigationService, IConfiguration configuration)
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