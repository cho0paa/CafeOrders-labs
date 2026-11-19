using CafeOrders.Core;

namespace CafeOrders.Tests;

public class VersionInfoTests
{
    [Fact]
    public void Current_ReturnsVersionWithoutBuildMetadata()
    {
        string version = VersionInfo.Current();

        Assert.Equal("0.8.0", version);
        Assert.DoesNotContain("+", version, StringComparison.Ordinal);
    }
}
