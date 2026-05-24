using System.Net.Http;
using Microsoft.Extensions.Configuration;

namespace Tharga.Wpf.ApplicationUpdate;

internal class ApplicationDownloadService : IApplicationDownloadService
{
    private readonly IConfiguration _configuration;
    private readonly ThargaWpfOptions _options;

    public ApplicationDownloadService(IConfiguration configuration, IHttpClientFactory httpClientFactory, ThargaWpfOptions options)
    {
        _configuration = configuration;
        _options = options;
    }

    public Task<(string ApplicationLocation, string ApplicationLocationSource)> GetApplicationLocationAsync()
    {
        var path = _options.UpdateLocation?.Invoke(_configuration);
        if (string.IsNullOrEmpty(path)) return Task.FromResult<(string, string)>((null, null));

        return _options.UpdateSystem switch
        {
            UpdateSystem.None => throw new NotSupportedException(),
            UpdateSystem.Velopack => Task.FromResult<(string, string)>((path, path)),
            _ => throw new ArgumentOutOfRangeException(nameof(_options.UpdateSystem), @$"Unknown {nameof(_options.UpdateSystem)} {_options.UpdateSystem}")
        };
    }
}