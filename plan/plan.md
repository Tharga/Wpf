# Plan: Pipe ErrorId-displayed exceptions to ILogger.LogError

## Steps

- [x] 1. Move `correlationId` generation and `LogError` call to the top of `FallbackHandlerInternalAsync`
- [x] 2. Removed duplicate `LogError` call in fallback path; also removed redundant `LogCritical` in the no-main-window branch (top-level log already covers it)
- [x] 3. Build and test — 0 errors, 146 tests pass
- [x] 4. Commit and PR
