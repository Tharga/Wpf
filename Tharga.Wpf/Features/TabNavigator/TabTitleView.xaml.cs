using System.Windows;

namespace Tharga.Wpf.Features.TabNavigator;

public partial class TabTitleView
{
    private readonly TabView _tabView;
    private readonly Func<TabView, Task> _close;

    public TabTitleView(TabView tabView, Func<TabView, Task> close)
    {
        _tabView = tabView;
        _close = close;
        InitializeComponent();

        Title.Content = tabView.Title;
        CloseButton.Visibility = tabView.CanClose ? Visibility.Visible : Visibility.Collapsed;
    }

    private async void CloseButton_Click(object sender, RoutedEventArgs e)
    {
        await _close.Invoke(_tabView);
    }
}