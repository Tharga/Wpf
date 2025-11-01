using System.Diagnostics;
using System.Windows;
using System.Windows.Markup;
using Tharga.Runtime;

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
        var viewModel = GetInstance(applicationBase);
        if (viewModel == null) throw new TypeNotRegisteredException(ViewModelType, applicationBase == null ? $" Application.Current is '{Application.Current.GetType().FullName}', it needs to be of type {nameof(ApplicationBase)}." : null);
        return viewModel;
    }

    private object GetInstance(ApplicationBase applicationBase)
    {
        var item = applicationBase?.AppHost.Services.GetService(ViewModelType);
        if (item == null)
        {
            var serviceType = AssemblyService.GetTypes(x => x == ViewModelType).Single().AsType();
            item = applicationBase?.AppHost.Services.GetService(serviceType);
            if (item == null)
            {
                Debugger.Break();
                throw new NullReferenceException($"Cannot create instane of '{serviceType.Name}'.");
            }
        }

        return item;
    }
}