# Feature: Silent Update Check

## Goal
Timer-triggered update checks should be silent — no splash unless an actual update is found. This prevents the splash from popping up every interval and blocking the user.

## Scope
- Add `silent` parameter to `UpdateClientApplication` — timer passes `true`, startup/About pass `false`
- When silent: skip splash before `UpdateAsync`, let subclass show splash only when a download starts
- When not silent: keep current behavior (splash opens immediately)
- Close splash in `finally` if it was opened during the check but no update was downloading (uncomment auto-close)
- Subclasses (Squirrel/Velopack) show splash only when they detect a real update

## Acceptance Criteria
- Timer-triggered check shows no splash when up to date
- Timer-triggered check shows splash when an update is found
- Startup and About menu paths still show splash immediately
- Error case: splash shows with error message and close button
- Splash closes after "no update" when opened from startup path

## Done Condition
- All acceptance criteria met
- All tests pass
- Committed and ready for PR
