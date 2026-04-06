# Feature: GitHub Actions CI/CD

## Goal
Add a GitHub Actions workflow for build, test, security scanning, and NuGet publishing — following the Tharga.Platform pattern.

## Scope
- `.github/workflows/build.yml` with build, security, release, and prerelease jobs
- Uses `windows-latest` runner (required for WPF)
- Packs `Tharga.Wpf` and `Tharga.License`
- Version scheme: `2.0.x` (continuing from existing tags)
- Warning threshold: 25

## Acceptance Criteria
- Workflow builds and tests successfully
- NuGet packages are produced for both projects
- Release job triggers on push to master
- Prerelease job triggers on PRs to master
- CodeQL security scanning runs

## Done Condition
- Workflow file committed and ready for review
