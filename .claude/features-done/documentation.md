# Feature: Documentation

## Goal
Improve API documentation and project metadata so that consumers of the NuGet packages get good IntelliSense, the project is discoverable, and there is a clear change history.

## Scope
- Add XML doc comments (`/// <summary>`) to all public types and members in Tharga.Wpf
- Add XML doc comments to all public types and members in Tharga.License
- Remove `<NoWarn>CS1591</NoWarn>` so missing docs trigger build warnings
- Create a CHANGELOG.md with history derived from git log
- Complete NuGet metadata in both .csproj files:
  - `PackageTags`
  - `PackageReleaseNotes`
  - `PackageLicenseExpression` (MIT)
- Improve `Tharga.License/README.md` beyond its current 3 lines

## Out of Scope
- Generating a static documentation site (e.g. DocFX)
- Writing a full architecture guide
- Modifying the main README.md (already in good shape)

## Acceptance Criteria
- [ ] Zero CS1591 warnings when `<NoWarn>CS1591</NoWarn>` is removed
- [ ] All public types and members in Tharga.Wpf have XML doc comments
- [ ] All public types and members in Tharga.License have XML doc comments
- [ ] CHANGELOG.md exists with version history
- [ ] Both .csproj files have PackageTags, PackageLicenseExpression
- [ ] Tharga.License/README.md is expanded with usage information
- [ ] Build succeeds cleanly (`dotnet build -c Release`)

## Done Condition
All acceptance criteria met, build clean with no doc warnings, NuGet metadata complete.
