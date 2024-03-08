using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Microsoft.Extensions.DependencyInjection;

namespace Tharga.Wpf.Features.TabNavigator;

public class TabNavigationStateService : ITabNavigationStateService
{
    private readonly IServiceProvider _serviceProvider;
    private double _uiScale = TabNavigatorViewModel.DefaultUiScale;
    private TabView _activeTab;

    public TabNavigationStateService(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    //public event EventHandler<UiScaleChangedEventArgs> UiScaleChangedEvent;

    //public double UiScale
    //{
    //    get => _uiScale;
    //    set
    //    {
    //        if (Math.Abs(_uiScale - value) < 0.01) return;
    //        _uiScale = value;
    //        UiScaleChangedEvent?.Invoke(this, new UiScaleChangedEventArgs());
    //    }
    //}

    public ObservableCollection<TabItem> TabItems { get; } = new ();

    public void OpenTab<TTabView>()
        where TTabView : TabView
    {
        var existing = TabItems.FirstOrDefault(x => x.Content.GetType() == typeof(TTabView));
        if (existing != null)
        {
            var existingTabContent = (TTabView)existing.Content;
            if (!existingTabContent.AllowMultiple)
            {
                existing.Focus();
                return;
            }
        }

        var tabContent = _serviceProvider.GetService<TTabView>();

        var tabItem = new TabItem
        {
            Header = GetHeader(tabContent),
            Content = tabContent
        };

        var ctx = (TTabView)tabItem.Content;
        ctx.GotFocus += (_, _) =>
        {
            _activeTab = ctx;
            CommandManager.InvalidateRequerySuggested();
        };

        tabItem.GotFocus += (s, e) =>
        {
            _activeTab = ctx;
            CommandManager.InvalidateRequerySuggested();
        };

        TabItems.Add(tabItem);
        tabItem.Focus();
    }

    public async Task<bool> CloseAllTabsAsync(bool forceClose)
    {
        var allowClose = true;

        foreach (var tabItem in TabItems.ToArray())
        {
            var canClose = await CloseTabAsync(tabItem.Content as TabView, forceClose);
            if (!canClose) allowClose = false;
        }

        return allowClose;
    }

    public TabView GetActiveTab()
    {
        return _activeTab;
    }

    private object GetHeader<TTabView>(TTabView tabContent)
        where TTabView : TabView
    {
        return new TabTitleView(tabContent, CloseTabAsync);
    }

    private Task<bool> CloseTabAsync(TabView tabView)
    {
        return CloseTabAsync(tabView, false);
    }

    private async Task<bool> CloseTabAsync(TabView tabView, bool forceClose)
    {
        var result = await tabView.OnCloseAsync();
        if (!result && !forceClose)
        {
            MessageBox.Show($"Kan inte stänga '{tabView.Title}'. Åtgärda och försök igen.", "Stängning avbrutning", MessageBoxButton.OK, MessageBoxImage.Warning);
            return false;
        }

        var tab = TabItems.FirstOrDefault(x => Equals(x.Content, tabView));
        TabItems.Remove(tab);
        return true;
    }
}