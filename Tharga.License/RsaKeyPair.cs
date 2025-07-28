namespace Tharga.License;

public record RsaKeyPair
{
    public required RsaPublicKey Public { get; init; }
    public required RsaPrivateKey Private { get; init; }
}