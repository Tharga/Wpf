# Plan: Splash STA Thread Fix

## Steps

- [x] 1. Dispatch Splash construction through Dispatcher.Invoke in ShowSplashAsync
- [x] 2. Reset visual state (progress bar, close button) when re-showing the splash — HideProgress() always, ShowCloseButton/HideCloseButton based on parameter
- [x] 3. Build and run tests — 0 errors, 120 tests pass
- [x] 4. Commit
