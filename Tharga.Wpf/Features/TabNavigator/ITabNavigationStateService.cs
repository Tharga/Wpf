using System.Collections.ObjectModel;
using System.Windows.Controls;

namespace Tharga.Wpf.TabNavigator;

public interface ITabNavigationStateService
{
    ObservableCollection<TabItem> TabItems { get; }
    Task<(TTabView TabView, TabAction TabAction)> OpenTabAsync<TTabView>(string title = default, object parameter = default) where TTabView : TabView;
    Task<bool> CloseAllTabsAsync(bool forceClose = false);
    Task<bool> CloseTabAsync(TabView tabItem, bool forceClose = false);
    TabView GetActiveTabView();
    T GetActiveTabView<T>();
    T GetActiveTabViewModel<T>();
    TabItem GetTabByTabView<T>(Func<TabView, bool> func);
    IEnumerable<T> GetTabsByTabView<T>(Func<T, bool> func = default);
}