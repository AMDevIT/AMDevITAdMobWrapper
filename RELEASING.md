# Release process

The release being prepared is **0.2.0**. Its changes are recorded in
[CHANGELOG.md](CHANGELOG.md); publication is manual.

## Version and release metadata

The four public NuGet packages share `VersionPrefix`, `VersionSuffix`, and
`PackageReleaseNotes` from
`sources/dotnet/AMDevIT.Admob.Wrapper/Directory.Build.props`.
Keep the following aligned before preparing artifacts:

- All four package versions, with no project-local release-note/version overrides.
- Android native module `version` in `admob-wrapper/build.gradle.kts`.
- iOS `MARKETING_VERSION` and an incremented `CURRENT_PROJECT_VERSION` in the
  Xcode project. For 0.2.0 these are `0.2.0` and build `2`.
- README package examples, SDK/dependency descriptions, and changelog entry.

A patch increments the last component (for example, 0.2.0 to 0.2.1). Use a minor
increment for a broader SDK/dependency or capability update. Version 0.2.0 marks
this SDK/dependency and release-tooling update from the 0.1.x line; it does not
declare a 1.0 compatibility baseline. Keep the suffix empty for the stable
candidate, or set a prerelease suffix explicitly and update examples accordingly.

## Native and macOS preparation

Prerequisites include .NET 10, JDK 21, Android SDK, and Xcode with iOS SDKs.
The tools check Xcode availability without comparing its version.

From the repository root:

```bash
bash sources/tools/prepare-nuget.sh
```

This rebuilds the native Android/iOS wrappers, replaces the binding binaries,
runs Kotlin tests, restores/builds Release, and prepares four `.nupkg` and four
`.snupkg` files under `artifacts/packages/0.2.0/`. It validates package metadata,
dependencies, symbols, and platform assemblies. See the
[tooling guide](sources/tools/README.md) for prerequisites and options.

Rerun native preparation after changing the native version metadata. A successful
0.1.40 build cannot be reused as proof that the embedded framework reports 0.2.0.
Review and transfer the rebuilt AAR/XCFramework with the corresponding source
changes before preparing the Windows-inclusive release artifact.

On macOS the MAUI package contains Android, iOS, and Mac Catalyst. Its Windows
target is conditional on the host. If publishing the package with Windows support,
use the Windows-produced package as the final artifact. Do not publish both host
variants under the same package ID/version.

## Windows checks and packaging

The manual GitHub Actions workflow builds/checks the .NET libraries and prepares
Windows-inclusive packages from the native binaries checked into that commit.
It does not rebuild the native Swift/Android wrappers or publish to NuGet.
Run it only after updating the native binaries for the release.

For local validation, from the C# solution directory:

```powershell
dotnet restore AMDevIT.Admob.Wrapper.slnx
dotnet build AMDevIT.Admob.Wrapper.slnx --configuration Release --no-restore
dotnet restore AMDevIT.Admob.Wrapper.MAUICross.Tests/AMDevIT.Admob.Wrapper.MAUICross.Tests.csproj
dotnet build AMDevIT.Admob.Wrapper.MAUICross.Tests/AMDevIT.Admob.Wrapper.MAUICross.Tests.csproj --configuration Release --no-restore
dotnet vstest AMDevIT.Admob.Wrapper.MAUICross.Tests/bin/Release/net10.0-windows10.0.26100.0/win-x64/AMDevIT.Admob.Wrapper.MAUICross.Tests.dll
```

Configure the JDK 21 and Android SDK paths for .NET Android if they are not
discovered automatically. The test project currently uses VSTest, while
`global.json` selects Microsoft.Testing.Platform; direct `dotnet vstest` avoids
that runner mismatch. Windows tests are excluded from aggregate solution builds.

Pack the Droid, iOSNative, shared wrapper, and MAUICross projects into one clean
output directory using Release outputs from the same commit/version. Run
`eng/verify-packages.ps1` with that directory and version `0.2.0`; it checks package
metadata and builds an Android/Windows consumer. Do not mix earlier build artifacts
with the candidate being validated.

## Publication gate

1. Complete the native rebuilds, .NET builds/tests, package validation, and a
   local-package consumer restore/build for the exact selected release artifacts.
2. Review BG8605/BG8606 resolution reports. The known baseline excludes only
   Kotlin compiler-generated dollar-sign members; investigate newly missing
   types or public APIs and any other new build/dependency warnings.
3. Install the generated packages in a real consumer without project references.
   Test Android/iOS first-run and returning-user consent, privacy options, error
   fallback, logging, all ad formats, rewards, repeated loads, refresh, resizing,
   and teardown on physical devices using Google's test ad IDs.
4. Check package versions, release notes, README rendering, dependency groups,
   native binaries, repository commit, license/icon, and symbols. Confirm whether
   the final MAUI artifact includes the Windows target.
5. Publish only the reviewed packages and symbols manually. CI and the macOS
   preparation script hold no publishing API key and do not push packages.
6. Tag the exact published source commit as `v0.2.0` and finalize its changelog
   publication date. Never replace an already published package with new bytes
   under the same ID/version; select a new version instead.
