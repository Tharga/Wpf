using System.Diagnostics;
using System.Windows;

namespace Tharga.Wpf.Dialog;

/// <summary>
/// Base class for dialog windows that integrates with <see cref="DialogViewModelBase"/> for automatic close handling.
/// </summary>
public class DialogBase : Window
{
    /// <summary>
    /// Initializes a new instance of the <see cref="DialogBase"/> class with default dialog settings.
    /// </summary>
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