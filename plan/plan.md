# Plan: Splash Thread Safety

## Steps

- [x] 1. Make all ISplash methods in Splash.xaml.cs dispatcher-safe — added DispatchIfRequired helper, wrapped Close/Hide/Show/SetErrorMessage/ShowCloseButton/ClearMessages/IsCloseButtonVisible
- [x] 2. Clean up redundant Dispatcher.Invoke wrappers in ApplicationUpdateStateServiceBase — removed 3 wrapper blocks, removed unused Application alias
- [x] 3. Run tests and verify build — 0 errors, 120 tests pass
- [~] 4. Commit
