# NuGet preview readiness — 2026-07-31

## Objective and status

Prepare the four public libraries for a `0.1.0-preview.1` NuGet release and
define a safe promotion path to `0.1.0`.

Status: the local release candidate is built, packed, and validated. Automated
gates are green for the package projects and for a temporary MAUI consumer on
Android and Windows. Publication remains intentionally manual and must follow
physical-device and real-consumer smoke tests.

## Decisions made

- Centralized the four package versions and common NuGet metadata in
  `sources/dotnet/AMDevIT.Admob.Wrapper/Directory.Build.props`.
- Selected `0.1.0-preview.1` as the first public preview. Stable `0.1.0` is not
  produced by default and requires removing `VersionSuffix` after the preview
  gate has passed.
- Kept publishing out of CI. The workflow builds and uploads validated
  artifacts but has no NuGet API key.
- Replaced the incomplete full-screen `LoadAsync` implementations with a shared
  deterministic lifecycle. Native success and failure callbacks now complete
  the returned task; overlapping loads are rejected; disposal completes a
  pending task; caller cancellation remains isolated from the non-cancellable
  native load operation.
- Added `AdLoadException` for MAUI full-screen load failures.
- Changed `IShowableRewardedAdService` to inherit `IShowableAdService` and expose
  `AdRewardEarned`, so the DI-facing rewarded service can be loaded, shown, and
  observed without a platform-specific cast.
- Added direct AndroidX version pins required by the .NET 10 MAUI dependency
  graph. This removed the previous `NU1608` restore warnings.
- Moved build outputs to the repository-level `artifacts` directory to avoid
  Windows `MAX_PATH` failures in Android AAPT2 resource processing.
- Added package validation, symbols, README, icon, repository/source metadata,
  a release checklist, and a CI workflow.
- The package verifier checks all four `.nupkg` and `.snupkg` files, critical
  dependency versions, repository metadata, and compiles a temporary consumer
  against the local packages for both Android and Windows.

## Affected files

- Root release material: `README.md`, `RELEASING.md`, `.github/workflows/ci.yml`,
  and `eng/verify-packages.ps1`.
- Shared package configuration:
  `sources/dotnet/AMDevIT.Admob.Wrapper/Directory.Build.props`.
- Package projects: the Droid, iOSNative, multi-platform wrapper, and MAUICross
  `.csproj` files.
- Async lifecycle:
  `AMDevIT.Admob.Wrapper.MAUICross/Services/BaseFullScreenAdService.cs`, all six
  Android/iOS full-screen service implementations, `AdLoadException.cs`, and
  `IShowableRewardedAdService.cs`.
- Tests: the new `AMDevIT.Admob.Wrapper.MAUICross.Tests` project and the solution
  file.
- Progressive context: this file and `.agents/context.md`.

## Checks performed and results

- Restore completed after the AndroidX pins with no `NU1608` warnings.
- Each of the four packable projects built in Release with zero warnings and
  zero errors.
- The complete solution built in Release with zero errors. It emitted six
  `XA4301` warnings only while packaging the Android demo applications; the
  warnings report duplicate `libdatastore_shared_counter.so` entries.
- Seven async lifecycle tests passed: success, native failure, cancellation
  isolation, concurrent-load rejection, synchronous-start failure and retry,
  disposal, and load-then-show sequencing.
- NuGet package validation succeeded while packing all four projects.
- Four `.nupkg` and four `.snupkg` files were generated under
  `artifacts/packages` for `0.1.0-preview.1`.
- `eng/verify-packages.ps1` passed. Its temporary MAUI consumer restored and
  built `net10.0-android` and `net10.0-windows10.0.26100.0` with zero warnings
  and zero errors using the local MAUICross package.
- `dotnet msbuild -getProperty:Version` returned `0.1.0-preview.1`; CI uses this
  value instead of duplicating the version.
- `git diff --check` passed; Git reported only expected LF-to-CRLF conversion
  notices for the Windows worktree.

## Open issues and recommended next step

1. Install these exact local packages in the existing real MAUI consumer, with
   project references removed, and repeat Android and iOS advertising flows.
2. On physical Android and iOS devices, test banner, interstitial, rewarded,
   and app-open success, no-fill/error, dismissal, reward, and repeated-load
   scenarios using Google's test IDs.
3. Run the new GitHub Actions workflow on the release commit and inspect its
   uploaded package artifacts.
4. Investigate the demo-app-only `XA4301` duplicate native-library warnings
   before stable `0.1.0`. They do not occur in the packable projects or in the
   Android/Windows package consumer class-library smoke test, but stable policy
   requires no unresolved build warnings.
5. After the manual preview gate passes, publish the exact validated artifacts
   and tag the commit `v0.1.0-preview.1`.
6. Promote to `0.1.0` only after preview soak time in the real consumer, API
   acceptance, resolution of the demo warnings, updated release notes/version
   examples, and a complete rerun of the release gate.

