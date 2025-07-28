namespace Tharga.License;

public record LicenseCheckRequest
{
    public required string ApplicationName { get; init; }
    public required Fingerprint MachineFingerprint { get; init; }
    public required Fingerprint VersionFingerprint { get; init; }
    public required string Username { get; init; }
}