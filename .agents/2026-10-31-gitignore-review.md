# Gitignore review — 2026-10-31

## Objective and status

Android/Kotlin, Swift/Xcode, and .NET ignore review completed after confirmation.

## Decisions and affected files

- Updated .gitignore: added Kotlin state and SwiftPM cache/user-state rules;
  removed broad workspace/resolved-file exclusions and duplicate rules.
- Existing .NET rules already cover bin, obj, packages, test results,
  artifacts/, and .artifacts/; retained them.
- Binding AAR/XCFramework and Gradle wrapper remain versionable.
- Updated this note and .agents/context.md.

## Checks performed and results

- git check-ignore --no-index passed for 12 ignored and 10 versionable paths.
- Initial check caught a nested SwiftPM cache pattern; corrected with **/
  and reran all checks successfully.
- git diff --check passed. No builds/restore/tests run; Git checks only.

## Open issues and next step

No open issues. Review and commit; shared workspace files and Package.resolved
may now appear as untracked when generated.
