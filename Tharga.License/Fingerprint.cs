namespace Tharga.License;

/// <summary>
/// Represents a named fingerprint value used for machine or version identification during license checks.
/// </summary>
public record Fingerprint
{
    /// <summary>The name of the fingerprint (e.g. "Machine" or "Version").</summary>
    public required string Name { get; init; }

    /// <summary>The fingerprint value.</summary>
    public required string Value { get; init; }
}