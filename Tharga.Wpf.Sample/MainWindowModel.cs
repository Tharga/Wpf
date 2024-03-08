using System.Windows;
using System.Windows.Input;
using Tharga.Wpf.Features.ApplicationUpdate;
using Tharga.Wpf.Framework;

namespace Tharga.Wpf.Sample;

public class MainWindowModel
{
    private readonly IApplicationUpdateStateService _applicationUpdateStateService;

    public MainWindowModel(MyService myService, IApplicationUpdateStateService applicationUpdateStateService)
    {
        _applicationUpdateStateService = applicationUpdateStateService;
    }

    public ICommand ShowSplashCommand => new RelayCommand(_ => { _applicationUpdateStateService.ShowSplash(); }, _ => true);
    public ICommand ThrowExceptionCommand => new RelayCommand(_ => throw new InvalidOperationException("Some error"), _ => true);
    public ICommand ExitCommand => new RelayCommand(_ => { Application.Current?.MainWindow?.Close(); }, _ => true);
}