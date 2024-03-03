using FluentAssertions;
using Xunit;

namespace Tharga.Wpf.Tests;

public class Class1
{
    [Fact]
    public void Basic()
    {
        true.Should().BeTrue();
    }
}