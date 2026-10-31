# Windows PowerShell NuGet preparation - 2026-10-31

## Objective and status
Add sources/tools/prepare-nuget.ps1 as the Windows counterpart of Bash native
preparation. Implementation and full Windows execution completed, including native Android,
.NET tests, all packages/symbols, and an isolated Android/Windows consumer.

## Decisions
- User confirmed implementation and complete execution. Require PowerShell 7,
  Windows, .NET 10, JDK 21, Android SDK and Python 3 named python on PATH.
- Run Gradle Kotlin tests and Release AAR build, copy the AAR to the binding.
  Reuse the iOS XCFramework prepared on macOS, validating both native slices;
  never pretend Windows can compile Swift/Xcode.
- Compare all four MSBuild package versions; restore workloads and packages,
  build Release, run Windows .NET tests through VSTest, pack four libraries and
  symbols, run both validators and the isolated Android/Windows consumer.
- Offer SkipWorkloadRestore for prepared hosts; default restores workloads.
  No package restore/build/test/validation step is skipped by this option.
- Stage below the selected output root and expose <Version> only after success;
  refuse collisions, retain failed staging, never publish/commit/push.
- Resolve relative paths through PowerShell's current location (not process cwd).
  Restore caller location and JAVA_HOME, ANDROID_HOME, ANDROID_SDK_ROOT and PATH.
- Preserve prior task and concurrent user changes, including MAUI 10.0.120 in
  project files. Read the MAUI version from the project in the PowerShell verifier
  instead of hard-coding 10.0.90, and use XPath for dependency groups so empty
  groups work under the preparation script strict mode.
- Native rebuild changed only proguard.txt line endings inside the AAR. Compared
  every archive entry, confirmed CRLF-only differences, and restored original
  checked-in AAR bytes to avoid unrelated binary churn.

## Affected files
sources/tools/prepare-nuget.ps1, sources/tools/README.md, README.md, RELEASING.md,
the validator docstring, eng/verify-packages.ps1 and .agents notes.

## Checks performed and results
- PowerShell AST syntax and detailed help passed.
- Missing JDK rejected; native exit code 7 propagated; staging parent checks
  accepted paths with spaces.
- Collision check from a different calling directory passed and confirmed
  environment/location restoration. That regression first exposed the process
  cwd versus PowerShell cwd issue; path resolution was corrected.
- Real Gradle build passed; all 7 existing Kotlin tests passed.
- iOS device/simulator framework validation passed.
- Default workload restore completed successfully after lengthy installer cleanup.
  The full run reached package verification, exposing strict-mode empty-group
  handling in the existing verifier; this was corrected. The next verification
  exposed its hard-coded MAUI version; that is now read from the project.
- Final script executed from artifacts with relative output containing spaces,
  using SkipWorkloadRestore after successful workload installation. Restore and
  aggregate Release build passed (four existing BG8605/BG8606 warnings), all 10
  VSTest tests passed (existing PRI warnings), four nupkg and four snupkg passed
  Python validation, and the Android/Windows consumer built with zero warnings
  and zero errors. Success also preserved the caller directory and environment.
- Final output: artifacts/PowerShell packages verified/0.2.1. Final log:
  artifacts/powershell-preparation-final.log. Failed staging directories were
  retained as designed; no incomplete version directory was exposed.
- PowerShell syntax/help and git diff --check passed. No package publication,
  commit, push, real macOS execution or physical-device tests performed.

## Open issues and recommended next step
No implementation issue remains. Use the documented PowerShell entry point on
Windows; prepare native iOS changes on macOS first. Review exact release artifacts
and complete physical-device checks before manual publication. Existing standard
artifacts/packages/0.2.1 output is preserved; choose another output root to rerun.
