# Feature: require LicensePublicKey (3.0)

## Goal
Close advisory GHSA-6q43-4qx5-7gqw: the license check must never trust a public key fetched from the server it verifies.

## Background
- 2.5.6: the response must return the request's application name and machine fingerprint.
- 2.6.0: optional `LicensePublicKey` pinning; without it the client still downloaded the key from `{address}/license/key`.

## Scope
- `LicensePublicKey` required whenever `LicenseServerLocation` is set.
- Remove the `license/key` fallback in `LicenseClient`.
- No key configured (option not set, or delegate returns null): report not valid with a clear message, without calling the server.
- Release as the next major version, 3.0 (`MAJOR_MINOR` in `build.yml`).
- Tests in `LicenseClientTests`: no key configured; a response signed by a different key is rejected.
- README and `docs/articles/license-server.md`: key required, upgrade note.
- Decide whether a server-side `license/key` endpoint in Tharga.License stays.

## Decision: server-side license/key
`Tharga.License` has no endpoint; it ships only the request/response models and `ISigningService`. Each server implements its own routes (e.g. Eplicta Conserver's `LicenseController`). Nothing to remove from the package. The docs say a server may keep `license/key` to hand out the key, but the client never calls it.

## Acceptance criteria
- `LicenseClient` has no code path that requests a key from the server.
- Without a configured key, `CheckLicenseAsync` returns not valid with `NoPublicKeyMessage` and makes no HTTP request.
- A response signed by any key other than the configured one is rejected, and `license/key` is never requested.
- Full test suite green.
- README and docs describe the key as required and include an upgrade note.

## After the release
- Advisory: affected range `<= 2.6.0`, patched `3.0.0`, description notes what 2.5.6 and 2.6.0 fixed, then publish.

## Done when
3.0.0 is on nuget.org and the advisory is published.
