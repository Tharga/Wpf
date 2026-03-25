namespace Tharga.License;

/// <summary>
/// Provides RSA-based signing and verification operations for license validation.
/// </summary>
public interface ISigningService
{
    /// <summary>
    /// Generates a new RSA 2048-bit key pair for signing and verification.
    /// </summary>
    /// <returns>An <see cref="RsaKeyPair"/> containing the public and private keys.</returns>
    RsaKeyPair BuildKeyPair();

    /// <summary>
    /// Signs the specified data using the given private key.
    /// </summary>
    /// <param name="data">The data to sign.</param>
    /// <param name="privateRsaKey">The RSA private key used for signing.</param>
    /// <returns>A base64-encoded signature string.</returns>
    string Sign(string data, RsaPrivateKey privateRsaKey);

    /// <summary>
    /// Verifies that a signature is valid for the given data and public key.
    /// </summary>
    /// <param name="data">The original data that was signed.</param>
    /// <param name="base64Signature">The base64-encoded signature to verify.</param>
    /// <param name="rsaPublicKey">The RSA public key to verify against.</param>
    /// <returns><c>true</c> if the signature is valid; otherwise, <c>false</c>.</returns>
    bool VerifySignature(string data, string base64Signature, RsaPublicKey rsaPublicKey);
}