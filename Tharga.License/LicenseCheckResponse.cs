namespace Tharga.License;

/// <summary>
/// Represents the response from a license server after a license check.
/// </summary>
public record LicenseCheckResponse
{
    /// <summary>A unique key identifying this license check response.</summary>
    public required string ResponseKey { get; init; }

    /// <summary>The name of the application the license was checked for.</summary>
    public required string ApplicationName { get; init; }

    /// <summary>The fingerprint identifying the machine the license was approved for.</summary>
    public required Fingerprint MachineFingerprint { get; init; }

    /// <summary>The date and time when the license was approved.</summary>
    public required DateTime ApprovedAt { get; init; }
}