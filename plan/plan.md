# Plan: Splash stays open during update

## Step 1 — Add HideCloseButton to ISplash and Splash ✓
- [x] Add `HideCloseButton()` to ISplash interface
- [x] Implement in Splash.xaml.cs

## Step 2 — Track update state and keep splash open ✓
- [x] Add `_isUpdating` flag to ApplicationUpdateStateServiceBase
- [x] Set flag true when update starts downloading, false in finally block
- [x] In ShowSplashAsync: don't auto-close while `_isUpdating` is true (double-checked before and after delay)
- [x] In UpdateClientApplication: show splash and hide close button when update starts
- [x] On error: show close button (existing behavior preserved)

## Step 3 — Tests ✓
- [x] Build succeeds (0 errors)
- [x] All 120 tests pass

## Step 4 — Final verification ✓
- [x] `dotnet build -c Release` — clean
- [x] `dotnet test -c Release` — all pass
- [x] Commit
