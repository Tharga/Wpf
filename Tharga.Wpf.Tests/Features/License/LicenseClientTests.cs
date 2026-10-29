using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Tharga.License;
using Tharga.Wpf.License;

namespace Tharga.Wpf.Tests.Features.License;

public class LicenseClientTests
{
    private const string ServerAddress = "https://license.example";
    private const string ApplicationName = "Test Application";

    [Fact]
    public async Task Response_Matching_The_Request_Is_Valid()
    {
        var sut = CreateClient(request => request);

        var result = await sut.CheckLicenseAsync();

        Assert.True(result.IsValid, result.Message);
    }

    [Fact]
    public async Task Response_For_Another_Machine_Is_Not_Valid()
    {
        var sut = CreateClient(request => request with
        {
            MachineFingerprint = new Fingerprint { Name = request.MachineFingerprint.Name, Value = "another-machine" }
        });

        var result = await sut.CheckLicenseAsync();

        Assert.False(result.IsValid);
    }

    [Fact]
    public async Task Response_For_Another_Application_Is_Not_Valid()
    {
        var sut = CreateClient(request => request with { ApplicationName = "Another Application" });

        var result = await sut.CheckLicenseAsync();

        Assert.False(result.IsValid);
    }

    [Fact]
    public async Task Response_Signed_By_The_Configured_Key_Is_Valid_Without_Requesting_The_Key_From_The_Server()
    {
        var signingService = CreateSigningService();
        var serverKeyPair = signingService.BuildKeyPair();
        var handler = new LicenseServerHandler(signingService, serverKeyPair, request => request);
        var sut = CreateClient(signingService, handler, _ => serverKeyPair.Public);

        var result = await sut.CheckLicenseAsync();

        Assert.True(result.IsValid, result.Message);
        Assert.Equal(0, handler.KeyRequestCount);
    }

    [Fact]
    public async Task Response_Signed_By_A_Key_The_Server_Publishes_Instead_Of_The_Configured_Key_Is_Not_Valid()
    {
        var signingService = CreateSigningService();
        var configuredKeyPair = signingService.BuildKeyPair();
        var serverKeyPair = signingService.BuildKeyPair();
        var handler = new LicenseServerHandler(signingService, serverKeyPair, request => request);
        var sut = CreateClient(signingService, handler, _ => configuredKeyPair.Public);

        var result = await sut.CheckLicenseAsync();

        Assert.False(result.IsValid);
        Assert.Equal("Invalid signature.", result.Message);
        Assert.Equal(0, handler.KeyRequestCount);
    }

    [Fact]
    public async Task Without_Public_Key_Option_The_License_Is_Not_Valid_And_The_Server_Is_Not_Called()
    {
        var signingService = CreateSigningService();
        var handler = new LicenseServerHandler(signingService, signingService.BuildKeyPair(), request => request);
        var sut = CreateClient(signingService, handler, null);

        var result = await sut.CheckLicenseAsync();

        Assert.False(result.IsValid);
        Assert.Equal(LicenseClient.NoPublicKeyMessage, result.Message);
        Assert.Equal(0, handler.RequestCount);
    }

    [Fact]
    public async Task With_Public_Key_Option_Returning_No_Key_The_License_Is_Not_Valid_And_The_Server_Is_Not_Called()
    {
        var signingService = CreateSigningService();
        var handler = new LicenseServerHandler(signingService, signingService.BuildKeyPair(), request => request);
        var sut = CreateClient(signingService, handler, _ => null);

        var result = await sut.CheckLicenseAsync();

        Assert.False(result.IsValid);
        Assert.Equal(LicenseClient.NoPublicKeyMessage, result.Message);
        Assert.Equal(0, handler.RequestCount);
    }

    [Fact]
    public async Task Signed_Response_Missing_A_Required_Field_Is_Not_Valid()
    {
        var sut = CreateRawClient(requestKey => SignedJson($$"""{"responseKey":"{{requestKey}}","applicationName":"{{ApplicationName}}","approvedAt":"2026-01-01T00:00:00Z"}"""));

        var result = await sut.CheckLicenseAsync();

        Assert.False(result.IsValid);
        Assert.StartsWith(LicenseClient.InvalidResponseMessage, result.Message);
    }

    [Fact]
    public async Task Signed_Response_With_A_Field_Of_The_Wrong_Type_Is_Not_Valid()
    {
        var sut = CreateRawClient(requestKey => SignedJson($$"""{"responseKey":"{{requestKey}}","applicationName":"{{ApplicationName}}","machineFingerprint":{"name":"m","value":"v"},"approvedAt":"not a date"}"""));

        var result = await sut.CheckLicenseAsync();

        Assert.False(result.IsValid);
        Assert.StartsWith(LicenseClient.InvalidResponseMessage, result.Message);
    }

    [Fact]
    public async Task Signed_Response_That_Is_Not_Json_Is_Not_Valid()
    {
        var sut = CreateRawClient(_ => SignedJson("not json"));

        var result = await sut.CheckLicenseAsync();

        Assert.False(result.IsValid);
        Assert.StartsWith(LicenseClient.InvalidResponseMessage, result.Message);
    }

    [Theory]
    [InlineData("{}")]
    [InlineData("{\"message\":42}")]
    [InlineData("[]")]
    [InlineData("")]
    [InlineData("<html>Bad gateway</html>")]
    public async Task Unsigned_Response_Without_A_Message_Reports_The_Missing_Signature(string body)
    {
        var sut = CreateRawClient(_ => Response(body));

        var result = await sut.CheckLicenseAsync();

        Assert.False(result.IsValid);
        Assert.Equal(LicenseClient.NoSignatureMessage, result.Message);
    }

    [Fact]
    public async Task Unsigned_Response_Reports_The_Server_Message()
    {
        var sut = CreateRawClient(_ => Response("""{"message":"No license for this machine."}"""));

        var result = await sut.CheckLicenseAsync();

        Assert.False(result.IsValid);
        Assert.Equal("No license for this machine.", result.Message);
    }

    [Theory]
    [InlineData("not base64!")]
    [InlineData("")]
    public async Task Response_With_A_Malformed_Signature_Is_Not_Valid(string signature)
    {
        var sut = CreateRawClient(requestKey =>
        {
            var response = Response(CompleteResponseJson(requestKey));
            response.Headers.TryAddWithoutValidation(SignatureHeader, signature);
            return response;
        });

        var result = await sut.CheckLicenseAsync();

        Assert.False(result.IsValid);
        Assert.Equal(LicenseClient.InvalidSignatureMessage, result.Message);
    }

    [Fact]
    public async Task Response_With_Two_Signatures_Is_Not_Valid()
    {
        var sut = CreateRawClient(requestKey =>
        {
            var json = CompleteResponseJson(requestKey);
            var response = SignedJson(json);
            response.Headers.Add(SignatureHeader, RawKeyPair.SigningService.Sign(json, RawKeyPair.KeyPair.Private));
            return response;
        });

        var result = await sut.CheckLicenseAsync();

        Assert.False(result.IsValid);
        Assert.Equal(LicenseClient.InvalidSignatureMessage, result.Message);
    }

    private const string SignatureHeader = "Signature";

    private static readonly (ISigningService SigningService, RsaKeyPair KeyPair) RawKeyPair = CreateRawKeyPair();

    private static (ISigningService SigningService, RsaKeyPair KeyPair) CreateRawKeyPair()
    {
        var signingService = CreateSigningService();
        return (signingService, signingService.BuildKeyPair());
    }

    private static LicenseClient CreateRawClient(Func<string, HttpResponseMessage> respond)
    {
        return CreateClient(RawKeyPair.SigningService, new RawLicenseServerHandler(respond), _ => RawKeyPair.KeyPair.Public);
    }

    private static string CompleteResponseJson(string requestKey)
    {
        return $$"""{"responseKey":"{{requestKey}}","applicationName":"{{ApplicationName}}","machineFingerprint":{"name":"m","value":"v"},"approvedAt":"2026-01-01T00:00:00Z"}""";
    }

    private static HttpResponseMessage Response(string body)
    {
        return new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(body, Encoding.UTF8, "application/json")
        };
    }

    private static HttpResponseMessage SignedJson(string json)
    {
        var response = Response(json);
        response.Headers.Add(SignatureHeader, RawKeyPair.SigningService.Sign(json, RawKeyPair.KeyPair.Private));
        return response;
    }

    private static LicenseClient CreateClient(Func<LicenseCheckRequest, LicenseCheckRequest> respondFor)
    {
        var signingService = CreateSigningService();
        var serverKeyPair = signingService.BuildKeyPair();
        var handler = new LicenseServerHandler(signingService, serverKeyPair, respondFor);
        return CreateClient(signingService, handler, _ => serverKeyPair.Public);
    }

    private static LicenseClient CreateClient(ISigningService signingService, HttpMessageHandler handler, Func<IConfiguration, RsaPublicKey?>? licensePublicKey)
    {
        var clientFactory = new Mock<IHttpClientFactory>();
        clientFactory.Setup(x => x.CreateClient(It.IsAny<string>())).Returns(() => new HttpClient(handler, false));

        var options = new ThargaWpfOptions
        {
            ApplicationFullName = ApplicationName,
            LicenseServerLocation = _ => ServerAddress,
            LicensePublicKey = licensePublicKey!
        };

        return new LicenseClient(clientFactory.Object, new ConfigurationBuilder().Build(), signingService, options);
    }

    private static ISigningService CreateSigningService()
    {
        var services = new ServiceCollection();
        services.AddThargaLicense();
        return services.BuildServiceProvider().GetRequiredService<ISigningService>();
    }

    private class LicenseServerHandler : HttpMessageHandler
    {
        private readonly ISigningService _signingService;
        private readonly RsaKeyPair _keyPair;
        private readonly Func<LicenseCheckRequest, LicenseCheckRequest> _respondFor;

        public LicenseServerHandler(ISigningService signingService, RsaKeyPair keyPair, Func<LicenseCheckRequest, LicenseCheckRequest> respondFor)
        {
            _signingService = signingService;
            _keyPair = keyPair;
            _respondFor = respondFor;
        }

        public int RequestCount { get; private set; }
        public int KeyRequestCount { get; private set; }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            RequestCount++;
            var path = request.RequestUri!.AbsolutePath;

            if (request.Method == HttpMethod.Get && path == "/license/key")
            {
                KeyRequestCount++;
                return new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StringContent(JsonSerializer.Serialize(_keyPair.Public), Encoding.UTF8, "application/json")
                };
            }

            if (request.Method == HttpMethod.Post && path == "/license/check")
            {
                var checkRequest = await request.Content!.ReadFromJsonAsync<LicenseCheckRequest>(cancellationToken);
                var answeredFor = _respondFor(checkRequest!);

                var checkResponse = new LicenseCheckResponse
                {
                    ResponseKey = checkRequest!.RequestKey,
                    ApplicationName = answeredFor.ApplicationName,
                    MachineFingerprint = answeredFor.MachineFingerprint,
                    ApprovedAt = DateTime.UtcNow
                };

                var json = JsonSerializer.Serialize(checkResponse, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });
                var response = new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StringContent(json, Encoding.UTF8, "application/json")
                };
                response.Headers.Add("Signature", _signingService.Sign(json, _keyPair.Private));
                return response;
            }

            return new HttpResponseMessage(HttpStatusCode.NotFound);
        }
    }

    private class RawLicenseServerHandler : HttpMessageHandler
    {
        private readonly Func<string, HttpResponseMessage> _respond;

        public RawLicenseServerHandler(Func<string, HttpResponseMessage> respond)
        {
            _respond = respond;
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var checkRequest = await request.Content!.ReadFromJsonAsync<LicenseCheckRequest>(cancellationToken);
            return _respond(checkRequest!.RequestKey);
        }
    }
}
