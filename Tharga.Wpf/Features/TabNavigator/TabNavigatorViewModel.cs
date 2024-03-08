using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Extensions.Configuration;

namespace Tharga.Wpf.Features.TabNavigator;

public class TabNavigatorViewModel : INotifyPropertyChanged
{
    public static readonly double DefaultUiScale = 1;

    private readonly ITabNavigationStateService _tabNavigationService;

    public TabNavigatorViewModel(ITabNavigationStateService tabNavigationService, IConfiguration configuration)
    {
        _tabNavigationService = tabNavigationService;
        //_tabNavigationService.UiScaleChangedEvent += (_, _) => { RaisePropertyChanged(nameof(UiScale)); };

        //_floridaUri = configuration.GetSection("ApiUri").Value;

        TabItems.CollectionChanged += (_, _) =>
        {
            //RaisePropertyChanged(nameof(Visibility));
        };

        _tabNavigationService.TabItems.CollectionChanged += (_, _) =>
        {

        };
    }

    public ObservableCollection<TabItem> TabItems => _tabNavigationService.TabItems;
    //public Visibility Visibility => TabItems.Any() ? Visibility.Visible : Visibility.Hidden;
    public Visibility Visibility => Visibility.Visible;

    //public double UiScale
    //{
    //    get => _tabNavigationService.UiScale;
    //    set
    //    {
    //        if (value.Equals(_tabNavigationService.UiScale)) return;
    //        _tabNavigationService.UiScale = value;
    //        //RaisePropertyChanged();
    //    }
    //}

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