# Plan: Test Coverage

## Step 1 — Create test projects and solution setup ✓
- [x] Create `Tharga.License.Tests` xUnit project (targets net10.0)
- [x] Create `Tharga.Wpf.Tests` xUnit project (targets net10.0-windows, UseWPF)
- [x] Add both to the solution with project references
- [x] Add xUnit, Moq, coverlet NuGet packages
- [x] Add InternalsVisibleTo for Tharga.License.Tests in Tharga.License.csproj
- [x] Verify `dotnet build -c Release` passes

## Step 2 — Tharga.License tests (pure logic, no WPF) ✓
- [x] SigningService: key pair generation, sign, verify, tampered data, wrong key, unicode, empty string (8 tests)
- [x] RsaPublicKey: FromParameters/ToParameters round-trip, base64 storage, record equality (3 tests)
- [x] RsaPrivateKey: FromParameters/ToParameters round-trip, all components, record equality (3 tests)
- [x] RsaKeyPair: construction, record equality (2 tests)
- [x] Fingerprint: construction, equality, inequality (3 tests)
- [x] LicenseCheckRequest: construction, record equality (2 tests)
- [x] LicenseCheckResponse: construction, record equality (2 tests)
- [x] ThargaLicenseRegistration: DI resolves ISigningService, transient lifetime (2 tests)
- Total: 26 tests, all passing

## Step 3 — Root-level and Framework tests ✓
- [x] ViewModelBase: PropertyChanged notification, SetField change detection, no-subscriber safety (6 tests)
- [x] RelayCommand: Execute, CanExecute with/without predicate (5 tests)
- [x] BeforeCloseEventArgs: Cancel default and setter (2 tests)
- [x] ThargaWpfOptions: defaults, setters, RegisterExceptionHandler, UseAssembly (10 tests)
- [x] CloseMode / UpdateSystem enums: values and member count (4 tests)
- [x] CancellationService: Token initial state, CancelAsync (3 tests)
- [x] StringExtensions: NullIfEmpty null/empty/value/whitespace (4 tests)
- Total: 34 tests, all passing

## Step 4 — Dialog feature tests ✓
- [x] DialogViewModelBase: OkCommand, CancelCommand, RequestCloseEvent, no-subscriber safety (7 tests)
- [x] RequestCloseEvent: DialogResult true/false (2 tests)
- Total: 9 tests, all passing

## Step 5 — ApplicationUpdate feature tests ✓
- [x] SplashData: construction, defaults, all properties (3 tests)
- [x] SplashImageLibrary: all 10 image paths are valid pack URIs, not null (11 tests)
- [x] UpdateInfoEventArgs, LicenseInfoEventArgs, SplashCompleteEventArgs (5 tests)
- [x] CloseMethod enum: values and count (2 tests)
- Total: 21 tests, all passing

## Step 6 — TabNavigator feature tests ✓
- [x] OpenTabCommand: CanExecute default, with true/false predicates (3 tests)
- [x] TabAction enum: values and count (2 tests)
- [x] EDocumentPreset enum: count (1 test)
- Total: 6 tests, all passing

## Step 7 — WindowLocation feature tests ✓
- [x] Location: defaults, all properties, metadata (3 tests)
- [x] LocationUpdatedEventArgs: location only, exception only, both (3 tests)
- [x] MinitorInfo: FileLocation, LoadLocation, event raised, no-subscriber safety (4 tests)
- Total: 10 tests, all passing

## Step 8 — IconTray feature tests ✓
- [x] IconTrayData: defaults, menu items (2 tests)
- [x] TrayMenuItem: defaults, text, action invocation (3 tests)
- Total: 5 tests, all passing

## Step 9 — ExceptionHandling feature tests ✓
- [x] HandleExceptionEventArgs: construction, type preservation (2 tests)
- Total: 2 tests, all passing

## Step 10 — Update CI pipeline ✓
- [x] Add `dotnet test` step to azure-pipelines.yml (between Build and Pack)
- [x] Verify `dotnet test -c Release --no-build` passes

## Step 11 — Final verification ✓
- [x] Full `dotnet test -c Release` — 120 tests, all green
- [x] All changes committed

## Grand Total: 120 tests (26 License + 94 Wpf), all passing
