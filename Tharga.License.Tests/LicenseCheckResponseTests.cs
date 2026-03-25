using Tharga.License;

namespace Tharga.License.Tests;

public class LicenseCheckResponseTests
{
    [Fact]
    public void Construction_With_Required_Properties()
    {
        var fingerprint = new Fingerprint { Name = "Machine", Value = "M1" };
        var approvedAt = new DateTime(2025, 1, 15, 10, 30, 0, DateTimeKind.Utc);

        var response = new LicenseCheckResponse
        {
            ResponseKey = "resp123",
            ApplicationName = "TestApp",
            MachineFingerprint = fingerprint,
            ApprovedAt = approvedAt
        };

        Assert.Equal("resp123", response.ResponseKey);
        Assert.Equal("TestApp", response.ApplicationName);
        Assert.Equal(fingerprint, response.MachineFingerprint);
        Assert.Equal(approvedAt, response.ApprovedAt);
    }

    [Fact]
    public void Record_Equality_Works()
    {
        var fp = new Fingerprint { Name = "Machine", Value = "M1" };
        var date = new DateTime(2025, 1, 15, 10, 30, 0, DateTimeKind.Utc);

        var resp1 = new LicenseCheckResponse
        {
            ResponseKey = "key", ApplicationName = "App",
            MachineFingerprint = fp, ApprovedAt = date
        };
        var resp2 = new LicenseCheckResponse
        {
            ResponseKey = "key", ApplicationName = "App",
            MachineFingerprint = fp, ApprovedAt = date
        };

        Assert.Equal(resp1, resp2);
    }
}
