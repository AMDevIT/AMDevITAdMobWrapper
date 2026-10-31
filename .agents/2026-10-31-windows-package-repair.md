# 0.2.1 Windows package repair - 2026-10-31

## Objective and status
Repair Windows NU1202 caused by the macOS-produced MAUICross 0.2.0 package.
Implementation, Release build, tests, package validation and consumer checks passed.

## Decisions
User confirmed changes/builds/checks and that 0.2.0 is published. Advance all four
NuGet packages to 0.2.1; preserve unchanged native 0.2.0 binaries (iOS build 2,
confirmed in both slices). Block MAUICross packing without Windows. macOS now
packs only three base libraries under artifacts/native-preparation/<Version>;
final four-platform MAUICross packages are produced on Windows. Both validators
require all four platform DLLs. Consumer checks use a fresh NuGet cache outside
the project directory and restore the previous environment afterward.
No publication, commit, push or MyCurrencyConverter edits were performed.

## Affected files
CHANGELOG.md, README.md, RELEASING.md, eng/verify-packages.ps1,
sources/dotnet/AMDevIT.Admob.Wrapper/Directory.Build.props, MAUICross.csproj,
sources/tools/README.md, prepare-nuget.sh, verify-nuget.py and .agents context.

## Checks
Restore and aggregate Release build passed; 10 VSTest tests passed. Existing
BG8605/BG8606, MSTEST0032 and PRI257/PRI263 warnings remain. JDK 21 selected from
C:/Program Files/Java/jdk-21 after Android Studio JDK 25 was rejected. Four packages
and four symbol packages created at artifacts/packages/0.2.1. Python package and
native-preparation validation passed; both validators rejected a fixture with
Windows removed. MSBuild rejected an iOS-only package target list. Consumer restore
and Android/Windows Release build passed with zero warnings/errors. Initial cache
placement inside the project caused SDK globbing errors; sibling project/cache
directories fixed the verification harness. Bash syntax/help and whitespace passed.
SDK and Bash checks needed execution outside the restrictive Windows sandbox.
No real macOS execution, physical-device tests or actual MyCurrencyConverter build.

## Next step
Review and manually publish complete 0.2.1 packages/symbols, then update the consumer.
Never overwrite published 0.2.0. Physical-device smoke checks remain a release gate.
