## Pending

### Splash.Close() crashes when called from background thread
- **From:** Florida
- **Date:** 2026-04-05
- **Priority:** High
- **Description:** `ISplash.Close()` calls `Window.Close()` directly without marshalling to the UI thread. When `SquirrelApplicationUpdateStateService.UpdateAsync` runs on a background thread and calls `CloseSplash()`, it throws `"The calling thread cannot access this object because a different thread owns it"`. The same flow works when triggered from the menu because the menu runs on the UI thread. Fix: dispatch `Close()` (and likely `Hide()`) through `Application.Current.Dispatcher.Invoke` in the `Splash` implementation so it's safe to call from any thread.
- **Stack trace:**
  ```
  SquirrelApplicationUpdateStateService.UpdateAsync
    → ApplicationUpdateStateServiceBase.ShowSplashWithRetryAsync
      → ApplicationUpdateStateServiceBase.CloseSplash
        → ISplash.Close()
          → Window.Close()  ← wrong thread
  ```
- **Status:** Pending
