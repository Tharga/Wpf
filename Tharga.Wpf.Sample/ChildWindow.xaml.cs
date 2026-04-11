using System.ComponentModel;
using Tharga.Wpf.WindowLocation;

namespace Tharga.Wpf.Sample;

public partial class ChildWindow
{
    public ChildWindow()
    {
        this.RegisterWindow();

        InitializeComponent();
    }

    protected override void OnClosing(CancelEventArgs e)
    {
        if (ApplicationBase.CloseMode != CloseMode.Force && BlockClosingCheckBox.IsChecked == true)
        {
            e.Cancel = true;
            return;
        }

        base.OnClosing(e);
    }
}
