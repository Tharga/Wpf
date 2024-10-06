using System.Collections.ObjectModel;
using System.Windows.Controls;

namespace Tharga.Wpf.TabNavigator;

public interface ITabNavigationStateService
{
    ObservableCollection<TabItem> TabItems { get; }
    (TTabView TabView, TabAction TabAction) OpenTab<TTabView>(string title = default) where TTabView : TabView;
    Task<bool> CloseAllTabsAsync(bool forceClose = false);
    Task<bool> CloseTabAsync(TabView tabItem, bool forceClose = false);
    TabView GetActiveTabView();
    T GetActiveTabView<T>();
    T GetActiveTabViewModel<T>();
    TabItem GetTabByTabView<T>(Func<TabView, bool> func);
}