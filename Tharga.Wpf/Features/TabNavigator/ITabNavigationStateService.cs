using System.Collections.ObjectModel;
using System.Windows.Controls;

namespace Tharga.Wpf.TabNavigator;

/// <summary>
/// Manages tab navigation state, including opening, closing, and querying tabs.
/// </summary>
public interface ITabNavigationStateService
{
    /// <summary>Gets the collection of tab items currently displayed.</summary>
    ObservableCollection<TabItem> TabItems { get; }

    /// <summary>
    /// Opens a tab of the specified type, optionally with a title and parameter.
    /// </summary>
    /// <typeparam name="TTabView">The type of tab view to open.</typeparam>
    /// <param name="title">An optional title for the tab.</param>
    /// <param name="parameter">An optional parameter passed to the tab's <see cref="TabView.LoadActionAsync"/>.</param>
    /// <returns>The opened tab view and whether it was newly created or focused.</returns>
    Task<(TTabView TabView, TabAction TabAction)> OpenTabAsync<TTabView>(string title = default, object parameter = default) where TTabView : TabView;

    /// <summary>Closes all open tabs.</summary>
    /// <param name="forceClose">If <c>true</c>, closes tabs even if they report they cannot close.</param>
    /// <returns><c>true</c> if all tabs were closed; otherwise, <c>false</c>.</returns>
    Task<bool> CloseAllTabsAsync(bool forceClose = false);

    /// <summary>Closes the specified tab.</summary>
    /// <param name="tabItem">The tab view to close.</param>
    /// <param name="forceClose">If <c>true</c>, closes the tab even if it reports it cannot close.</param>
    /// <returns><c>true</c> if the tab was closed; otherwise, <c>false</c>.</returns>
    Task<bool> CloseTabAsync(TabView tabItem, bool forceClose = false);

    /// <summary>Gets the currently active tab view.</summary>
    /// <returns>The active <see cref="TabView"/>, or <c>null</c> if no tab is active.</returns>
    TabView GetActiveTabView();

    /// <summary>Gets the currently active tab view cast to the specified type.</summary>
    /// <typeparam name="T">The expected type of the active tab view.</typeparam>
    T GetActiveTabView<T>();

    /// <summary>Gets the view model of the currently active tab.</summary>
    /// <typeparam name="T">The expected type of the view model.</typeparam>
    T GetActiveTabViewModel<T>();

    /// <summary>Finds a tab item by matching its tab view with the specified predicate.</summary>
    /// <typeparam name="T">The type of tab view to search for.</typeparam>
    /// <param name="func">A predicate to match the tab view.</param>
    TabItem GetTabByTabView<T>(Func<TabView, bool> func);

    /// <summary>Gets all tab views of the specified type, optionally filtered by a predicate.</summary>
    /// <typeparam name="T">The type of tab views to return.</typeparam>
    /// <param name="func">An optional predicate to filter results.</param>
    IEnumerable<T> GetTabsByTabView<T>(Func<T, bool> func = default);
}