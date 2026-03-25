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

## Step 3 — Root-level and Framework tests [~]
- [ ] ViewModelBase: PropertyChanged notification, SetField change detection
- [ ] RelayCommand: Execute, CanExecute, CanExecuteChanged
- [ ] BeforeCloseEventArgs: Cancel property
- [ ] ThargaWpfOptions: property defaults, setters, RegisterExceptionHandler, UseAssembly
- [ ] CloseMode / UpdateSystem enums: defined values
- [ ] CancellationService: Token, CancelAsync
- [ ] StringExtensions: NullIfEmpty (null, empty, whitespace, value)

## Step 4 — Dialog feature tests
- [ ] DialogViewModelBase: OkCommand, CancelCommand, RequestCloseEvent
- [ ] RequestCloseEvent: DialogResult property

## Step 5 — ApplicationUpdate feature tests
- [ ] SplashData: construction and required properties
- [ ] SplashImageLibrary: resource paths
- [ ] UpdateInfoEventArgs, LicenseInfoEventArgs, SplashCompleteEventArgs: construction
- [ ] CloseMethod enum: defined values
- [ ] ApplicationDownloadService: GetApplicationLocationAsync via mocked IHttpClientFactory

## Step 6 — TabNavigator feature tests
- [ ] OpenTabCommand: Execute calls ITabNavigationStateService, CanExecute
- [ ] TabAction enum: defined values

## Step 7 — WindowLocation feature tests
- [ ] Location record: construction, properties, default values
- [ ] MinitorInfo: construction, LocationUpdatedEvent
- [ ] LocationUpdatedEventArgs: construction

## Step 8 — IconTray feature tests
- [ ] IconTrayData: construction, properties
- [ ] TrayMenuItem: construction, properties

## Step 9 — ExceptionHandling feature tests
- [ ] HandleExceptionEventArgs: construction

## Step 10 — Update CI pipeline
- [ ] Add `dotnet test` step to azure-pipelines.yml
- [ ] Verify full build + test passes

## Step 11 — Final verification
- [ ] Run full `dotnet test -c Release` — all green
- [ ] Commit all changes
