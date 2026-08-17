# Plan: velopack-update-progress

Note: NuGet check at feature start (2026-08-17, same session as display-change-revalidation)
found no available updates, so there are no upgrade steps.

## Steps

- [x] 1. `VelopackUpdateMessage.Build` helper added with 7 tests (single/multiple deltas,
        count fallback incl. boundary, size fallback, no deltas, negative limit).
- [x] 2. `ISplash.SetProgress(int)` added as a default interface method (no-op body, so
        custom implementations keep compiling). Built-in `Splash` implements it —
        determinate bar + value; `ShowProgress`/`HideProgress` reset to indeterminate/0.
- [x] 3. `UpdateAsync` wired. Note: `UpdateManager.MaximumDeltasBeforeFallback` turned out
        to be protected (the issue assumed public), so the service now constructs
        `UpdateOptions { MaximumDeltasBeforeFallback = 10 }` explicitly (10 = Velopack's
        default) and uses the same constant for the message — manager decision and message
        are guaranteed to agree. Version is always `TargetFullRelease.Version`.
- [x] 4. Full test suite: 159 passed, 0 failed. Committed.
- [x] 5. Docs: README "Update behavior", docs/articles/application-update.md (startup-check
        section + custom-ISplash note), docs/articles/splash.md (update-behaviour line).
- [~] 6. Push branch for user testing. (Close-out steps only after user confirms.)

## Last session
2026-08-17 — All implementation steps done and committed; 159 tests pass. Next: push for
user testing, then close-out (comment + close GitHub issue #48, archive plan, remove
plan/, PR). New public API surface (`ISplash.SetProgress` default interface method)
suggests a minor version bump when releasing.
