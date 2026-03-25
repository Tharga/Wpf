using Tharga.Wpf.Dialog;

namespace Tharga.Wpf.Tests.Features.Dialog;

public class DialogViewModelBaseTests
{
    private class TestDialogViewModel : DialogViewModelBase
    {
        public void InvokeRequestClose(bool result) => RequestClose(result);
    }

    [Fact]
    public void OkCommand_Is_Not_Null()
    {
        var vm = new TestDialogViewModel();

        Assert.NotNull(vm.OkCommand);
    }

    [Fact]
    public void CancelCommand_Is_Not_Null()
    {
        var vm = new TestDialogViewModel();

        Assert.NotNull(vm.CancelCommand);
    }

    [Fact]
    public void OkCommand_CanExecute_Returns_True()
    {
        var vm = new TestDialogViewModel();

        Assert.True(vm.OkCommand.CanExecute(null));
    }

    [Fact]
    public void CancelCommand_CanExecute_Returns_True()
    {
        var vm = new TestDialogViewModel();

        Assert.True(vm.CancelCommand.CanExecute(null));
    }

    [Fact]
    public void OkCommand_Execute_Raises_RequestCloseEvent_With_True()
    {
        var vm = new TestDialogViewModel();
        bool? dialogResult = null;
        vm.RequestCloseEvent += (_, e) => dialogResult = e.DialogResult;

        vm.OkCommand.Execute(null);

        Assert.True(dialogResult);
    }

    [Fact]
    public void CancelCommand_Execute_Raises_RequestCloseEvent_With_False()
    {
        var vm = new TestDialogViewModel();
        bool? dialogResult = null;
        vm.RequestCloseEvent += (_, e) => dialogResult = e.DialogResult;

        vm.CancelCommand.Execute(null);

        Assert.False(dialogResult);
    }

    [Fact]
    public void RequestClose_Without_Subscribers_Does_Not_Throw()
    {
        var vm = new TestDialogViewModel();

        vm.InvokeRequestClose(true);
    }
}
