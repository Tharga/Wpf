# Feature: Splash stays open during update

## Goal
Keep the splash screen open and visible (with no close button) while an application update is downloading and installing, so the user knows an update is in progress.

## Originating Branch
`develop`

## Request
From Florida (2026-04-06), tracked in `Requests.md` under "Tharga.Wpf".

## Scope
- Add `HideCloseButton()` to `ISplash` interface and `Splash` implementation
- Track update-in-progress state in `ApplicationUpdateStateServiceBase`
- When update starts: show splash, hide close button, show "Updating..." message
- Prevent splash auto-close while update is in progress
- When update completes or fails: restore normal splash behavior
- Write tests for new logic

## Out of Scope
- Progress bar / percentage indicator (future enhancement)
- Spinner animation

## Acceptance Criteria
- [ ] `ISplash` has `HideCloseButton()` method
- [ ] Splash stays open during download/install with close button hidden
- [ ] Splash shows "Updating..." type messages during update
- [ ] Splash auto-close is blocked while update is in progress
- [ ] On error, close button reappears and error message is shown (existing behavior preserved)
- [ ] Tests pass
- [ ] Build succeeds

## Done Condition
All acceptance criteria met, all tests green.
