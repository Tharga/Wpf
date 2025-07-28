using System.Security.Cryptography;

namespace Tharga.License;

public record RsaPrivateKey
{
    public required string Modulus { get; init; }
    public required string Exponent { get; init; }
    public required string D { get; init; }
    public required string P { get; init; }
    public required string Q { get; init; }
    public required string DP { get; init; }
    public required string DQ { get; init; }
    public required string InverseQ { get; init; }

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