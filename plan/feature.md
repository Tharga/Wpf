# Feature: Prerelease Version Fix

## Goal
Fix the latent bug in `.github/workflows/build.yml` where the prerelease job pushes stable-versioned artifacts to NuGet, instead of pre-release versioned ones.

## Background
Raised by Tharga.Mcp. The build job's `Pack` step always stamps the stable version into the `.nupkg`. On a PR run, the `prerelease` job downloads that artifact and pushes it to NuGet — publishing the stable version from the PR under a GitHub Release tagged as pre-release. `--skip-duplicate` means whichever PR runs first mints that stable patch on NuGet before the PR merges.

Not damaging yet (the `prerelease` environment requires manual approval; hasn't fired anywhere) but worth fixing before it does.

## Scope
Replace the single `Pack` step in the build job with two conditional steps:
- **Pack stable** — when `github.ref == 'refs/heads/master' && github.event_name == 'push'`, uses `steps.version.outputs.version`
- **Pack prerelease** — when `github.event_name == 'pull_request'`, uses `steps.preversion.outputs.version`

Reference: `Tharga.Mcp feature/prerelease-version-fix`.

## Acceptance Criteria
- Push to master packs and publishes the stable version
- PR run packs and publishes the pre-release version
- Existing release flow continues to work

## Done Condition
- Workflow updated
- Committed and ready for PR
