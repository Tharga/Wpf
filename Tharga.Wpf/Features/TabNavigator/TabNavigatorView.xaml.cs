namespace Tharga.Wpf.TabNavigator;

public partial class TabNavigatorView
{
    private TabNavigatorViewModel _dataContext;

    public TabNavigatorView()
    {
        DataContextChanged += (_, _) => _dataContext = (TabNavigatorViewModel)DataContext;

        InitializeComponent();
    }
}