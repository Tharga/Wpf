using Microsoft.Extensions.Configuration;
using System.ComponentModel;
using System.Windows;
using Tharga.Wpf.WindowLocation;

namespace Tharga.Wpf.Sample;

public partial class MainWindow
{
    private MainWindowViewModel _dataContext;

    public MainWindow()
    {
        var windowLocationService = ApplicationBase.GetService<IWindowLocationService>();
        var environmentName = ApplicationBase.GetService<IConfiguration>().GetSection("Environment").Value;

        windowLocationService.Monitor(this, nameof(MainWindow), environmentName);

        DataContextChanged += (_, _) => OnDataContextChanged();

        InitializeComponent();
    }

    private async void OnDataContextChanged()
    {
        _dataContext = (MainWindowViewModel)DataContext;
    }

    protected override async void OnClosing(CancelEventArgs e)
    {
        try
        {
            await this.HandleCloseAsync(e, base.OnClosing);
        }
        catch (Exception ex)
        {
            StaticExceptionHandler.Handle(ex, this);
        }
    }

    private void MainWindow_OnLoaded(object sender, RoutedEventArgs e)
    {
        _dataContext.Startup();
    }
}