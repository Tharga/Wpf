# Plan: Fix Background-Thread Exception Crash (#39)

## Steps

- [x] 1. ErrorEvent handler — resolve from `AppHost.Services` singleton, wrap in try/catch
- [x] 2. `FallbackHandlerInternalAsync` — dispatch to UI thread at the top if not already there
- [x] 3. Dispatcher-safe `MainWindow` read in `IExceptionStateService` factory
- [x] 4. Dispatcher-safe `MainWindow` read in `IApplicationUpdateStateService` factory
- [x] 5. Build and test — 0 errors, 146 tests pass
- [x] 6. Commit and PR (references #39)
