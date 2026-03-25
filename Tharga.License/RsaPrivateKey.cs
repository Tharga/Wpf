using System.Security.Cryptography;

namespace Tharga.License;

/// <summary>
/// Represents an RSA private key with base64-encoded components.
/// </summary>
public record RsaPrivateKey
{
    /// <summary>The base64-encoded RSA modulus.</summary>
    public required string Modulus { get; init; }

    /// <summary>The base64-encoded RSA exponent.</summary>
    public required string Exponent { get; init; }

    /// <summary>The base64-encoded RSA private exponent (D).</summary>
    public required string D { get; init; }

    /// <summary>The base64-encoded RSA prime factor (P).</summary>
    public required string P { get; init; }

    /// <summary>The base64-encoded RSA prime factor (Q).</summary>
    public required string Q { get; init; }

    /// <summary>The base64-encoded RSA CRT exponent (DP = D mod P-1).</summary>
    public required string DP { get; init; }

    /// <summary>The base64-encoded RSA CRT exponent (DQ = D mod Q-1).</summary>
    public required string DQ { get; init; }

    /// <summary>The base64-encoded RSA CRT coefficient (InverseQ = Q^-1 mod P).</summary>
    public required string InverseQ { get; init; }

    /// <summary>
    /// Converts this key to <see cref="RSAParameters"/> for use with .NET cryptography APIs.
    /// </summary>
    /// <returns>The RSA parameters.</returns>
    public RSAParameters ToParameters()
    {
        return new RSAParameters
        {
            Modulus = Convert.FromBase64String(Modulus),
            Exponent = Convert.FromBase64String(Exponent),
            D = Convert.FromBase64String(D),
            P = Convert.FromBase64String(P),
            Q = Convert.FromBase64String(Q),
            DP = Convert.FromBase64String(DP),
            DQ = Convert.FromBase64String(DQ),
            InverseQ = Convert.FromBase64String(InverseQ)
        };
    }

    /// <summary>
    /// Creates an <see cref="RsaPrivateKey"/> from the specified <see cref="RSAParameters"/>.
    /// </summary>
    /// <param name="parameters">The RSA parameters containing the private key components.</param>
    /// <returns>A new <see cref="RsaPrivateKey"/> instance.</returns>
    public static RsaPrivateKey FromParameters(RSAParameters parameters)
    {
        return new RsaPrivateKey
        {
            Modulus = Convert.ToBase64String(parameters.Modulus ?? ReadOnlySpan<byte>.Empty),
            Exponent = Convert.ToBase64String(parameters.Exponent ?? ReadOnlySpan<byte>.Empty),
            D = Convert.ToBase64String(parameters.D ?? ReadOnlySpan<byte>.Empty),
            P = Convert.ToBase64String(parameters.P ?? ReadOnlySpan<byte>.Empty),
            Q = Convert.ToBase64String(parameters.Q ?? ReadOnlySpan<byte>.Empty),
            DP = Convert.ToBase64String(parameters.DP ?? ReadOnlySpan<byte>.Empty),
            DQ = Convert.ToBase64String(parameters.DQ ?? ReadOnlySpan<byte>.Empty),
            InverseQ = Convert.ToBase64String(parameters.InverseQ ?? ReadOnlySpan<byte>.Empty)
        };
    }
}