# Feature: Splash STA Thread Fix

## Goal
Fix regression in 2.0.12 where the Splash constructor throws "The calling thread must be STA" when constructed from a background thread.

## Scope
- `ApplicationUpdateStateServiceBase.ShowSplashAsync` — dispatch Splash construction through `Application.Current.Dispatcher.Invoke`
- This also addresses the "visual state not reset between uses" request by resetting progress bar and close button state in `ShowSplashAsync`

## Acceptance Criteria
- Splash can be constructed from any thread without throwing STA exception
- Progress bar and close button state are reset each time the splash is shown
- All existing tests pass

## Done Condition
- All acceptance criteria met
- All tests pass
- Committed and ready for review
