# Feature: OpenTabAsync Thread Fix

## Goal
Fix the cross-thread `InvalidOperationException` thrown when `OpenTabAsync` reads `ContentControl.Content` off the UI thread. 82× per 7-day window in Florida 4.2.45 production.

## Root cause
`TabNavigationStateService.OpenTabAsync` line 28:
```csharp
var existing = TabItems.Where(x => x.Content.GetType() == typeof(TTabView)).ToArray();
```
`x.Content` (ContentControl.Content) is UI-thread-only. When `OpenTabComamnd<T>.Execute` runs its `async void` body on a worker thread (continuation after an await loses the dispatcher context), this throws.

Same UI-thread access happens at:
- Line 33: `(TTabView)item.Content`
- Line 38, 76: `item.Focus()`
- Line 60: `new TabItem { Content = tabContent }`
- Line 75: `TabItems.Add(tabItem)`
- Line 104: `TabItems.ToArray()` in `CloseAllTabsAsync` (already fixed by `BeforeRestartAsync` for that caller)

## Scope

### Make `OpenTabAsync` dispatcher-safe internally
Wrap the body in `Application.Current.Dispatcher.InvokeAsync(...)` so the WPF work always runs on the UI thread, regardless of which thread the caller is on. Mirrors the `BeforeRestartAsync` pattern.

This is preferred over fixing only `OpenTabComamnd.Execute` because:
- Centralizes the dispatcher logic in the service (single point of truth)
- Covers any consumer who calls `OpenTabAsync` directly (not just via `OpenTabComamnd`)
- The service is internal-only, so this change has no public API surface

### Also dispatch `CloseTabAsync` and `CloseAllTabsAsync`
Same pattern — they touch `TabItems` (ObservableCollection of DependencyObject) and `tabView.OnCloseAsync()`. `BeforeRestartAsync` already dispatches `CloseAllTabsAsync`, but other consumers could call these directly too.

## Acceptance Criteria
- `OpenTabAsync` can be called from any thread without throwing the cross-thread exception
- `CloseTabAsync` and `CloseAllTabsAsync` are equally safe
- All existing tests pass
- Florida sees no more `<OpenTabAsync>b__7_0` cross-thread crashes after upgrading

## Done Condition
- All acceptance criteria met
- Tests pass
- PR created
