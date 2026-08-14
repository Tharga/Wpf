# Plan: Single-instance guard actually guards

- [x] 1. NuGet updates first (cascade tier 1) — internal `Tharga.Runtime` 1.0.1; externals to 10.0.11; SourceLink 10.0.400; Test.Sdk 18.9.0. **Done:** Release build succeeded (12 pre-existing warnings, 0 errors), 145 tests pass (119 Wpf + 26 License). No breakage from the bumps, so feature code starts on current dependencies.
- [~] 2. Fix #50 — `args.PerformDefaultAction = false` in `ApplicationBase.OnStartup` before `Shutdown()`
- [ ] 3. Fix #49 — split mutex name from pipe name in `SingleInstanceService`; `Global\` prefix on the mutex only
- [ ] 4. Fix #49 — ACL on the mutex via `MutexAcl.Create` so a second session can open it rather than throwing
- [ ] 5. Tests — mutex name is machine-scoped, pipe name unchanged, `PerformDefaultAction` cleared when the lock is lost
- [ ] 6. Bump `MAJOR_MINOR` 2.3 → 2.4 (behaviour change, must not ship as a patch)
- [ ] 7. Docs — correct `ApplicationShortName` → `ApplicationFullName`, document machine-wide scope and the `StartupUri` effect
- [ ] 8. Verify — `dotnet build -c Release`, `dotnet test -c Release`
- [ ] 9. Close-out — archive `plan/feature.md`, `git rm -r plan`, final commit, PR
