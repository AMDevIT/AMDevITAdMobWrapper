# macOS workload restore path correction — 2026-10-31

## Objective and status

Fix prepare-nuget.sh after the user's first macOS execution stopped during
workload restoration with the no-project-found error. Corrected and locally
validated; real macOS .NET build/pack verification remains pending.

## Evidence and decisions

- The supplied macOS log confirms Android Gradle tests/Release assembly passed.
  Swift Package resolution selected GoogleMobileAds 13.11.0 and UMP 3.1.0;
  device/simulator archives and XCFramework creation succeeded, and native
  binaries were copied into the .NET bindings on that Mac.
- The failing command was dotnet workload restore . from the directory containing
  AMDevIT.Admob.Wrapper.slnx and nested project directories. Directory discovery
  did not find a project in the user's .NET SDK 10.0.400 execution.
- Replaced the directory argument with the explicit MAUICross .csproj, which
  covers Android/iOS/Mac Catalyst and references both native bindings.
- Restore/build now also receive the explicit .slnx filename. Both paths are
  checked before native builds begin, and progress messages identify each step.
- Preserved availability-only Xcode checks and ValidateXcodeVersion=false.
  No SDK version, source, package version, or native binary was changed locally.
- The user ran the script with sudo; elevation does not supply the missing
  project argument. No privilege or ownership changes were made by this fix.

## Affected files

- sources/tools/prepare-nuget.sh
- sources/tools/README.md
- This note and .agents/context.md

## Checks performed and results

- HTTPS fetch succeeded; local HEAD matched the remote working branch.
- bash -n and --help passed using Git Bash.
- A focused regression fixture rejected the old directory-based workload
  command, accepted the corrected three-command .NET phase only with existing
  explicit .csproj/.slnx paths, and checked quoting for paths containing spaces
  and preservation of ValidateXcodeVersion=false on restore/build.
- Scoped git diff --check passed, with expected Markdown line-ending notices.
- No real workload installation, restore, compilation, or packing was run on
  this Windows host. The fixture checks command construction and failure
  behavior; it cannot establish macOS toolchain compatibility.
- Preserved pre-existing .gitignore, gitignore-review note, and context edits.

## Open issues and recommended next step

Transfer the corrected script to the Mac and rerun it from sources/tools.
The previous failed run created only a temporary .prepare-nuget directory,
so the final version-directory collision check should not prevent a retry
unless another run already completed. Inspect the next .NET build/pack output.
