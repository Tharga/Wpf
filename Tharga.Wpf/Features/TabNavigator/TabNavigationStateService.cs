using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Microsoft.Extensions.DependencyInjection;
using Tharga.Runtime;

namespace Tharga.Wpf.TabNavigator;

internal class TabNavigationStateService : ITabNavigationStateService
{
    private readonly ThargaWpfOptions _options;
    private readonly IServiceProvider _serviceProvider;
    private TabView _activeTab;

    public TabNavigationStateService(ThargaWpfOptions options, IServiceProvider serviceProvider)
    {
        _options = options;
        _serviceProvider = serviceProvider;
    }

    public ObservableCollection<TabItem> TabItems { get; } = new ();

    public Task<(TTabView TabView, TabAction TabAction)> OpenTabAsync<TTabView>(string title, object parameter)
        where TTabView : TabView
    {
        // TabItems and TabItem.Content are UI-thread-only (ContentControl). Dispatch so the
        // method is safe to call from any thread (e.g. from OpenTabComamnd<T>.Execute whose
        // async void body may resume on a thread-pool thread).
        var dispatcher = Application.Current?.Dispatcher;
        if (dispatcher != null && !dispatcher.CheckAccess())
        {
            // Start the async method on the dispatcher thread; its continuations will resume
            // on the dispatcher's SynchronizationContext. Caller awaits the returned Task safely.
            return dispatcher.Invoke(() => OpenTabCoreAsync<TTabView>(title, parameter));
        }
        return OpenTabCoreAsync<TTabView>(title, parameter);
    }

    private async Task<(TTabView TabView, TabAction TabAction)> OpenTabCoreAsync<TTabView>(string title, object parameter)
        where TTabView : TabView
    {
        var existing = TabItems.Where(x => x.Content.GetType() == typeof(TTabView)).ToArray();
        if (existing.Any())
        {
            foreach (var item in existing)
            {
                var existingTabContent = (TTabView)item.Content;

                //NOTE: This setting is per type.
                if (!existingTabContent.AllowMultiple)
                {
                    item.Focus();
                    await ExecuteActionAsync(parameter, existingTabContent);
                    return (existingTabContent, TabAction.Focused);
                }

                if (!_options.AllowTabsWithSameTitles && existingTabContent.Title == title)
                {
                    item.Focus();
                    await ExecuteActionAsync(parameter, existingTabContent);
                    return (existingTabContent, TabAction.Focused);
                }
            }
        }

        var tabContent = _serviceProvider.GetService<TTabView>();
        if (tabContent == null) throw new NullReferenceException($"Cannot create tab view of type {typeof(TTabView).Name}");
        tabContent.Title = title;

        var tabItem = new TabItem
        {
            Header = GetHeader(tabContent),
            Content = tabContent
        };

        var tabView = (TTabView)tabItem.Content;
        tabView.GotFocus += (_, _) =>
        {
            _activeTab = tabView;
            CommandManager.InvalidateRequerySuggested();
        };

        tabItem.GotFocus += (_, _) =>
        {
            _activeTab = tabView;
            CommandManager.InvalidateRequerySuggested();
        };

        TabItems.Add(tabItem);
        tabItem.Focus();
        await ExecuteActionAsync(parameter, tabContent);
        return (tabView, TabAction.Created);
    }

    private async Task ExecuteActionAsync<TTabView>(object parameter, TTabView tabContent) where TTabView : TabView
    {
        try
        {
            await tabContent.LoadActionAsync(parameter);
        }
        catch (Exception e)
        {
            Debugger.Break();
            Trace.TraceError($"{e.Message} @{e.StackTrace}");
            //var exceptionHandlerService = _serviceProvider.GetService<IExceptionHandlerService>();
            //await Application.Current.Dispatcher.Invoke(async () =>
            //{
            //    await exceptionHandlerService.HandleExceptionAsync(e, Application.Current.MainWindow);
            //});
            throw;
        }
    }

    public Task<bool> CloseAllTabsAsync(bool forceClose)
    {
        var dispatcher = Application.Current?.Dispatcher;
        if (dispatcher != null && !dispatcher.CheckAccess())
        {
            return dispatcher.Invoke(() => CloseAllTabsCoreAsync(forceClose));
        }
        return CloseAllTabsCoreAsync(forceClose);
    }

    private async Task<bool> CloseAllTabsCoreAsync(bool forceClose)
    {
        var allowClose = true;

        foreach (var tabItem in TabItems.ToArray())
        {
            var canClose = await CloseTabAsync(tabItem.Content as TabView, forceClose);
            if (!canClose) allowClose = false;
        }

        return allowClose;
    }

    public TabView GetActiveTabView()
    {
        return _activeTab;
    }

    public T GetActiveTabView<T>()
    {
        if (_activeTab is T t)
        {
            return t;
        }
        else if (_activeTab?.GetType().IsOfType<T>() ?? false)
        {
            Debugger.Break();
        }

        return default;
    }

    public T GetActiveTabViewModel<T>()
    {
        if (_activeTab?.DataContext is T t)
        {
            return t;
        }

        return default;
    }

    public TabItem GetTabByTabView<T>(Func<TabView, bool> func)
    {
        var ti = TabItems.FirstOrDefault(x => func((x.Content as TabView)));
        return ti;
    }

    public IEnumerable<T> GetTabsByTabView<T>(Func<T, bool> func)
    {
        foreach (var tabItem in TabItems)
        {
            if (tabItem?.Content is T t)
            {
                if (func == null || func(t))
                {
                    yield return t;
                }
            }
        }
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

    public Task<bool> CloseTabAsync(TabView tabView, bool forceClose)
    {
        var dispatcher = Application.Current?.Dispatcher;
        if (dispatcher != null && !dispatcher.CheckAccess())
        {
            return dispatcher.Invoke(() => CloseTabCoreAsync(tabView, forceClose));
        }
        return CloseTabCoreAsync(tabView, forceClose);
    }

    private async Task<bool> CloseTabCoreAsync(TabView tabView, bool forceClose)
    {
        var closeResult = await tabView.OnCloseAsync();
        if (!forceClose && !closeResult)
        {
            MessageBox.Show($"Cannot close tab '{tabView.Title}'.", "Close aborted", MessageBoxButton.OK, MessageBoxImage.Warning);
            return false;
        }

        var tab = TabItems.FirstOrDefault(x => Equals(x.Content, tabView));
        TabItems.Remove(tab);
        _activeTab = TabItems.FirstOrDefault(x => x.IsSelected)?.Content as TabView;
        return true;
    }
}