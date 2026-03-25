using Tharga.License;

namespace Tharga.License.Tests;

public class SigningServiceTests
{
    private readonly SigningService _sut = new();

    [Fact]
    public void BuildKeyPair_Returns_Valid_KeyPair()
    {
        var keyPair = _sut.BuildKeyPair();

        Assert.NotNull(keyPair);
        Assert.NotNull(keyPair.Public);
        Assert.NotNull(keyPair.Private);
        Assert.NotNull(keyPair.Public.Modulus);
        Assert.NotNull(keyPair.Public.Exponent);
        Assert.NotNull(keyPair.Private.Modulus);
        Assert.NotNull(keyPair.Private.D);
    }

    [Fact]
    public void BuildKeyPair_Generates_Unique_Keys_Each_Call()
    {
        var keyPair1 = _sut.BuildKeyPair();
        var keyPair2 = _sut.BuildKeyPair();

        Assert.NotEqual(keyPair1.Public.Modulus, keyPair2.Public.Modulus);
    }

    [Fact]
    public void Sign_Returns_Base64_Signature()
    {
        var keyPair = _sut.BuildKeyPair();
        var data = "test data to sign";

        var signature = _sut.Sign(data, keyPair.Private);

        Assert.NotNull(signature);
        Assert.NotEmpty(signature);
        // Verify it's valid base64
        var bytes = Convert.FromBase64String(signature);
        Assert.NotEmpty(bytes);
    }

    [Fact]
    public void VerifySignature_Returns_True_For_Valid_Signature()
    {
        var keyPair = _sut.BuildKeyPair();
        var data = "test data to sign";
        var signature = _sut.Sign(data, keyPair.Private);

        var result = _sut.VerifySignature(data, signature, keyPair.Public);

        Assert.True(result);
    }

    [Fact]
    public void VerifySignature_Returns_False_For_Tampered_Data()
    {
        var keyPair = _sut.BuildKeyPair();
        var data = "original data";
        var signature = _sut.Sign(data, keyPair.Private);

        var result = _sut.VerifySignature("tampered data", signature, keyPair.Public);

        Assert.False(result);
    }

    [Fact]
    public void VerifySignature_Returns_False_For_Wrong_Key()
    {
        var keyPair1 = _sut.BuildKeyPair();
        var keyPair2 = _sut.BuildKeyPair();
        var data = "test data";
        var signature = _sut.Sign(data, keyPair1.Private);

        var result = _sut.VerifySignature(data, signature, keyPair2.Public);

        Assert.False(result);
    }

    [Fact]
    public void Sign_Produces_Different_Signatures_For_Different_Data()
    {
        var keyPair = _sut.BuildKeyPair();

        var sig1 = _sut.Sign("data one", keyPair.Private);
        var sig2 = _sut.Sign("data two", keyPair.Private);

        Assert.NotEqual(sig1, sig2);
    }

    [Fact]
    public void Sign_And_Verify_Works_With_Empty_String()
    {
        var keyPair = _sut.BuildKeyPair();
        var signature = _sut.Sign("", keyPair.Private);

        var result = _sut.VerifySignature("", signature, keyPair.Public);

        Assert.True(result);
    }

    [Fact]
    public void Sign_And_Verify_Works_With_Unicode()
    {
        var keyPair = _sut.BuildKeyPair();
        var data = "Thargelion AB \u00e5\u00e4\u00f6 \u2603 \ud83d\ude00";
        var signature = _sut.Sign(data, keyPair.Private);

        var result = _sut.VerifySignature(data, signature, keyPair.Public);

        Assert.True(result);
    }
}
