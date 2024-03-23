using System.Windows;

namespace Tharga.Wpf.TabNavigator;

public partial class TabTitleView
{
    private readonly TabView _tabView;
    private readonly Func<TabView, Task> _close;

    public TabTitleView(TabView tabView, Func<TabView, Task> close)
    {
        tabView.CanCloseChangedEvent += (s, e) =>
        {
            if (CloseButton != null) Application.Current.Dispatcher.Invoke(() => CloseButton.IsEnabled = tabView.CanClose );
        };

        tabView.TitleChangedEvent += (_, _) =>
        {
            Application.Current.Dispatcher.Invoke(() => Title.Content = tabView.Title);
        };

        _tabView = tabView;
        _close = close;
        InitializeComponent();

        Title.Content = tabView.Title;
        CloseButton.Visibility = tabView.AllowClose ? Visibility.Visible : Visibility.Collapsed;
        CloseButton.IsEnabled = tabView.CanClose;
    }

    private async void CloseButton_Click(object sender, RoutedEventArgs e)
    {
        await _close.Invoke(_tabView);
    }
}