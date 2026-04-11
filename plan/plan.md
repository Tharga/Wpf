# Plan: Window Lifecycle Management

## Steps

- [x] 1. Add `StartupWindowState` enum and `HideOnClose` property to `ThargaWpfOptions`
- [x] 2. Create `CloseActionResolver` — pure logic for close mode decisions, with 7 tests
- [x] 3. Create `LocationValidator` — validates saved size/position against screen bounds, with 13 tests
- [x] 4. Add `ApplicationBase.Hide()` and integrate `CloseActionResolver` into `ApplicationBase.Close()`
- [x] 5. Integrate `LocationValidator` into `WindowLocationService.OnLoaded` — fix zero-size and off-screen restore
- [x] 6. Integrate `StartupWindowState` into `WindowLocationService.OnLoaded` — restore visibility and window state from options
- [x] 7. Framework handles X button hide via Closing event in MonitorEngine when HideOnClose=true — no OnClosing boilerplate needed
- [x] 8. Update sample app — uses HideOnClose=true, simplified OnClosing, added Hide/Exit/Force Exit buttons
- [x] 9. Update README with Window Management section
- [x] 10. Full build and test suite — 0 errors, 140 tests pass
- [x] 11. Commit
