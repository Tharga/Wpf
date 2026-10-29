using Microsoft.Extensions.Configuration;
using System.IO;
using System.Management;
using System.Net.Http;
using System.Net.Http.Json;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using Tharga.License;

namespace Tharga.Wpf.License;

internal class LicenseClient : ILicenseClient
{
    internal const string NoPublicKeyMessage = "No license public key is configured. Set LicensePublicKey in ThargaWpfOptions to the public key the license server signs with.";
    internal const string NoSignatureMessage = "Cannot find signature.";
    internal const string InvalidSignatureMessage = "Invalid signature.";
    internal const string InvalidResponseMessage = "Invalid license response.";
    private const string SignatureHeader = "Signature";

    private readonly IHttpClientFactory _clientFactory;
    private readonly IConfiguration _configuration;
    private readonly ISigningService _signingService;
    private readonly ThargaWpfOptions _options;

    public LicenseClient(IHttpClientFactory clientFactory, IConfiguration configuration, ISigningService signingService, ThargaWpfOptions options)
    {
        _clientFactory = clientFactory;
        _configuration = configuration;
        _signingService = signingService;
        _options = options;
    }

    public async Task<(bool IsValid, string Message)> CheckLicenseAsync()
    {
        var address = _options.LicenseServerLocation?.Invoke(_configuration);
        if (address == null) throw new InvalidOperationException("No license server address found.");

        var publicKey = _options.LicensePublicKey?.Invoke(_configuration);
        if (publicKey == null) return (false, NoPublicKeyMessage);

        var version = BuildVersionFingerprint();
        var machine = GetMachineFingerprint();
        var requestKey = Guid.NewGuid().ToString().Replace("-", "");

        var request = new LicenseCheckRequest
        {
            RequestKey = requestKey,
            ApplicationName = _options.ApplicationFullName ?? _options.ApplicationShortName ?? version.applicationName,
            MachineFingerprint = new Fingerprint { Name = machine.MachineName, Value = machine.Fingerprint },
            VersionFingerprint = new Fingerprint { Name = version.Version, Value = version.Fingerprint },
            Username = Environment.UserName
        };

        using var client = _clientFactory.CreateClient("license");
        var response = await client.PostAsJsonAsync($"{address}/license/check", request);
        if (!response.IsSuccessStatusCode) return (false, $"{response.ReasonPhrase}. (Status code: {(int)response.StatusCode})");

        var json = await response.Content.ReadAsStringAsync();

        if (!response.Headers.TryGetValues(SignatureHeader, out var signatures))
        {
            return (false, ReadServerMessage(json) ?? NoSignatureMessage);
        }

        var signatureValues = signatures.ToArray();
        if (signatureValues.Length != 1 || !IsSignatureValid(json, signatureValues[0], publicKey)) return (false, InvalidSignatureMessage);

        LicenseCheckResponse data;
        try
        {
            data = JsonSerializer.Deserialize<LicenseCheckResponse>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        }
        catch (JsonException e)
        {
            return (false, $"{InvalidResponseMessage} {e.Message}");
        }

        if (data?.ResponseKey != requestKey) return (false, "Invalid response key.");
        if (!string.Equals(data.ApplicationName, request.ApplicationName, StringComparison.Ordinal)) return (false, "Invalid response application.");
        if (!string.Equals(data.MachineFingerprint?.Value, request.MachineFingerprint.Value, StringComparison.Ordinal)) return (false, "Invalid response machine fingerprint.");

        return (true, "License is valid.");
    }

    private bool IsSignatureValid(string json, string signature, RsaPublicKey publicKey)
    {
        try
        {
            return _signingService.VerifySignature(json, signature, publicKey);
        }
        catch (FormatException)
        {
            return false;
        }
    }

    private static string ReadServerMessage(string json)
    {
        try
        {
            using var doc = JsonDocument.Parse(json);
            return doc.RootElement.ValueKind == JsonValueKind.Object
                   && doc.RootElement.TryGetProperty("message", out var message)
                   && message.ValueKind == JsonValueKind.String
                ? message.GetString()
                : null;
        }
        catch (JsonException)
        {
            return null;
        }
    }

    private static (string MachineName, string Fingerprint) GetMachineFingerprint()
    {
        var identifiers = new List<string>
        {
            Environment.MachineName,
            GetWmiProperty("Win32_Processor", "ProcessorId"),
            GetWmiProperty("Win32_BIOS", "SerialNumber"),
            GetWmiProperty("Win32_BaseBoard", "SerialNumber")
        };

        var raw = string.Join("|", identifiers.Where(i => !string.IsNullOrWhiteSpace(i)));
        return (Environment.MachineName, Convert.ToBase64String(SHA256.HashData(Encoding.UTF8.GetBytes(raw))));
    }

    private static string GetWmiProperty(string wmiClass, string property)
    {
        try
        {
            using var searcher = new ManagementObjectSearcher($"SELECT {property} FROM {wmiClass}");
            foreach (var o in searcher.Get())
            {
                var mo = (ManagementObject)o;
                return mo[property]?.ToString();
            }
        }
        catch
        {
            // ignore and fallback
        }

        return "";
    }

    private static (string applicationName, string Version, string Fingerprint) BuildVersionFingerprint()
    {
        var entryAssembly = Assembly.GetEntryAssembly();
        if (entryAssembly == null) throw new InvalidOperationException("Could not determine entry assembly.");

        var assemblyName = entryAssembly.GetName();
        var version = $"{assemblyName.Version}";
        var prefix = assemblyName.Name?.Split('.')[0] ?? throw new InvalidOperationException("Invalid assembly name.");

        var assemblies = AppDomain.CurrentDomain
            .GetAssemblies()
            .Where(a =>
                !a.IsDynamic &&
                !string.IsNullOrEmpty(a.Location) &&
                (a.GetName().Name?.StartsWith(prefix, StringComparison.OrdinalIgnoreCase) == true
                 || a.GetName().Name?.StartsWith("Tharga", StringComparison.OrdinalIgnoreCase) == true))
            .OrderBy(a => a.Location)
            .ToArray();

        using var sha256 = SHA256.Create();
        using var stream = new MemoryStream();

        foreach (var assembly in assemblies)
        {
            try
            {
                var bytes = File.ReadAllBytes(assembly.Location);
                var hash = sha256.ComputeHash(bytes);
                stream.Write(hash);
            }
            catch
            {
                // Skip inaccessible assemblies (e.g. dynamic or reflection-only)
            }
        }

        var finalHash = sha256.ComputeHash(stream.ToArray());
        return (assemblyName.Name, version, Convert.ToBase64String(finalHash));
    }
}