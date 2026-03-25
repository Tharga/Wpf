# Feature: Test Coverage

## Goal
Add a comprehensive test suite to the Tharga.Wpf and Tharga.License projects, bringing the codebase from zero tests to meaningful coverage of all major features.

## Scope
- Create an xUnit test project `Tharga.Wpf.Tests`
- Create an xUnit test project `Tharga.License.Tests`
- Add both to the solution
- Add a test step to the CI pipeline (`azure-pipelines.yml`)
- Write unit tests for each feature area:
  - **License** (9 source files) — cryptographic signing, fingerprint, validation
  - **Framework** (5 files) — TypeHelper, base classes
  - **ApplicationUpdate** (16 files) — update state, version checking
  - **Dialog** (5 files) — dialog services, view models
  - **ExceptionHandling** (5 files) — exception handler logic
  - **IconTray** (4 files) — icon tray service
  - **TabNavigator** (8 files) — tab management, navigation
  - **WindowLocation** (5 files) — position persistence, restore logic
  - **Root-level** — options, extensions, registration

## Out of Scope
- UI automation / integration tests (WPF rendering)
- Performance or load testing

## Acceptance Criteria
- [ ] `Tharga.Wpf.Tests` project exists with xUnit and is in the solution
- [ ] `Tharga.License.Tests` project exists with xUnit and is in the solution
- [ ] Every public class/method has at least one test
- [ ] All tests pass (`dotnet test -c Release`)
- [ ] CI pipeline runs tests on every build
- [ ] Code coverage is reported (optional stretch goal)

## Done Condition
All acceptance criteria met, all tests green, CI pipeline updated.
