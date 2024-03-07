using System.Windows.Input;
using Tharga.Wpf.Framework;

namespace Tharga.Wpf.Sample;

public class MainWindowModel
{
    public MainWindowModel(MyService myService)
    {
    }

    public ICommand ThrowExceptionCommand => new RelayCommand(_ => throw new InvalidOperationException("Some error"), _ => true);
}