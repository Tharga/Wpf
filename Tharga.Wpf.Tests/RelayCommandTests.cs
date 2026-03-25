using Tharga.Wpf;

namespace Tharga.Wpf.Tests;

public class RelayCommandTests
{
    [Fact]
    public void Execute_Invokes_Action()
    {
        object receivedParam = null;
        var command = new RelayCommand(p => receivedParam = p);

        command.Execute("test");

        Assert.Equal("test", receivedParam);
    }

    [Fact]
    public void Execute_With_Null_Parameter()
    {
        var executed = false;
        var command = new RelayCommand(_ => executed = true);

        command.Execute(null);

        Assert.True(executed);
    }

    [Fact]
    public void CanExecute_Returns_True_By_Default()
    {
        var command = new RelayCommand(_ => { });

        Assert.True(command.CanExecute(null));
    }

    [Fact]
    public void CanExecute_Uses_Provided_Predicate()
    {
        var command = new RelayCommand(_ => { }, p => p is string s && s == "yes");

        Assert.True(command.CanExecute("yes"));
        Assert.False(command.CanExecute("no"));
        Assert.False(command.CanExecute(null));
    }

    [Fact]
    public void CanExecute_Returns_True_For_Any_Parameter_When_No_Predicate()
    {
        var command = new RelayCommand(_ => { });

        Assert.True(command.CanExecute("anything"));
        Assert.True(command.CanExecute(42));
        Assert.True(command.CanExecute(null));
    }
}
