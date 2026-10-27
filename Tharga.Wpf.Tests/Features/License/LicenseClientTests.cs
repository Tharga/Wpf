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
    public async Task Without_Public_Key_Option_The_Key_Is_Requested_From_The_Server()
    {
        var signingService = CreateSigningService();
        var handler = new LicenseServerHandler(signingService, signingService.BuildKeyPair(), request => request);
        var sut = CreateClient(signingService, handler);

        var result = await sut.CheckLicenseAsync();

        Assert.True(result.IsValid, result.Message);
        Assert.Equal(1, handler.KeyRequestCount);
    }

    [Fact]
    public async Task With_Public_Key_Option_Response_Signed_By_That_Key_Is_Valid()
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
    public async Task With_Public_Key_Option_Response_Signed_By_Another_Key_Is_Not_Valid()
    {
        var signingService = CreateSigningService();
        var configuredKeyPair = signingService.BuildKeyPair();
        var otherKeyPair = signingService.BuildKeyPair();
        var handler = new LicenseServerHandler(signingService, otherKeyPair, request => request);
        var sut = CreateClient(signingService, handler, _ => configuredKeyPair.Public);

        var result = await sut.CheckLicenseAsync();

        Assert.False(result.IsValid);
        Assert.Equal(0, handler.KeyRequestCount);
    }

    [Fact]
    public async Task With_Public_Key_Option_Returning_No_Key_The_Key_Is_Not_Requested_From_The_Server()
    {
        var signingService = CreateSigningService();
        var handler = new LicenseServerHandler(signingService, signingService.BuildKeyPair(), request => request);
        var sut = CreateClient(signingService, handler, _ => null);

        await Assert.ThrowsAsync<InvalidOperationException>(() => sut.CheckLicenseAsync());
        Assert.Equal(0, handler.KeyRequestCount);
    }

    private static LicenseClient CreateClient(Func<LicenseCheckRequest, LicenseCheckRequest> respondFor)
    {
        var signingService = CreateSigningService();
        var handler = new LicenseServerHandler(signingService, signingService.BuildKeyPair(), respondFor);
        return CreateClient(signingService, handler);
    }

    private static LicenseClient CreateClient(ISigningService signingService, LicenseServerHandler handler, Func<IConfiguration, RsaPublicKey?>? licensePublicKey = null)
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

        public int KeyRequestCount { get; private set; }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
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
}
