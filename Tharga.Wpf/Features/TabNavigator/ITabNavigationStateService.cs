using System.Collections.ObjectModel;
using System.Windows.Controls;

namespace Tharga.Wpf.TabNavigator;

public interface ITabNavigationStateService
{
    ObservableCollection<TabItem> TabItems { get; }
    void OpenTab<TTabView>(string title = default) where TTabView : TabView;
    Task<bool> CloseAllTabsAsync(bool forceClose = false);
    TabView GetActiveTabView();
}