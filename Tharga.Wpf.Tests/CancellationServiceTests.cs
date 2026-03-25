using Tharga.Wpf.Framework;

namespace Tharga.Wpf.Tests;

public class CancellationServiceTests
{
    [Fact]
    public void Token_Is_Not_Cancelled_Initially()
    {
        var service = new CancellationService();

        Assert.False(service.Token.IsCancellationRequested);
    }

    [Fact]
    public async Task CancelAsync_Cancels_Token()
    {
        var service = new CancellationService();

        await service.CancelAsync();

        Assert.True(service.Token.IsCancellationRequested);
    }

    [Fact]
    public void Token_Can_Be_Used_With_CancellationToken()
    {
        var service = new CancellationService();

        // Token should be usable in async operations
        CancellationToken token = service.Token;
        Assert.False(token.IsCancellationRequested);
    }
}
