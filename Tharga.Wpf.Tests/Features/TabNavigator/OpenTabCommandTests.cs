using Moq;
using Tharga.Wpf.TabNavigator;

namespace Tharga.Wpf.Tests.Features.TabNavigator;

public class OpenTabCommandTests
{
    [Fact]
    public void CanExecute_Returns_True_By_Default()
    {
        var mockService = new Mock<ITabNavigationStateService>();
        var command = new OpenTabComamnd<TestTabView>(mockService.Object);

        Assert.True(command.CanExecute(null));
    }

    [Fact]
    public void CanExecute_Uses_Provided_Predicate()
    {
        var mockService = new Mock<ITabNavigationStateService>();
        var command = new OpenTabComamnd<TestTabView>(mockService.Object, canExecute: () => false);

        Assert.False(command.CanExecute(null));
    }

    [Fact]
    public void CanExecute_Predicate_Returns_True()
    {
        var mockService = new Mock<ITabNavigationStateService>();
        var command = new OpenTabComamnd<TestTabView>(mockService.Object, canExecute: () => true);

        Assert.True(command.CanExecute(null));
    }
}

public class TabActionTests
{
    [Theory]
    [InlineData(TabAction.Created, 0)]
    [InlineData(TabAction.Focused, 1)]
    public void TabAction_Has_Expected_Values(TabAction action, int expected)
    {
        Assert.Equal(expected, (int)action);
    }

    [Fact]
    public void TabAction_Has_Two_Members()
    {
        Assert.Equal(2, Enum.GetValues<TabAction>().Length);
    }
}

public class EDocumentPresetTests
{
    [Fact]
    public void EDocumentPreset_Has_Four_Members()
    {
        Assert.Equal(4, Enum.GetValues<EDocumentPreset>().Length);
    }
}

public class TestTabView : TabView
{
}
