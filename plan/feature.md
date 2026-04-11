# Feature: Single Instance

## Goal
Improve the existing `AllowMultipleApplications = false` feature to properly handle hidden windows and be compatible with Squirrel/Velopack update restarts.

## Current state
- `AllowMultipleApplications` exists on `ThargaWpfOptions` (default `true`)
- Uses `Mutex` to detect duplicate instances
- `BringExistingInstanceToFront` uses `WindowHelper.FocusWindowByProcessId` with Win32 `SetForegroundWindow`
- **Bug**: When the existing instance is hidden to tray, `MainWindowHandle` is `IntPtr.Zero` — the second instance can't find or show it
- **Bug**: Squirrel `RestartApp()` and Velopack `ApplyUpdatesAndRestart()` launch a new process — the mutex blocks it

## Scope

### Fix 1: Show hidden instance
When the second instance detects the mutex is held, it needs to signal the existing instance to show itself. Options:
- Named pipe: second instance sends a "show" message, first instance listens and shows
- This works regardless of whether the window is visible, minimized, or hidden to tray

### Fix 2: Release mutex before update restart
- `SquirrelApplicationUpdateStateService.UpdateAsync`: release mutex before `UpdateManager.RestartApp()`
- `VelopackApplicationUpdateStateService.UpdateAsync`: release mutex before `mgr.ApplyUpdatesAndRestart()`
- Expose `ReleaseMutex()` on `ApplicationBase` for the update services to call

### Rename option
- Rename `AllowMultipleApplications` to a clearer name or add `SingleInstance` as an alias
- Keep backwards compatibility

## Acceptance Criteria
- Second instance shows a hidden-to-tray first instance
- Second instance shows a minimized first instance
- Second instance focuses a visible first instance
- Squirrel update restart works with SingleInstance enabled
- Velopack update restart works with SingleInstance enabled
- Tests cover the named pipe message logic

## Done Condition
- All acceptance criteria met
- All tests pass
- README documented
- Sample app demonstrates the feature
