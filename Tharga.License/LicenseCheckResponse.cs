namespace Tharga.License;

public record LicenseCheckResponse
{
    public required string ResponseKey { get; init; }
    public required string ApplicationName { get; init; }
    public required Fingerprint MachineFingerprint { get; init; }
    public required DateTime ApprovedAt { get; init; }
}