//using System.Security.Cryptography;

//namespace Tharga.Wpf.License;

//public record RsaPublicKey
//{
//    public string Modulus { get; init; }
//    public string Exponent { get; init; }

//    public RSAParameters ToParameters()
//    {
//        return new RSAParameters
//        {
//            Modulus = Convert.FromBase64String(Modulus),
//            Exponent = Convert.FromBase64String(Exponent)
//        };
//    }

//    public static RsaPublicKey FromParameters(RSAParameters parameters)
//    {
//        return new RsaPublicKey
//        {
//            Modulus = Convert.ToBase64String(parameters.Modulus ?? ReadOnlySpan<byte>.Empty),
//            Exponent = Convert.ToBase64String(parameters.Exponent ?? ReadOnlySpan<byte>.Empty)
//        };
//    }
//}