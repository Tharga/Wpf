using System.Windows;
using System.Windows.Markup;

namespace Tharga.Wpf;

public class ViewModelProvider : MarkupExtension
{
    public ViewModelProvider(Type viewModelType)
    {
        ViewModelType = viewModelType;
    }

    public Type ViewModelType { get; set; }

    public override object ProvideValue(IServiceProvider serviceProvider)
    {
        var viewModel = ((ApplicationBase)Application.Current).AppHost.Services.GetService(ViewModelType);
        if (viewModel == null) throw new InvalidOperationException($"Cannot find view model of type '{ViewModelType?.GetType().Name}'. Prehaps it is not registered in the IOC.");
        return viewModel;
    }
}