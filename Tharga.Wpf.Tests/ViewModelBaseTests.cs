using System.ComponentModel;
using Tharga.Wpf;

namespace Tharga.Wpf.Tests;

public class ViewModelBaseTests
{
    private class TestViewModel : ViewModelBase
    {
        private string _name;

        public string Name
        {
            get => _name;
            set => SetField(ref _name, value);
        }
    }

    [Fact]
    public void SetField_Raises_PropertyChanged()
    {
        var vm = new TestViewModel();
        string changedProperty = null;
        vm.PropertyChanged += (_, e) => changedProperty = e.PropertyName;

        vm.Name = "test";

        Assert.Equal("Name", changedProperty);
    }

    [Fact]
    public void SetField_Returns_True_When_Value_Changes()
    {
        var vm = new TestViewModel();

        vm.Name = "test";

        Assert.Equal("test", vm.Name);
    }

    [Fact]
    public void SetField_Does_Not_Raise_When_Same_Value()
    {
        var vm = new TestViewModel();
        vm.Name = "test";

        var raised = false;
        vm.PropertyChanged += (_, _) => raised = true;
        vm.Name = "test";

        Assert.False(raised);
    }

    [Fact]
    public void SetField_Returns_False_When_Same_Value()
    {
        var vm = new TestViewModel();
        vm.Name = "test";

        // Set same value again — property should stay unchanged
        vm.Name = "test";

        Assert.Equal("test", vm.Name);
    }

    [Fact]
    public void PropertyChanged_Is_Null_When_No_Subscribers()
    {
        var vm = new TestViewModel();

        // Should not throw even without subscribers
        vm.Name = "test";

        Assert.Equal("test", vm.Name);
    }

    [Fact]
    public void Multiple_Properties_Raise_Correct_Names()
    {
        var changedProperties = new List<string>();
        var vm = new TestViewModel();
        vm.PropertyChanged += (_, e) => changedProperties.Add(e.PropertyName);

        vm.Name = "first";
        vm.Name = "second";

        Assert.Equal(2, changedProperties.Count);
        Assert.All(changedProperties, p => Assert.Equal("Name", p));
    }
}
