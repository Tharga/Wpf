# Plan: Update Restart Fixes

## Steps

- [x] 1. Add `BeforeRestartAsync()` to base class — dispatches CloseAllTabsAsync to UI thread, releases single-instance lock
- [x] 2. Squirrel uses `BeforeRestartAsync()` — fixes thread bug
- [x] 3. Velopack uses `BeforeRestartAsync()` — now also closes tabs (was missing)
- [x] 4. Defensive splash cleanup in catch — try/catch each call, opens splash if not yet shown (silent path), shows close button and Show()
- [x] 5. Final guard in finally — on exception, force ShowCloseButton + Show; otherwise auto-close after delay (existing)
- [x] 6. Build and test — 146 tests pass
- [x] 7. Commit and PR
