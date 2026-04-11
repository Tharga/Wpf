# Plan: Single Instance

## Steps

- [x] 1. Create `SingleInstanceService` with named pipe listener/sender — 6 tests (mutex acquire, release, signal callback)
- [x] 2. Integrate listener into `ApplicationBase.OnStartup` — shows hidden/minimized window on signal via Dispatcher
- [x] 3. Update second instance detection to send "show" via named pipe instead of Win32
- [x] 4. Expose `ReleaseSingleInstanceLock()` on `ApplicationBase`
- [x] 5. Release mutex before restart in Squirrel and Velopack update services
- [x] 6. Sample app already has `AllowMultipleApplications = false` — works out of the box
- [x] 7. Update README with Single Instance section
- [x] 8. Build and test — 0 errors, 126 tests pass
- [x] 9. Commit
