# Feature: Single-instance guard actually guards

## Source
Two GitHub issues filed by **Florida** against 2.3.0, both found while investigating a customer report that the point-of-sale client could run several times on one machine:

- [#49](https://github.com/Tharga/Wpf/issues/49) — single-instance lock is scoped to the Windows session, not the machine
- [#50](https://github.com/Tharga/Wpf/issues/50) — suppressed second instance still constructs its main window

Picked up during the Tier 1 cascade update, so the NuGet bumps for this repo ride in the same PR (per shared-instructions: package updates are applied up front on feature start).

## Goal
Make `AllowMultipleApplications = false` mean what it is documented to mean — **one instance per machine** — and make a suppressed instance actually do nothing.

## Scope

### #49 — machine-wide lock
- `SingleInstanceService`: prefix the mutex name with `Global\` so it lands in the machine-wide kernel namespace instead of the per-session `Local\` one.
- Give the mutex an **ACL**. Without one, a second instance in a different session (potentially a different user) gets `UnauthorizedAccessException` from `new Mutex` instead of `createdNew = false`. `MutexAcl.Create` with a `MutexSecurity` granting `Synchronize | Modify` to `WorldSid`.
- **Split the mutex name from the pipe name.** They currently share one `_pipeName` field. Named pipes are already machine-wide, so the pipe name must NOT gain the prefix — only the mutex.

### #50 — suppressed instance builds nothing
- `ApplicationBase.OnStartup`: set `args.PerformDefaultAction = false` before `Shutdown()`.
- `Shutdown()` is asynchronous — it posts to the dispatcher — so WPF's `DoStartup()` continues and calls `LoadComponent(StartupUri)`, constructing and showing the window. `PerformDefaultAction` is the documented hook that stops it.

### Versioning
- `MAJOR_MINOR` 2.3 → **2.4**. Both changes alter observable behaviour, so they must not ship as a patch.

### Docs
- `docs/articles/single-instance.md` says the lock is keyed off `ApplicationShortName`; the source uses **`ApplicationFullName`** (`ApplicationBase.cs:210,213`). Correct the doc — Florida deliberately varies `ApplicationFullName` so Test and Production run side by side, which the documented behaviour would forbid.
- Document that a host setting `StartupUri` was affected by #50.
- Document the new machine-wide scope, since it is a behaviour change.

### NuGet (cascade, applied first)
- Internal: `Tharga.Runtime` 1.0.0 → 1.0.1
- External library: `Microsoft.Extensions.DependencyInjection` / `.Hosting` / `.Http`, `System.Management` 10.0.10 → 10.0.11; `Microsoft.SourceLink.GitHub` 10.0.301 → 10.0.400 (Tharga.License)
- Test tooling: `Microsoft.NET.Test.Sdk` 18.6.0 → 18.9.0 (both test projects)

## Deliberately NOT in scope
- **An opt-in `SingleInstanceScope` (Machine | Session) enum.** #49 raises it as a consideration. Rejected: `AllowMultipleApplications = false` is *documented* as one-per-machine, so per-session behaviour is the bug, not a feature someone chose. Adding an enum would preserve the broken semantic as a supported option and expand the API for a hypothetical host. Revisit only if a real host reports depending on per-session scoping.
- The other five Wpf backlog items (ReSharper warnings, `TabNavigatorView` binding, `ObservableDictionary` shadowing, `ResxNotResolved`, FluentAssertions licence).

## Acceptance criteria
- `dotnet build -c Release` clean
- `dotnet test -c Release` green
- A test proves the mutex name is machine-scoped (`Global\` prefix) and that the pipe name is unchanged
- A test proves `PerformDefaultAction` is cleared when the lock is not acquired
- `MAJOR_MINOR` is 2.4 so the release is 2.4.0

## Done condition
User confirms, and **Florida re-verifies** both behaviours on a real multi-session machine — neither fix can be fully proven from a unit test, since #49 needs two Windows sessions and #50 needs a real `StartupUri`.
