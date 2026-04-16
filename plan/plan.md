# Plan: Silent Update Check

## Steps

- [x] 1. Add `silent` parameter to `UpdateClientApplication` — skip splash, skip UpdateInfo events when silent
- [x] 2. Timer passes `silent: true`
- [x] 3. Subclasses already show splash only when update found (line 51 in Squirrel, similar in Velopack) — no changes needed
- [x] 4. Uncommented auto-close in `finally` for non-silent path — 2s delay then CloseSplash if no close button visible
- [x] 5. Build and test — 0 errors, 146 tests pass
- [x] 6. Commit
