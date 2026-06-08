# Plan: Icon, docs site, and NuGet upgrades

- [x] 1. Create feature branch `feature/icon-and-docs` from `master`
- [x] 2. Create `plan/feature.md` and `plan/plan.md`
- [x] 3. Update `<PackageIconUrl>` in `Tharga.Wpf.csproj` and `Tharga.License.csproj` → `https://thargelion.net/assets/component-wpf.png`
- [x] 4. Bump NuGet: Tharga.Runtime 0.1.12 → 0.1.14, Velopack 0.0.1298 → 1.2.0, Microsoft.NET.Test.Sdk 18.5.1 → 18.6.0 (both Tests projects)
- [x] 5. `dotnet build -c Release` — clean, 0 errors. Velopack 1.x introduced no breaking changes we touch.
- [x] 6. `dotnet test -c Release` — 145/145 pass (26 License + 119 Wpf)
- [x] 7. Scaffold `docs/` (docfx.json with both csprojs as metadata source, CNAME=wpf.tharga.net, index.md, toc.yml)
- [x] 8. Write 9 articles: getting-started, application-base, single-instance, window-management, tab-navigator, splash, application-update, exception-handling, license-server (+ articles/index.md and articles/toc.yml)
- [x] 9. Add `docs/templates/thg/public/main.css` and `docs/templates/thg/layout/_master.tmpl` (mirror Test — option (b) absolute-URL logo overlay)
- [x] 10. Update `README.md` with `**Docs:** [wpf.tharga.net](https://wpf.tharga.net)` line near top
- [x] 11. Update `.gitignore` — exclude `docs/_site/` and `docs/api/`
- [x] 12. Add `pages: write` + `id-token: write` to `.github/workflows/build.yml` permissions
- [x] 13. Append `docs` (needs: release, **windows-latest** because Tharga.Wpf is `net10.0-windows`) and `docs-deploy` jobs to `build.yml`
- [x] 14. `docfx docs/docfx.json` — 0 errors, 2 warnings (one pre-existing CS0693, one benign multi-target "no Compile target" on License which still processed)
- [x] 15. Fix duplicate `<inheritdoc />` on `OnExit` in `ApplicationBase.cs` (caught by docfx)
- [x] 16. Final `dotnet build -c Release` + `dotnet test -c Release` green (145/145)
- [x] 17. Commit all changes (commit `c140f53`)
- [~] 18. Bump `MAJOR_MINOR` in `build.yml` from `2.1` to `2.2` — Velopack 0.x → 1.x is a transitive breaking change for consumers that reference Velopack directly. First release will be `2.2.0`.
- [ ] 19. Push branch and open PR to `master`

## Last session
- All implementation steps complete on `feature/icon-and-docs`.
- Velopack 1.x → no API changes needed in our code (tests still pass).
- Docs job runs on `windows-latest` because DocFX needs WPF assemblies to extract metadata from `Tharga.Wpf.csproj` (`net10.0-windows`); `docs-deploy` stays on `ubuntu-latest`.
- DocFX generated 71 API .yml files (Tharga.License + Tharga.Wpf surface) — `docs/api/` is gitignored.
- Manual follow-up (out of scope for the code commit): DNS `CNAME wpf.tharga.net → tharga.github.io`; Pages source = "GitHub Actions"; `github-pages` environment.

## README impact when feature closes
- `**Docs:** [wpf.tharga.net](https://wpf.tharga.net)` line added — already in this commit.
