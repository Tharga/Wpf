# Feature: Update Restart Fixes

## Goal
Fix two related production issues from Florida:
1. **CloseAllTabsAsync threading**: Squirrel update fails with `InvalidOperationException` because tab closing runs on a background thread
2. **Stuck splash on error**: When an update fails, the close button doesn't appear — user has to kill via Task Manager
3. **Unify Squirrel and Velopack**: Both should close tabs before restart in the same way

## Scope

### Unified pre-restart logic
Move the pre-restart steps (tab closing + single-instance lock release) into the base class so both Squirrel and Velopack work identically:
- Add `protected async Task BeforeRestartAsync()` to `ApplicationUpdateStateServiceBase`
- Method dispatches `CloseAllTabsAsync` through `Application.Current.Dispatcher.InvokeAsync` so it's UI-thread safe
- Method also calls `ApplicationBase.ReleaseSingleInstanceLock()`
- Both Squirrel and Velopack subclasses call `await BeforeRestartAsync()` before their restart method
- Remove the existing direct calls in subclasses

### Defensive splash cleanup on error
Ensure the user is never stuck with a closed but visible splash:
- In `UpdateClientApplication`'s catch block, wrap each cleanup call (`HideProgress`, `SetErrorMessage`, `ShowCloseButton`, `Show`) in its own try/catch
- After error cleanup, ensure the splash is visible (`_splash?.Show()`) and has a close button
- Add a final guard in `finally`: if there was an exception AND splash exists AND no close button, force-show one

## Acceptance Criteria
- Squirrel update no longer throws on tab closing during restart
- Velopack now closes tabs before restart (same as Squirrel)
- After a failed update, the splash always has a visible close button
- All existing tests pass

## Done Condition
- All acceptance criteria met
- Tests pass
- PR created
