# Feature: Splash Thread Safety

## Goal
Make `Splash` window methods safe to call from any thread by dispatching all UI operations through `Application.Current.Dispatcher`.

## Scope
- `Splash.xaml.cs` — wrap `Close()`, `Hide()`, `Show()`, `SetErrorMessage()`, `ShowCloseButton()`, `ClearMessages()`, and `IsCloseButtonVisible` in dispatcher calls
- `ApplicationUpdateStateServiceBase` — remove redundant `Dispatcher.Invoke` wrappers now that the Splash itself is thread-safe
- Tests for the dispatcher-safe behavior

## Acceptance Criteria
- Calling `ISplash.Close()` from a background thread does not throw
- Calling `ISplash.Hide()` from a background thread does not throw
- All existing tests pass
- The redundant dispatcher wrappers in `ApplicationUpdateStateServiceBase` are cleaned up

## Done Condition
- All acceptance criteria met
- All tests pass
- Committed and ready for review
