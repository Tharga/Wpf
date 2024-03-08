using System.Windows.Input;
using Tharga.Wpf.Features.ApplicationUpdate;
using Tharga.Wpf.Framework;

namespace Tharga.Wpf.Sample;

public class MainWindowModel
{
    public MainWindowModel(MyService myService)
    {
    }

    public ICommand ShowSplashCommand => new RelayCommand(_ =>
    {
        //Use a specific "ShowSplash" function that populates this automatically
        var splash = new Splash(new SplashData
        {
            FullName = "A",
            EntryMessage = "yeee",
            EnvironmentName = "Something",
            FirstRun = false,
            Version = "1.2.3.4"
        });
        splash.Show();
    }, _ => true);
    public ICommand ThrowExceptionCommand => new RelayCommand(_ => throw new InvalidOperationException("Some error"), _ => true);
}