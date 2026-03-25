namespace Tharga.License;

/// <summary>
/// Represents an RSA key pair containing both the public and private keys.
/// </summary>
public record RsaKeyPair
{
    /// <summary>
    /// The RSA public key used for signature verification.
    /// </summary>
    public required RsaPublicKey Public { get; init; }

    /// <summary>
    /// The RSA private key used for signing.
    /// </summary>
    public required RsaPrivateKey Private { get; init; }
}