using System.Net.Http;
using Microsoft.Extensions.Configuration;
using Tharga.Toolkit;

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
        var path = _options.UpdateLocation?.Invoke(_configuration);
        if (path.IsNullOrEmpty()) return (null, null);

        switch (_options.UpdateSystem)
        {
            case UpdateSystem.None:
                throw new NotSupportedException();
            case UpdateSystem.Squirrel:
                var httpClient = _httpClientFactory.CreateClient("ApplicationUpdate");
                var result = await httpClient.GetAsync(path);
                if (!result.IsSuccessStatusCode) throw new InvalidOperationException($"Failed to get application location at '{path}'.");
                var data = await result.Content.ReadAsStringAsync();
                return (data, path);
            case UpdateSystem.Velopack:
                return (path, path);
            default:
                throw new ArgumentOutOfRangeException(nameof(_options.UpdateSystem), @$"Unknown {nameof(_options.UpdateSystem)} {_options.UpdateSystem}");
        }
    }
}