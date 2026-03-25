using Tharga.License;

namespace Tharga.License.Tests;

public class LicenseCheckRequestTests
{
    [Fact]
    public void Construction_With_Required_Properties()
    {
        var machineFingerprint = new Fingerprint { Name = "Machine", Value = "M1" };
        var versionFingerprint = new Fingerprint { Name = "Version", Value = "V1" };

        var request = new LicenseCheckRequest
        {
            RequestKey = "key123",
            ApplicationName = "TestApp",
            MachineFingerprint = machineFingerprint,
            VersionFingerprint = versionFingerprint,
            Username = "testuser"
        };

        Assert.Equal("key123", request.RequestKey);
        Assert.Equal("TestApp", request.ApplicationName);
        Assert.Equal(machineFingerprint, request.MachineFingerprint);
        Assert.Equal(versionFingerprint, request.VersionFingerprint);
        Assert.Equal("testuser", request.Username);
    }

    [Fact]
    public void Record_Equality_Works()
    {
        var fp1 = new Fingerprint { Name = "Machine", Value = "M1" };
        var fp2 = new Fingerprint { Name = "Version", Value = "V1" };

        var req1 = new LicenseCheckRequest
        {
            RequestKey = "key", ApplicationName = "App",
            MachineFingerprint = fp1, VersionFingerprint = fp2, Username = "user"
        };
        var req2 = new LicenseCheckRequest
        {
            RequestKey = "key", ApplicationName = "App",
            MachineFingerprint = fp1, VersionFingerprint = fp2, Username = "user"
        };

        Assert.Equal(req1, req2);
    }
}
