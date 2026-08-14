# Plan: Single-instance guard actually guards

- [x] 1. NuGet updates first (cascade tier 1) — internal `Tharga.Runtime` 1.0.1; externals to 10.0.11; SourceLink 10.0.400; Test.Sdk 18.9.0. **Done:** Release build succeeded (12 pre-existing warnings, 0 errors), 145 tests pass (119 Wpf + 26 License). No breakage from the bumps, so feature code starts on current dependencies.
- [x] 2. Fix #50 — **the fix suggested in the issue does not compile.** `StartupEventArgs.PerformDefaultAction` is `internal` to PresentationFramework, so a consumer cannot clear it. Used the public equivalent instead: `StartupUri = null`, which is the condition `DoStartup()` actually tests before calling `LoadComponent`. Same effect, no reflection.
- [x] 3. Fix #49 — split `_mutexName` from `_pipeName` (they were one field). Mutex now built by `BuildMutexName` with the `Global\` prefix; pipe still built by `BuildPipeName` with no prefix, since named pipes are already machine-wide and prefixing would break signalling.
- [x] 4. Fix #49 — `MutexAcl.Create` with a `MutexSecurity` granting `Synchronize | Modify` to `WorldSid`, so a second session opens the mutex instead of throwing `UnauthorizedAccessException`. `UnauthorizedAccessException` is still caught and treated as "another instance holds it" rather than "first instance", which is the fail-safe direction. **No package reference needed** — `MutexAcl` resolves from net10.0-windows.
- [x] 5. Tests — 7 added, taking Wpf from 119 to 126. They assert the *old* values explicitly (`NotEqual("Tharga.Wpf.Whatever")`), so they fail against the previous code rather than passing vacuously. Two go beyond string checks: one opens the mutex by its `Global\` name via `TryOpenExisting`, and one proves the unprefixed name is a genuinely different kernel object.
- [x] 6. Bump `MAJOR_MINOR` 2.3 → 2.4 — ships as **2.4.0**. The tag-lookup `|| true` guard was already present in all three sites, so starting a new series is safe here. Workflow re-validated with the duplicate-key YAML loader.
- [x] 7. Docs — corrected `ApplicationShortName` → `ApplicationFullName` in `docs/articles/single-instance.md`; added a "Scope of the lock" section and 2.4.0 change notes for both fixes; updated the `README.md` Single Instance section. Both doc surfaces done, per the Feature Workflow.
- [x] 8. Verify — `dotnet build -c Release --no-incremental` succeeded (12 pre-existing warnings, 0 errors); `dotnet test -c Release` 152 passing, 0 skipped, 0 failed.
- [~] 9. Close-out — archive `plan/feature.md`, `git rm -r plan`, final commit, PR

## Still needed from outside this branch

- **Florida must re-verify on real hardware.** Neither fix is fully provable from a unit test: #49 needs two concurrent Windows sessions, #50 needs a real `StartupUri` and a window constructor with observable side effects (Florida's creates a tray icon).
- The `Requests.md` Wpf entry ("Window locations are not revalidated when the display setup changes") is a **separate** issue and untouched here. It cites 2.2.0 while the repo now ships 2.3.0, so it needs verifying independently.
- `Wpf.md` claims "Requests → Tharga.Wpf (0 pending)", which is stale — there is one open request. Correcting that is a backlog edit, not part of this branch.
