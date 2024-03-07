namespace Tharga.Wpf.Sample;

public partial class MainWindow
{
    public MainWindow()
    {
        var myService = App.GetService<MyService>();
        InitializeComponent();
    }
}