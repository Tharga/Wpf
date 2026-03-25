using System.Security.Cryptography;

namespace Tharga.License;

/// <summary>
/// Represents an RSA public key with base64-encoded components.
/// </summary>
public record RsaPublicKey
{
    /// <summary>
    /// The base64-encoded RSA modulus.
    /// </summary>
    public string Modulus { get; init; }

    /// <summary>
    /// The base64-encoded RSA exponent.
    /// </summary>
    public string Exponent { get; init; }

    /// <summary>
    /// Converts this key to <see cref="RSAParameters"/> for use with .NET cryptography APIs.
    /// </summary>
    /// <returns>The RSA parameters.</returns>
    public RSAParameters ToParameters()
    {
        return new RSAParameters
        {
            Modulus = Convert.FromBase64String(Modulus),
            Exponent = Convert.FromBase64String(Exponent)
        };
    }

    /// <summary>
    /// Creates an <see cref="RsaPublicKey"/> from the specified <see cref="RSAParameters"/>.
    /// </summary>
    /// <param name="parameters">The RSA parameters containing the public key components.</param>
    /// <returns>A new <see cref="RsaPublicKey"/> instance.</returns>
    public static RsaPublicKey FromParameters(RSAParameters parameters)
    {
        return new RsaPublicKey
        {
            Modulus = Convert.ToBase64String(parameters.Modulus ?? ReadOnlySpan<byte>.Empty),
            Exponent = Convert.ToBase64String(parameters.Exponent ?? ReadOnlySpan<byte>.Empty)
        };
    }
}