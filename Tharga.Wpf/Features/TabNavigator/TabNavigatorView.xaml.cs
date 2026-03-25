namespace Tharga.Wpf.TabNavigator;

/// <summary>
/// A container view that holds and displays tab view items managed by <see cref="ITabNavigationStateService"/>.
/// </summary>
public partial class TabNavigatorView
{
    private TabNavigatorViewModel _dataContext;

    /// <summary>
    /// Initializes a new instance of the <see cref="TabNavigatorView"/> class.
    /// </summary>
    public TabNavigatorView()
    {
        DataContextChanged += (_, _) => _dataContext = (TabNavigatorViewModel)DataContext;

        InitializeComponent();
    }
}