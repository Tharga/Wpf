using System.Windows;
using System.Windows.Markup;

namespace Tharga.Wpf;

public class ViewModelProvider : MarkupExtension
{
    public ViewModelProvider(Type viewModelType)
    {
        ViewModelType = viewModelType;
    }

    private Type ViewModelType { get; set; }

    public override object ProvideValue(IServiceProvider serviceProvider)
    {
        var applicationBase = (Application.Current as ApplicationBase);
        var viewModel = applicationBase?.AppHost.Services.GetService(ViewModelType);
        if (viewModel == null) throw new TypeNotRegisteredException(ViewModelType, applicationBase == null ? $" Application.Current is '{Application.Current.GetType().FullName}', it needs to be of type {nameof(ApplicationBase)}." : null);
        return viewModel;
    }
}