# Plan: Splash Foreground Color + Discoverable Startup

## Steps

- [x] 1. `SplashData.Foreground` (Brush, init)
- [x] 2. `ThargaWpfOptions.SplashForeground` (Brush, set)
- [x] 3. `ApplicationUpdateStateService.ShowSplashAsync` populates `SplashData.Foreground` from `_options.SplashForeground`
- [x] 4. `Splash` constructor applies the brush to FullName, Environment, Version, ExeLocation, Messages, Client/ClientLocation, ClientSource/ClientSourceLocation, CloseButton
- [x] 5. Added Resource entries for 5 new dark images (colours, fire, mosaik, prism, silicon)
- [x] 6. Added 5 new `SplashImageLibrary` entries (`Colours`, `Fire`, `Mosaik`, `Prism`, `Silicon`)
- [x] 7. Sample uses `Silicon` image + `Brushes.White` foreground
- [x] 8. README — "Foreground color" subsection with light-on-dark example
- [x] 9. README — "Triggering the splash" subsection (3 entry points)
- [x] 10. README — built-in images list split into Light / Dark
- [x] 11. Build + test (0 errors, 145 tests pass)
- [ ] 12. Commit and PR
