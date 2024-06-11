using System.Diagnostics;
using System.Windows;

namespace Tharga.Wpf.Dialog;

public class DialogBase : Window
{
    protected DialogBase()
    {
        DataContextChanged += DialogBase_DataContextChanged;
        SourceInitialized += (_, _) => { this.HideMinimizeAndMaximizeButtons(); };

        ShowInTaskbar = false;
        ResizeMode = ResizeMode.CanResizeWithGrip;
        WindowStartupLocation = WindowStartupLocation.CenterScreen;
    }

    private void DialogBase_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
    {
        if (DataContext is DialogViewModelBase viewModel)
        {
            viewModel.RequestCloseEvent += (_, ev) =>
            {
                DialogResult = ev.DialogResult;
                Close();
            };
        }
    }
}