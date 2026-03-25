# Feature: Test Coverage

## Goal
Add a comprehensive test suite to Tharga.Wpf and Tharga.License, bringing the codebase from zero tests to meaningful coverage of all testable code.

## Originating Branch
`develop`

## Scope
- Create xUnit test projects for both Tharga.Wpf and Tharga.License
- Use Moq for mocking (already configured via InternalsVisibleTo for DynamicProxyGenAssembly2)
- Write unit tests for all testable public and internal types
- Update CI pipeline to run tests
- Focus on logic/behavior tests — skip pure WPF UI rendering tests

## Out of Scope
- UI automation / integration tests
- Performance testing
- Code coverage tooling (stretch goal)

## Acceptance Criteria
- [ ] `Tharga.Wpf.Tests` project exists with xUnit and is in the solution
- [ ] `Tharga.License.Tests` project exists with xUnit and is in the solution
- [ ] Every testable public class/method has at least one test
- [ ] All tests pass (`dotnet test -c Release`)
- [ ] CI pipeline runs tests on every build

## Done Condition
All acceptance criteria met, all tests green, CI pipeline updated.
