# Prepare NuGet packages

## Windows: complete release packages

Run with PowerShell 7 from the repository root, or use the script's absolute path:

```powershell
./sources/tools/prepare-nuget.ps1 -JavaSdkDirectory 'C:\Program Files\Java\jdk-21'
```

The script rebuilds Android, runs Kotlin tests, and replaces the binding AAR.
It validates and reuses the checked-in iOS XCFramework, which must first be built
on macOS when native iOS source or dependencies change. Swift/Xcode is not run
on Windows. Both device and simulator slices are required.

It restores workloads and packages, builds the solution in Release, runs the
Windows .NET tests through VSTest, and packs all four libraries with symbols.
Both package validators and an isolated Android/Windows consumer restore/build
must pass before output appears under `artifacts/packages/<Version>/`.
MAUICross includes Android, iOS, Mac Catalyst, and Windows.

Requires Windows, PowerShell 7, .NET 10, JDK 21, Android SDK, Python 3 on `PATH`,
and network access for Gradle, workloads, and NuGet. Workload restoration may
install missing workloads and require permissions. No Xcode version validation
is enabled. Python must be available as `python`.

Use `-SkipWorkloadRestore` to reuse workloads already installed on a prepared
build machine. Package restore, build, tests, and package/consumer checks still
run; missing workloads cause the build to fail. The default restores workloads,
matching the Bash preparation flow.

`-JavaSdkDirectory` defaults to `JAVA_HOME`, then `C:\Program Files\Java\jdk-21`.
`-AndroidSdkDirectory` defaults to `ANDROID_HOME`, `ANDROID_SDK_ROOT`, or the
standard Android Studio/Visual Studio SDK directories. Explicit paths override
discovery. The script restores the calling directory and changed environment
variables when it finishes, including after a failure.

An existing version directory is refused before starting builds. Select another
output root to prepare the same version again:

```powershell
./sources/tools/prepare-nuget.ps1 -OutputDirectory 'D:\NuGet candidates' -JavaSdkDirectory 'C:\Program Files\Java\jdk-21' -AndroidSdkDirectory 'D:\Android\Sdk'
Get-Help ./sources/tools/prepare-nuget.ps1 -Detailed
```

Relative output paths are resolved from the calling directory. Failures stop the
script and retain the staging directory for inspection; an AAR already replaced
remains replaced. Review native changes before committing. Nothing is committed,
pushed, or published. Physical-device release checks remain separate.

## macOS: native preparation

Run from the repository root, or pass the script's absolute path from any directory:

```bash
bash sources/tools/prepare-nuget.sh
```

The script builds the Android wrapper in Release, runs its Kotlin tests, resolves
the Swift packages, builds the iOS device/simulator XCFramework, and replaces both
native binaries in the .NET binding projects. It then restores workloads and
packages, builds the .NET solution in Release, and packs the three base libraries.
MAUICross is built but packed only on Windows.
The curated C# binding definitions are retained; Objective Sharpie is not run.
Workload restoration targets the MAUI `.csproj` explicitly, covering Android,
iOS, Mac Catalyst and its referenced bindings. Package restore and build use the
explicit `.slnx` path, rather than relying on current-directory discovery.

The version comes from the .NET projects and must match across all four packages.
The script creates three `.nupkg` and three `.snupkg` files under
`artifacts/native-preparation/<Version>/`. It checks archive integrity, identity,
symbols, license, README/icon, repository metadata, and direct dependencies using
`verify-nuget.py --native-preparation`. These checks do not replace installing the
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

macOS output is intermediate native preparation, not a complete release.
Transfer the rebuilt native binaries and source changes to Windows, pack all four
projects there, and run `eng/verify-packages.ps1` before publication. MAUICross
packing is blocked when Windows is absent; both release validators require all
four MAUI platform assemblies. Do not publish an incomplete host variant.

The existing .NET unit tests target Windows and are not run by this script.
Run them on Windows separately. The macOS package checks do not invoke the
Windows-specific consumer smoke test in `eng/verify-packages.ps1`.

Display command options with:

```bash
bash sources/tools/prepare-nuget.sh --help
```
