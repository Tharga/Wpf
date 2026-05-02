# Feature: Pipe ErrorId-displayed exceptions to ILogger.LogError

## Goal
Ensure every exception that produces a user-facing ErrorId-dialog also reaches Application Insights via `ILogger.LogError`. Today, exceptions handled by custom `IExceptionHandler<T>` implementations (registered via `RegisterExceptionHandler`) bypass the existing log statement entirely.

## Background
Florida 2026-05-01 NEPTUNUS: an `InvalidOperationException` was displayed to the cashier with ErrorId `e39de547...` but does NOT appear in `AppExceptions` or `AppTraces`. Root cause: Florida registers a custom `InvalidOperationExceptionHandler` via `RegisterExceptionHandler`. The current `FallbackHandlerInternalAsync`:

```csharp
if (_exceptionHandlers.TryGetValue(exception.GetType(), out var handlerType))
{
    // invoke custom handler — early return, no LogError
    return;
}
// ... LogError only happens here, in the fallback path
```

So custom handlers display a dialog with their own ErrorId, but the exception is invisible in AI.

## Scope
**`ExceptionStateService.FallbackHandlerInternalAsync`** — log the exception at the top of the method, before any handler runs:

```csharp
var correlationId = Guid.NewGuid();
_logger?.LogError(exception, "ErrorId={ErrorId} ExceptionType={ExceptionType} Message={Message}",
    correlationId, exception.GetType().FullName, exception.Message);
```

The fallback path then uses the same `correlationId` in the dialog (already does). Custom handlers continue to display their own dialog — but at least the exception is now in AI, queryable.

## Acceptance Criteria
- Every call to `FallbackHandlerInternalAsync` produces an `AppExceptions` row in AI (via `ILogger.LogError`)
- Custom handlers continue to work unchanged (backwards compatible)
- Fallback dialog ErrorId matches the logged ErrorId
- All existing tests pass

## Done Condition
- All acceptance criteria met
- Tests pass
- PR created
