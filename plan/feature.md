# Feature: velopack-update-progress

## Goal
The Velopack update flow reports honest information: the "delta/full" message matches what
Velopack will actually download, and the splash progress bar shows real download progress.

## Source
GitHub issue Tharga/Wpf#48 — "Velopack update: delta count can be misleading, and download
progress is not reported". Verified against current code 2026-08-17.

## Scope
1. **Honest update message** — mirror Velopack's own fallback heuristic (delta count vs
   `UpdateManager.MaximumDeltasBeforeFallback`, summed delta size vs `TargetFullRelease.Size`)
   before formatting, so the message says `(full)` when Velopack will fall back. The version
   shown is always `TargetFullRelease.Version` (fixes the minor issue of reading it from
   `DeltasToTarget.Last()`). Message formatting extracted to a pure testable helper.
2. **Download progress** — `ISplash` gains `SetProgress(int percent)` as a default interface
   method (no-op default, so existing custom `ISplash` implementations keep compiling and
   working). The built-in `Splash` implements it: switches the progress bar from
   indeterminate to determinate and sets the value; `ShowProgress`/`HideProgress` reset to
   indeterminate. `VelopackApplicationUpdateStateService` passes the `Action<int>` callback
   to `DownloadUpdatesAsync`, wired to `_splash?.SetProgress`.
3. **Docs** — README application-update section and `docs/articles/application-update.md`
   updated where they describe the splash/update behaviour.

## Acceptance criteria
- [ ] Message helper covered by tests: single delta, multiple deltas, delta-count fallback,
      delta-size fallback, no deltas, version always from the target full release.
- [ ] `ISplash.SetProgress` exists with a no-op default; built-in `Splash` shows determinate
      percentage during download.
- [ ] `DownloadUpdatesAsync` receives a progress callback.
- [ ] Full test suite passes.
- [ ] Docs updated (README + docs/ article, where applicable).

## Done condition
All acceptance criteria met, branch pushed, user has tested and confirmed, PR opened after
close-out. GitHub issue #48 commented and closed as part of close-out.
