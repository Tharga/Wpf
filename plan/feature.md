# Feature: display-change-revalidation

## Goal
Window locations are revalidated when the display setup changes, so windows on a
disconnected monitor are recovered automatically instead of being stranded off-screen.

## Source
Central request "Window locations are not revalidated when the display setup changes"
(`$DOC_ROOT/Tharga/Requests.md` → Tharga.Wpf, filed 2026-08-10, Priority Medium).
Verified against current code 2026-08-17 — all defects still present.

## Scope
1. **Revalidate on display change** — `WindowLocationService.MonitorEngine` subscribes to
   `SystemEvents.DisplaySettingsChanged`, debounced (750 ms, each new event restarts the
   wait), re-runs `LocationValidator.Validate` against the fresh screen set and applies the
   result. Non-`Normal` window states are skipped (Windows re-places maximized/minimized
   windows itself). Handler is detached when the window closes (SystemEvents is static —
   a leaked handler keeps the window alive).
2. **`Validate` clamps size, not just position** — width/height are clamped to the rescue
   screen's work area before centring, so the result is always fully on-screen and
   non-negative (`LocationValidator.cs`).
3. **`Validate` rescues to the primary screen** — not `screens[0]`, which is not necessarily
   the primary. `ScreenBounds` gains an `IsPrimary` flag (defaulted, source-compatible).
4. **`SetLocation` saves negative coordinates** — a monitor left of / above the primary has
   legitimately negative Left/Top. The `>= 0` guard is replaced by a window-state guard:
   position/size are captured only while the window is in `Normal` state (which also covers
   the minimized −32000 sentinel the old guard incidentally caught). Validation on load and
   on display change is what rejects unreachable positions.
5. **Metadata round-trip fix** — `LoadLastLocation` parses metadata from `data[6]` (Height)
   instead of `data[7]`; fix the index.
6. **README** — note in the window-location section that saved positions are validated both
   on load and whenever the display setup changes, and that windows on a disconnected
   monitor are recovered automatically.

## Acceptance criteria
- [ ] `LocationValidator.Validate` clamps width/height to the rescue screen's work area;
      result is fully on-screen with non-negative offsets relative to that screen.
- [ ] `Validate` prefers the primary screen as the rescue target.
- [ ] Negative Left/Top from a real multi-monitor layout survive a save/load round-trip.
- [ ] Metadata survives a save/load round-trip.
- [ ] `MonitorEngine` revalidates all monitored `Normal`-state windows on
      `DisplaySettingsChanged`, debounced; handler detached on window close.
- [ ] Tests cover the validator changes, the save-format round-trip, and the debounce logic.
- [ ] Full test suite passes.
- [ ] README updated.

## Done condition
All acceptance criteria met, branch pushed, user has tested and confirmed, PR opened after
close-out per the feature workflow. Central request marked Done with evidence; the consumer
workaround (their app-level display-change guard) can then be deleted by the reporter.
