namespace Tharga.License;

/// <summary>
/// Represents a request to check the validity of a license with the license server.
/// </summary>
public record LicenseCheckRequest
{
    /// <summary>A unique key identifying this license check request.</summary>
    public required string RequestKey { get; init; }

    /// <summary>The name of the application requesting the license check.</summary>
    public required string ApplicationName { get; init; }

    /// <summary>The fingerprint identifying the machine making the request.</summary>
    public required Fingerprint MachineFingerprint { get; init; }

    /// <summary>The fingerprint identifying the application version.</summary>
    public required Fingerprint VersionFingerprint { get; init; }

    /// <summary>The username of the user requesting the license check.</summary>
    public required string Username { get; init; }
}