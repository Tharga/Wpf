# Feature: Splash Foreground Color + Discoverable Startup

Two related Tharga.Wpf splash requests from MediaManager (2026-05-31).

## Goal

### #1 — Foreground color (Important)
Built-in `Splash` overlays text on top of a background image — `FullName`, `Environment`, `Version`, `ExeLocation`, `Client`/`Client Source` hyperlinks, and the `Messages` ListBox — with the default WPF foreground (dark). A consumer with a **dark** custom splash image (via `SplashCreator`/`ImagePath`) cannot read this text. The only workaround today is to replace the whole splash window with a custom `ISplash`, losing all built-in behaviour.

Expose a way to set the foreground color so light-on-dark splash images are readable.

### #2 — Discoverable splash startup (Nice)
With only `StartupUri` + `CompanyName`/`SplashCreator`, no splash appears — the consumer must call `IApplicationUpdateStateService.ShowSplashAsync()` themselves. Nothing in `ApplicationBase`/`ThargaWpfOptions` surface hints at this. Document the trigger and the existing `AppExtensions.StartMainWindow<T>(showSplash: true, ...)` helper as the canonical entry point.

## Scope

### Foreground color
- Add `Foreground` (Brush) property on `SplashData`. Defaults to `null` → built-in `Splash` keeps the current default (no behaviour change).
- Built-in `Splash` constructor applies the brush to all overlaid text elements when set: `FullName`, `Environment`, `Version`, `ExeLocation`, `Messages` (ListBox), the two `Hyperlink` elements, and the `CloseButton` foreground.
- Pass-through from `ThargaWpfOptions.SplashForeground` (Brush) so consumers who use the default `Splash` (no custom `SplashCreator`) can set it directly via options.
- `ApplicationUpdateStateService.ShowSplashAsync` populates `SplashData.Foreground` from `_options.SplashForeground` when building the `SplashData`.

### README documentation
- Add a "Foreground color" subsection under "Splash Screen" with a light-on-dark example.
- Add a "Triggering the splash" subsection that explains the three entry points: (a) `AppExtensions.StartMainWindow<T>(showSplash: true, ...)` — canonical, (b) manual `IApplicationUpdateStateService.ShowSplashAsync(checkForUpdates: true)` for full control, (c) `ShowSplashAsync(showCloseButton: true)` for About-style display.

## Acceptance Criteria
- `SplashData.Foreground` is a `Brush` (init).
- `ThargaWpfOptions.SplashForeground` is a `Brush` (set).
- Setting either makes all overlaid text readable on a dark image.
- Default behaviour (no Foreground set) is unchanged.
- README has "Foreground color" subsection with example.
- README has "Triggering the splash" subsection with the three entry points.
- All existing tests pass.

## Done Condition
- All acceptance criteria met
- PR created
