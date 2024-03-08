using System.Collections.ObjectModel;
using System.Windows.Controls;

namespace Tharga.Wpf.Features.TabNavigator;

public interface ITabNavigationStateService
{
    //event EventHandler<UiScaleChangedEventArgs> UiScaleChangedEvent;
    //double UiScale { get; set; }
    ObservableCollection<TabItem> TabItems { get; }
    void OpenTab<TTabView>() where TTabView : TabView;
    Task<bool> CloseAllTabsAsync(bool forceClose = false);
    TabView GetActiveTab();
}