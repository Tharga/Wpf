# Feature: Fix Background-Thread Exception Crash

## Goal
Fix GitHub issue #39: any exception on a non-UI thread crashes the whole process because the error handler reads UI-thread-only `Application.MainWindow` off-thread, throwing a secondary unhandled `InvalidOperationException` with `IsTerminating=True`.

## Root cause (two compounding defects)
1. **`StaticExceptionHandler.ErrorEvent` handler** (`ApplicationBase.cs:131`) calls `services.BuildServiceProvider().GetService<IExceptionStateService>()` — builds a throwaway provider on the error's thread, bypassing the singleton and re-running the factory off-thread.
2. **Factory reads `MainWindow` off-thread** (`ApplicationBase.cs:164` and `:98`) — `Application.MainWindow` is UI-thread-only and throws when read from a ThreadPool thread.

## Scope

### Fix 1: ErrorEvent handler uses the real singleton
- Resolve `IExceptionStateService` from `AppHost.Services` (the app-lifetime singleton) instead of `services.BuildServiceProvider()`
- Wrap in try/catch so the error handler can never itself crash the process

### Fix 2: FallbackHandlerInternalAsync dispatches to UI thread
- At the top of `ExceptionStateService.FallbackHandlerInternalAsync`, if not on the dispatcher thread, marshal the whole call via `Application.Current.Dispatcher.InvokeAsync` and return. This makes the handler safe to call from any thread.

### Fix 3: Dispatcher-safe MainWindow reads in factories
- In both factories (`IExceptionStateService` at `:164`, `IApplicationUpdateStateService` at `:98`), read `MainWindow` through `Current.Dispatcher.Invoke(() => ...)` so the singleton can be constructed from any thread without throwing.

## Acceptance Criteria
- A background-thread exception routed through `StaticExceptionHandler.Handle` does not crash the process
- The error handler resolves the singleton (not a throwaway provider)
- `MainWindow` is never read off the UI thread
- All existing tests pass

## Done Condition
- All acceptance criteria met
- Tests pass
- PR created, references #39
