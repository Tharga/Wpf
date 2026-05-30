# Plan: OpenTabAsync Thread Fix

## Steps

- [x] 1. Wrap `OpenTabAsync` body — added `OpenTabCoreAsync` private, public method dispatches via `Dispatcher.Invoke(() => OpenTabCoreAsync(...))`
- [x] 2. Same pattern for `CloseTabAsync` and `CloseAllTabsAsync`
- [x] 3. Build and test — 0 errors, 145 tests pass
- [x] 4. Commit and PR
