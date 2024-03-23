using System.Net.Http;
using Microsoft.Extensions.Configuration;

namespace Tharga.Wpf.ApplicationUpdate;

internal class ApplicationDownloadService : IApplicationDownloadService
{
    private readonly IConfiguration _configuration;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ThargaWpfOptions _options;

    public ApplicationDownloadService(IConfiguration configuration, IHttpClientFactory httpClientFactory, ThargaWpfOptions options)
    {
        _configuration = configuration;
        _httpClientFactory = httpClientFactory;
        _options = options;
    }

    public async Task<(string ApplicationLocation, string ApplicationLocationSource)> GetApplicationLocationAsync()
    {
        var requestUri = _options.ApplicationDownloadLocationLoader?.Invoke(_configuration);
        if (requestUri == default) return (null, null);

        var httpClient = _httpClientFactory.CreateClient("ApplicationUpdate");
        var result = await httpClient.GetAsync(requestUri);
        if (!result.IsSuccessStatusCode) throw new InvalidOperationException($"Failed to get application location at '{requestUri}'.");
        var data = await result.Content.ReadAsStringAsync();
        return (data, requestUri.OriginalString);
    }
}