# Manual CI and Windows-only tests — 2026-07-31

## Objective and status

Prevent the Windows-only MAUICross test project from breaking aggregate
cross-platform solution builds, and stop GitHub Actions from running on every
push or pull request.

Status: completed.

## Decisions made

- Kept the test project visible under the solution's `Tests` folder.
- Set `IsBuildable="false"` on its `.slnx` project entry. Aggregate solution
  builds therefore skip the Windows-only project, while it remains available
  for explicit execution on Windows.
- Preserved the workflow's explicit `dotnet run --project` test step, so a
  manually started Windows CI run still executes the test suite.
- Removed the `push` and `pull_request` workflow triggers and retained only
  `workflow_dispatch`.

## Affected files

- `sources/dotnet/AMDevIT.Admob.Wrapper/AMDevIT.Admob.Wrapper.slnx`
- `.github/workflows/ci.yml`
- `.agents/2026-07-31-manual-ci-windows-tests.md`
- `.agents/context.md`

## Checks performed and results

- `dotnet sln ... list` parsed the modified `.slnx` successfully and retained
  the test project in the solution.
- `dotnet msbuild ... -t:ValidateSolutionConfiguration` succeeded.
- Direct execution of the Windows test project succeeded: seven tests passed,
  zero failed, zero skipped.
- The aggregate Release build command exceeded the local six-minute command
  limit while processing the Android sample applications; its child processes
  subsequently exited without returning the parent command's final status.
- An attempted aggregate `Compile` target was rejected because that target is
  not supported uniformly by the MAUI and binding projects. Its diagnostics are
  unrelated to the solution configuration change and it is not a valid check
  for this solution.
- `git diff --check` succeeded before the progressive-context update, with only
  the expected Windows line-ending notices.

## Open issues and recommended next step

- Start the updated workflow manually from GitHub Actions and confirm the full
  hosted build, package verification, and artifact upload complete.
- Cross-platform aggregate builds now skip the Windows-only tests by design;
  run the test project explicitly on a Windows host when validating a release.

