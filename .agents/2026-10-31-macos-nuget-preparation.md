# macOS NuGet preparation tools — 2026-10-31

## Objective and status

Create a Bash entry point under sources/tools that rebuilds the Android/iOS
native wrappers, replaces their .NET binding binaries, builds the libraries,
and prepares the four NuGet packages and symbols. Implementation and local
script/fixture validation are complete. A real end-to-end run requires macOS.

## Decisions made

- Added prepare-nuget.sh, compatible with the Bash shipped on macOS and callable
  from any directory. It uses the existing Gradle wrapper and iOS XCFramework
  builder; curated C# bindings are preserved and Sharpie is not invoked.
- Per the user's explicit instruction, check only Xcode/iOS SDK availability.
  Never query or compare the Xcode version; pass ValidateXcodeVersion=false to
  .NET restore/build/pack. Real toolchain incompatibilities may still fail builds.
- Require .NET 10, JDK 21, Android SDK, Python 3, and xcpretty. Reuse JAVA_HOME
  and ANDROID_HOME/ANDROID_SDK_ROOT; otherwise use standard macOS SDK discovery.
- Run Kotlin tests, assembleRelease, resolve Swift packages, build/validate both
  iOS XCFramework slices, copy AAR/XCFramework into the binding projects, restore
  workloads/packages, build the solution in Release, then pack four projects.
- Read and compare all four package versions using MSBuild. Stage output in a
  temporary sibling directory and expose artifacts/packages/<Version> only after
  validation. Refuse an existing version directory; --output selects another root.
- Stop on errors and retain partial package output. Native binary replacements
  already completed remain in the working tree. No Git changes or NuGet packages
  are committed/pushed/published by the script.
- Added a Python standard-library validator for archive integrity, identity,
  symbols, license, README/icon, repository metadata, direct package/project
  dependencies, absence of legacy Ads, and MAUI platform assemblies.
- Explicitly document that host-conditional Windows assemblies are absent from
  macOS MAUI packages; prepare the Windows-inclusive artifact on Windows, and
  never publish both variants with the same ID/version. Existing Windows .NET
  tests and Windows consumer smoke tests must run separately on Windows.
- Added tools-local LF rules and documentation linked from RELEASING.md.

## Affected files

- sources/tools/prepare-nuget.sh
- sources/tools/verify-nuget.py
- sources/tools/README.md
- sources/tools/.gitattributes
- RELEASING.md
- This note and .agents/context.md

## Checks performed and results

- HTTPS fetch succeeded; the working branch matched its remote branch, with no
  pull required. Existing repository files were not modified outside this scope.
- bash -n and --help succeeded using Git Bash on Windows. Missing --output values
  and a non-macOS host were correctly rejected without starting builds.
- Package fixtures: all four package/symbol pairs accepted; missing symbols and
  incorrect dependency versions rejected.
- Isolated full-flow simulation with stub native/.NET tools passed, including
  paths containing spaces, native file replacement, command ordering, and all
  eight expected package outputs. No Xcode version query occurred.
- The simulation rejected existing output and propagated a native Xcode failure
  with its original exit code, retaining partial output without exposing a final
  version directory. Temporary fixture directories were removed.
- Real Xcode, Swift/Gradle/.NET builds, real NuGet packing, and device tests were
  not executed for this tooling-only task on Windows. The mocked run proves
  orchestration and guards, not macOS compiler/toolchain compatibility.
- Whitespace, LF line endings, and Python syntax checked for the added tools.

## Open issues and recommended next step

Run bash sources/tools/prepare-nuget.sh on the Mac with the prerequisites ready,
review the generated native binary diffs and packages, and complete consumer and
physical-device smoke tests before manually publishing the chosen artifact.
