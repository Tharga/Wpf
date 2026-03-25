using Tharga.Wpf.IconTray;

namespace Tharga.Wpf.Tests.Features.IconTray;

public class IconTrayDataTests
{
    [Fact]
    public void Default_Properties_Are_Null()
    {
        var data = new IconTrayData();

        Assert.Null(data.Icon);
        Assert.Null(data.Menu);
    }

    [Fact]
    public void Can_Set_Menu()
    {
        var menuItems = new[]
        {
            new TrayMenuItem { Text = "Open" },
            new TrayMenuItem { Text = "Exit" }
        };

        var data = new IconTrayData { Menu = menuItems };

        Assert.Equal(2, data.Menu.Length);
        Assert.Equal("Open", data.Menu[0].Text);
        Assert.Equal("Exit", data.Menu[1].Text);
    }
}

public class TrayMenuItemTests
{
    [Fact]
    public void Default_Properties_Are_Null()
    {
        var item = new TrayMenuItem();

        Assert.Null(item.Text);
        Assert.Null(item.Image);
        Assert.Null(item.Action);
    }

    [Fact]
    public void Can_Set_Text()
    {
        var item = new TrayMenuItem { Text = "Settings" };

        Assert.Equal("Settings", item.Text);
    }

    [Fact]
    public void Can_Set_Action()
    {
        var invoked = false;
        var item = new TrayMenuItem { Action = (_, _) => invoked = true };

        item.Action(this, EventArgs.Empty);

        Assert.True(invoked);
    }
}
