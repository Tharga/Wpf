using System.Windows;
using Tharga.Wpf.TabNavigator;

namespace Tharga.Wpf.Sample;

public partial class FixedTabView
{
    public FixedTabView()
    {
        CanClose = false;
        InitializeComponent();
    }

    public override string DefaultTitle => "Fixed Tab";
    public override bool AllowMultiple => false;
    public override bool AllowClose => false;

    private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
    {
        ApplicationBase.GetService<ITabNavigationStateService>().CloseTabAsync(this, true);
    }
}