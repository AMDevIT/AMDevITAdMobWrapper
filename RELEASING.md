# Release process

The release being prepared is **0.2.1**. Its changes are recorded in
[CHANGELOG.md](CHANGELOG.md); publication is manual.

## Version and release metadata

The four public NuGet packages share `VersionPrefix`, `VersionSuffix`, and
`PackageReleaseNotes` from
`sources/dotnet/AMDevIT.Admob.Wrapper/Directory.Build.props`.
Keep the following aligned before preparing artifacts:

- All four package versions, with no project-local release-note/version overrides.
- Native metadata stays at 0.2.0 (iOS build 2): 0.2.1 changes only NuGet packaging
  and reuses the unchanged native binaries from 0.2.0.
- README package examples and changelog entry.

Version 0.2.1 is a packaging patch for the already published 0.2.0 release.
Keep the suffix empty for the stable candidate. Native rebuilds are required
when native source or dependencies change, not for this packaging-only repair.

## Native and macOS preparation

Prerequisites include .NET 10, JDK 21, Android SDK, and Xcode with iOS SDKs.
The tools check Xcode availability without comparing its version.

From the repository root:

```bash
bash sources/tools/prepare-nuget.sh
```

This rebuilds the native Android/iOS wrappers, replaces the binding binaries,
runs Kotlin tests, restores/builds Release, and prepares three base `.nupkg` and
three `.snupkg` files under `artifacts/native-preparation/0.2.1/`.
MAUICross is built but is never packed on macOS. These are intermediate outputs,
not a complete release. See the [tooling guide](sources/tools/README.md).

Review and transfer the rebuilt AAR/XCFramework with the corresponding source
changes before producing all four release packages on Windows. MAUICross packing
fails if its Windows target is absent. Never publish host-dependent variants
under the same package ID/version. Version 0.2.0 is already published; the repair
uses 0.2.1 and must not overwrite 0.2.0.

## Windows checks and packaging

From the repository root, PowerShell 7 can run the complete local preparation:

```powershell
./sources/tools/prepare-nuget.ps1 -JavaSdkDirectory 'C:\Program Files\Java\jdk-21'
```

This rebuilds/tests Android, reuses the iOS XCFramework prepared on macOS,
restores/builds Release, runs .NET tests, packs all four libraries and symbols,
and validates the packages plus an Android/Windows consumer. Output is staged
until validation succeeds at `artifacts/packages/<Version>/`. Existing version
directories are refused; `-OutputDirectory` selects another root. See the
[tooling guide](sources/tools/README.md) for SDK paths and prerequisites.

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
`eng/verify-packages.ps1` with that directory and version `0.2.1`; it checks package
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
   native binaries, repository commit, license/icon, and symbols. Require Android, iOS, Mac Catalyst, and Windows assemblies
   in the final MAUI artifact.
5. Publish only the reviewed packages and symbols manually. CI and the macOS
   preparation script hold no publishing API key and do not push packages.
6. Tag the exact published source commit as `v0.2.1` and finalize its changelog
   publication date. Never replace an already published package with new bytes
   under the same ID/version; select a new version instead.
