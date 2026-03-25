using Tharga.Wpf.ExceptionHandling;

namespace Tharga.Wpf.Tests.Features.ExceptionHandling;

public class HandleExceptionEventArgsTests
{
    [Fact]
    public void Constructor_Sets_Exception()
    {
        var exception = new InvalidOperationException("test error");

        var args = new HandleExceptionEventArgs(exception);

        Assert.Equal(exception, args.Exception);
    }

    [Fact]
    public void Exception_Type_Is_Preserved()
    {
        var exception = new ArgumentNullException("param");

        var args = new HandleExceptionEventArgs(exception);

        Assert.IsType<ArgumentNullException>(args.Exception);
        Assert.Equal("param", ((ArgumentNullException)args.Exception).ParamName);
    }
}
