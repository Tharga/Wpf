﻿using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Microsoft.Extensions.DependencyInjection;
using Tharga.Toolkit.TypeService;

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

    public async Task<(TTabView TabView, TabAction TabAction)> OpenTabAsync<TTabView>(string title, object parameter)
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

    public async Task<bool> CloseTabAsync(TabView tabView, bool forceClose)
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