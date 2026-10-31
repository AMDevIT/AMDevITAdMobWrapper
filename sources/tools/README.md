# Prepare NuGet packages on macOS

Run from the repository root, or pass the script's absolute path from any directory:

```bash
bash sources/tools/prepare-nuget.sh
```

The script builds the Android wrapper in Release, runs its Kotlin tests, resolves
the Swift packages, builds the iOS device/simulator XCFramework, and replaces both
native binaries in the .NET binding projects. It then restores workloads and
packages, builds the .NET solution in Release, and packs the four public libraries.
The curated C# binding definitions are retained; Objective Sharpie is not run.

The version comes from the .NET projects and must match across all four packages.
The script creates four `.nupkg` and four `.snupkg` files under
`artifacts/packages/<Version>/`. It checks archive integrity, identity, symbols,
license, README/icon, repository metadata, direct dependencies, and MAUI platform
assemblies using `verify-nuget.py`. These checks do not replace installing the
packages in a real consumer and running physical-device tests.

An existing version output directory is refused. To repeat preparation of the
same version without overwriting earlier packages, choose another output root:

```bash
bash sources/tools/prepare-nuget.sh --output "$HOME/Desktop/admob-candidates"
```

Relative output paths are relative to the calling directory. On failure, the
script stops immediately and retains its temporary package directory for
inspection. Native binaries already replaced in the working tree remain replaced;
review the Git diff before committing. No commit, push, or NuGet publication occurs.

## Prerequisites

- macOS and Xcode with the iOS device/simulator SDKs available through `xcrun`.
  Only availability is checked; no Xcode version is queried or compared.
- .NET SDK 10 and the required Android, iOS, MAUI, and Mac Catalyst workloads.
  `dotnet workload restore` installs missing workloads and may need permissions.
- JDK 21. Set `JAVA_HOME`, or the script locates it using `java_home -v 21`.
- An Android SDK with the project's platform/build tools and accepted licenses.
  Set `ANDROID_HOME` or `ANDROID_SDK_ROOT`; the default is `$HOME/Library/Android/sdk`.
- Python 3 and `xcpretty` on `PATH`. The existing XCFramework builder uses
  `xcpretty`; install it before running this script.
- Network access for Gradle, Swift packages, workloads, and NuGet restore.

No Xcode compatibility validation is enabled by this script in .NET builds.
Compilation can still fail if the installed tools cannot compile the source.

## Platform scope

On macOS, the MAUI package contains Android, iOS, and Mac Catalyst assemblies.
Its Windows target is conditional on the host OS, so prepare a Windows-inclusive
package on Windows if that is the artifact intended for publication. Do not
publish both variants with the same package ID/version.

The existing .NET unit tests target Windows and are not run by this script.
Run them on Windows separately. The macOS package checks do not invoke the
Windows-specific consumer smoke test in `eng/verify-packages.ps1`.

Display command options with:

```bash
bash sources/tools/prepare-nuget.sh --help
```
