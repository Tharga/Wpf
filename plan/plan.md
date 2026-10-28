# Plan: require LicensePublicKey (3.0)

Branch: `fix/require-license-public-key`

## Steps
- [x] NuGet updates: `dotnet outdated` found nothing to update.
- [x] `LicenseClient`: removed `GetPublicKeyAsync` and the `license/key` download; the key is resolved before any request and a missing key returns `(false, NoPublicKeyMessage)`. A delegate returning null used to throw; it now reports not valid too.
- [x] `ThargaWpfOptions.LicensePublicKey` XML doc: required with `LicenseServerLocation`.
- [x] `MAJOR_MINOR` 2.6 → 3.0 in `build.yml`.
- [x] Tests: no key configured (option unset, delegate returns null) → not valid, no HTTP request; response signed by a key the server publishes instead of the configured one → "Invalid signature.", `license/key` never requested. 214 tests green.
- [x] Docs: README *License Server Check* and `docs/articles/license-server.md` (key required, upgrade note, corrected server-side section).
- [~] User review, then close-out: archive `feature.md`, remove `plan/`, open PR.
- [ ] Merge → 3.0.0 released.
- [ ] Advisory GHSA-6q43-4qx5-7gqw: range `<= 2.6.0`, patched `3.0.0`, note 2.5.6 and 2.6.0 fixes, publish.

## Notes
- Consumer found: Eplicta Conserver (client on Tharga.Wpf 2.5.0). Its `LicenseController` builds a new key pair per process start (`_keyPair ??= BuildKeyPair()`), so upgrading it to 3.0 needs a stored key pair on the server and `LicensePublicKey` in the client.

## Last session
2026-10-28: implementation, tests and docs done on the branch. Next: user review, then push, close-out and PR.
