# Release process

The four public packages share the version declared in
`sources/dotnet/AMDevIT.Admob.Wrapper/Directory.Build.props`.

## Preview release gate

Before publishing `0.1.0-preview.1`:

1. Run restore, the async lifecycle tests, and a Release build with a configured
   Java 17 SDK and the required .NET MAUI workloads.
2. Build the MAUI test app on Android and iOS using Google's test ad unit IDs.
3. Exercise banner, interstitial, rewarded, and app-open ads on physical Android
   and iOS devices. Verify success, no-fill/error, dismissal, and repeated load
   cycles.
4. Pack all four projects into one clean output directory.
5. Run `eng/verify-packages.ps1` against that directory. It validates package
   and symbol metadata, critical dependency versions, and a temporary MAUI
   consumer targeting both Android and Windows.
6. Install the generated packages in the real consumer application without any
   project references and repeat its advertising smoke tests.
7. Inspect the CI artifact, release notes, README rendering, dependency groups,
   icon, license, repository commit, and symbols before pushing to NuGet.
8. Tag the exact published commit as `v0.1.0-preview.1`.

Publishing is intentionally manual. CI creates and validates artifacts but does
not hold or use a NuGet API key.

## Stable 0.1.0 gate

Promote the preview to `0.1.0` only after all of the following are true:

- the preview has been used by the real consumer application on both Android
  and iOS without a release-blocking regression;
- full-screen async loading has completed correctly for success, native error,
  cancellation, disposal, and repeated-use scenarios;
- no unresolved restore, build, package-validation, or dependency warnings
  remain;
- the public interfaces and documented setup are accepted as the compatibility
  baseline for the `0.1.x` line;
- breaking changes discovered during preview have been completed and documented.

Then remove `preview.1` from `VersionSuffix`, update the release notes, rerun the
entire preview gate with version `0.1.0`, publish the exact validated artifacts,
and tag the published commit as `v0.1.0`.

After `0.1.0`, preserve binary compatibility within `0.1.x` where practical and
use a new minor version for intentional public API changes.
