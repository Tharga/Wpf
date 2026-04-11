# Feature: Window Lifecycle Management

## Goal
Move window close/hide/restore behavior into the framework with configurable options, replacing boilerplate that every consuming app currently needs to implement manually.

## Scope

### New options on `ThargaWpfOptions`

**`HideOnClose`** (`bool`, default `false`):
- `false` (default): X and Default close exit the application. Standard behavior.
- `true`: Default close (X) hides the window instead of closing. Soft and Force close still exit.
- Developer is responsible for providing a way to restore the window (e.g. icon tray).

**`StartupWindowState`** (`enum`, default `Last`):
- `Last` — restore saved state including Visibility (Hidden = start in tray)
- `Normal` — always start in normal window state
- `Maximized` — always start maximized
- `Minimized` — always start minimized
- `Hidden` — always start hidden in tray

### `ApplicationBase.Hide()` — new method

Hides the main window and saves visibility state. Used by `HideOnClose` internally, and available for consumers (e.g. "Minimize to tray" menu item).

### Close mode behavior in `ApplicationBase.Close()`

| Method | HideOnClose=false | HideOnClose=true |
|--------|-------------------|------------------|
| Default (X) | Close tabs softly, exit | `Hide()` |
| Soft | Close tabs softly, exit | Close tabs softly, exit |
| Force | Close tabs forcefully, exit | Close tabs forcefully, exit |
| `Hide()` | Hide to tray | Hide to tray |

After an application update, use Force mode to ensure exit.

### Window size/position fallback in `WindowLocationService`

- If saved width or height ≤ 0, fall back to XAML-defined or reasonable defaults
- If saved position is entirely off-screen (no monitor contains the window), reposition to primary screen center

### Testable design

Extract close decision logic and restore logic into pure methods/services that can be unit tested without WPF Window objects:
- `CloseActionResolver` — given CloseMode + HideOnClose, returns the action to take
- `LocationValidator` — validates saved location against screen bounds, returns corrected location

## Acceptance Criteria
- Default behavior (no settings) closes the app on X and Exit — standard behavior
- `HideOnClose = true` hides on X, exits on Soft/Force
- `StartupWindowState` correctly restores Last, Normal, Maximized, Minimized, Hidden
- Zero-size and off-screen positions are corrected on restore
- Close mode after application update uses Force
- Sample app demonstrates both modes
- README documents all new options with examples
- Tests cover CloseActionResolver and LocationValidator logic

## Done Condition
- All acceptance criteria met
- All tests pass
- Committed and ready for review
