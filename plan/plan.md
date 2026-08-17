# Plan: display-change-revalidation

Note: NuGet check at feature start (2026-08-17) found no available updates
(`dotnet list package --outdated` clean across the solution), so there are no
upgrade steps.

## Steps

- [x] 1. `LocationValidator`: clamp width/height to the rescue screen's work area before
        centring; rescue to the primary screen (`ScreenBounds` gains `IsPrimary`, default
        false; `Validate` prefers the primary, falls back to `screens[0]`).
        Done — 7 new tests in `LocationValidatorTests`, including the reported
        1455×865-on-1440×852 case asserting a fully on-screen non-negative result.
- [x] 2. `SetLocation` captures bounds only in `Normal` state (negative Left/Top saved
        as-is; minimized −32000 and maximized bounds excluded by the state guard, replacing
        the old `>= 0` guard). Save/load format extracted to internal `LocationFormatter`;
        metadata index fixed (`parts[7]`) — round-trip covered by `LocationFormatterTests`.
- [x] 3. `MonitorEngine` subscribes to `SystemEvents.DisplaySettingsChanged` in `OnLoaded`,
        debounced 750 ms via new internal `Framework/Debouncer` (System.Threading.Timer,
        restart-on-trigger; covered by `DebouncerTests`). Revalidation dispatches to the
        window's dispatcher, skips non-`Normal` states, applies the validated bounds (which
        triggers the normal save path, so the rescued position is persisted). Unsubscribed
        and debouncer disposed on window `Closed`.
- [x] 4. Full test suite: 167 passed, 0 failed. Committed.
- [x] 5. README "Window size and position safety" section now documents validation on load
        + display change, size clamping, automatic recovery from disconnected monitors, and
        that negative coordinates are valid.
- [~] 6. Push branch for user testing. (Close-out steps only after user confirms.)

## Last session
2026-08-17 — Steps 1-5 implemented and committed; 167 tests pass. Next: push for user
testing, then close-out (mark central request Done, archive plan, remove plan/, PR).
README change is already in; no further doc surface (no docs/ site in this repo).
