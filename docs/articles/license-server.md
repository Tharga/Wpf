# License server

`Tharga.Wpf` can verify that the installed application has a valid license by calling a remote license server. The server-side endpoint is intentionally minimal so you can run it on top of any backend that exposes one route: `license/check`.

The `Tharga.License` NuGet package contains the request and response models and the signing service the endpoint needs. You implement the endpoint in your own API.

## Client configuration

Set `LicenseServerLocation` and `LicensePublicKey` in `Options`:

```csharp
protected override void Options(ThargaWpfOptions options)
{
    options.LicenseServerLocation += config => config["LicenseServer"];
    options.LicensePublicKey = config => new RsaPublicKey
    {
        Modulus = config["LicensePublicKey:Modulus"],
        Exponent = config["LicensePublicKey:Exponent"]
    };
}
```

The delegates receive `IConfiguration` so the URL and the key can be read from `appsettings.json` (or any other source you have wired into the host).

## Public key

The server signs every `license/check` response, and the client verifies that signature against `LicensePublicKey`. The key is required whenever `LicenseServerLocation` is set:

- The response is verified against this key only. The client never requests a key from the server.
- If `LicensePublicKey` is not set, or the delegate returns no key, the license is reported as not valid with a message saying no public key is configured, and the server is not called.

The server must sign with a fixed key pair that it keeps between starts, not a new one each time it starts. The key is `Modulus` and `Exponent`, base64.

## What the client accepts

The `license/check` response must be signed with the configured key and must return the request's `RequestKey`, `ApplicationName` and `MachineFingerprint.Value` unchanged. Any other response is reported as not valid.

That includes a response that is not JSON, misses a required field or has a field of the wrong type, and one whose `Signature` header is malformed or repeated. The signature is checked before the body is read, and the message says which check failed.

> **Changed in 3.1.0.** Before that, such a response threw an exception instead of reporting the license as not valid.

## Upgrading to 3.0

Before 3.0, `LicensePublicKey` was optional, and when it was not set the client downloaded the key from `{LicenseServerLocation}/license/key` and verified the response with it. Whoever answered at that address (it comes from configuration, and the hosts file or a proxy can redirect it) could serve their own key, sign their own response, and have the license reported as valid. See [GHSA-6q43-4qx5-7gqw](https://github.com/Tharga/Wpf/security/advisories/GHSA-6q43-4qx5-7gqw).

3.0 removes that download. To upgrade:

1. Make the license server sign with a fixed key pair that it keeps between starts. A server that builds a new key pair when it starts has to store one instead.
2. Set `LicensePublicKey` in the client to that key pair's public key.

Without step 2, every license check reports the license as not valid.

The fixes that came before it:

- **2.5.6:** the response must return the request's application name and machine fingerprint.
- **2.6.0:** `LicensePublicKey` was added as an option.

## Checking the license during splash

Pass `checkForLicense: true` to the canonical startup helper or to `ShowSplashAsync`:

```csharp
this.StartMainWindow<MainWindow>(
    showSplash: true,
    checkForUpdates: true,
    checkForLicense: true);
```

The splash makes a `license/check` call. The result is surfaced via `IApplicationUpdateStateService.LicenseInfoEvent`:

```csharp
update.LicenseInfoEvent += (_, args) =>
{
    if (!args.IsValid)
    {
        // args.Message describes why — display, log, or block the user.
    }
};
```

When license check is enabled the splash also exposes a `Client` / `Client Source` hyperlink so the user can see which client identifier the server used.

## Checking the license on demand

```csharp
var update = ApplicationBase.GetService<IApplicationUpdateStateService>();
var isValid = await update.CheckForLicenseAsync();
```

Returns the boolean directly; subscribe to `LicenseInfoEvent` if you also want the human-readable message.

## Server side — `Tharga.License`

`license/check` receives a `LicenseCheckRequest` and answers with a `LicenseCheckResponse` as JSON, with its signature in a `Signature` response header. Register the signing service with `services.AddThargaLicense()` and sign the JSON body with `ISigningService.Sign`:

```csharp
var json = JsonSerializer.Serialize(response, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });
Response.Headers["Signature"] = signingService.Sign(json, keyPair.Private);
return Content(json, "application/json");
```

`ISigningService.BuildKeyPair` creates a key pair. Create it once, store the private key with the server's secrets, and give the public key to the client through `LicensePublicKey`.

A `license/key` route is no longer called by the client. A server can keep one to hand out the public key, for example to copy into the client's configuration, but the client only ever trusts the key it is configured with.

## Disabling the license check

Leave `LicenseServerLocation` unset (the default) and do not pass `checkForLicense: true`.
