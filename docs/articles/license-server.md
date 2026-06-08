# License server

`Tharga.Wpf` can verify that the installed application has a valid license by calling a remote license server. The server-side endpoint is intentionally minimal so you can run it on top of any backend that exposes two routes: `license/check` and `license/key`.

The `Tharga.License` NuGet package contains a turn-key implementation for ASP.NET Core hosts. You can also implement the endpoints yourself.

## Client configuration

Set `LicenseServerLocation` in `Options`:

```csharp
protected override void Options(ThargaWpfOptions options)
{
    options.LicenseServerLocation += config => config["LicenseServer"];
}
```

The delegate receives `IConfiguration` so the URL can be read from `appsettings.json` (or any other source you have wired into the host).

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

## Server side — `Tharga.License` for ASP.NET Core

The `Tharga.License` package wraps the two endpoints in a controller-friendly shape. Wire it into your API project; the WPF host's calls to `license/check` and `license/key` resolve to the package's implementation, which in turn delegates the actual license lookup to your `ILicenseClient`.

`ILicenseClient`:

```csharp
public interface ILicenseClient
{
    Task<(bool IsValid, string Message)> CheckLicenseAsync();
}
```

Implement it however your product handles licenses (per-tenant flag in MongoDB, KV in Azure, a third-party service, etc.) and Tharga.License's controller plumbing routes the WPF host's request to your implementation.

## Disabling the license check

Leave `LicenseServerLocation` unset (the default). The client never makes the call, `checkForLicense` becomes a no-op, and `CheckForLicenseAsync` returns `true`.
