# Contesto progressivo

## 2026-07-26 — English translation of AGENTS.md

- Objective and status: translated the root `AGENTS.md` file from Italian into English; completed.
- Decisions made: preserved the original structure, technical meaning, paths, commands, and required region names.
- Affected files: `AGENTS.md`, `.agents/context.md`.
- Checks performed and results: reviewed the translated document and checked it for remaining Italian prose; no issues found.
- Open issues and recommended next step: none; no build was run because only documentation files were changed.

## 2026-07-27 — MAUI Windows and Mac Catalyst fallback

- Objective and status: added Windows and Mac Catalyst targets to the MAUI wrapper and test app, with `BannerAd.FallbackTemplate` rendered on those desktop platforms; completed.
- Decisions made: retained the existing `View`-based banner and native Android/iOS handlers; used a shared `DataTemplate` bindable property whose default creates an empty `ContentView`; instantiated the fallback only in the Windows and Mac Catalyst handlers; added explicit unsupported desktop implementations for shared full-screen services.
- Affected files: `README.md`, the MAUI wrapper and test-app project files, `BannerAd.cs`, the shared and desktop banner handlers, desktop service implementations, and `MainPage.xaml`. Detailed paths are recorded in `.agents/2026-07-27-maui-desktop-fallback.md`.
- Checks performed and results: restore succeeded; wrapper and XAML test-app builds succeeded for Windows, Mac Catalyst, and iOS; aggregate build reached those targets successfully but Android failed with `XA5300` because the Java SDK is unavailable; existing `NU1608` AndroidX warnings remain.
- Open issues and recommended next step: review the public fallback API and explicit desktop `PlatformNotSupportedException` behavior, then re-run Android checks after configuring a Java SDK.

## 2026-07-27 — MAUICross target-framework correction

- Objective and status: restored Mac Catalyst to the actual `TargetFrameworks` configuration after Windows support had been added separately; completed.
- Decisions made: added `net10.0-maccatalyst` to the unconditional target list with minimum version 15.0; preserved the user-provided conditional Windows target.
- Affected files: `AMDevIT.Admob.Wrapper.MAUICross.csproj`, `.agents/2026-07-27-maui-desktop-fallback.md`, `.agents/context.md`.
- Checks performed and results: restore succeeded; MSBuild resolves Android, iOS, Mac Catalyst, and Windows targets on the current Windows host; Mac Catalyst and Windows wrapper builds both succeeded with zero errors; existing `NU1608` warnings remain; `diff --check` succeeded apart from line-ending conversion warnings.
- Open issues and recommended next step: none for desktop target-framework configuration.

## 2026-07-27 — Windows 10 minimum runtime

- Objective and status: support Windows 10 while compiling against the latest configured Windows SDK contract; completed.
- Decisions made: retained `net10.0-windows10.0.26100.0` as the compile-time target and changed both `SupportedOSPlatformVersion` and `TargetPlatformMinVersion` to Windows 10 1809 (`10.0.17763.0`).
- Affected files: `AMDevIT.Admob.Wrapper.MAUICross.csproj`, `.agents/2026-07-27-maui-desktop-fallback.md`, `.agents/context.md`.
- Checks performed and results: the Windows MAUI wrapper build succeeded with zero errors; only the existing eight `NU1608` AndroidX dependency warnings remain.
- Open issues and recommended next step: validate the generated Release NuGet package before publishing.

## 2026-07-31 — NuGet 0.1 preview readiness

- Objective and status: prepared `0.1.0-preview.1` release candidates for all four public packages and defined the promotion gate to stable `0.1.0`; automated local gates are complete, while publication and physical-device checks remain manual.
- Decisions made: centralized package version/metadata; fixed full-screen task completion, cancellation isolation, disposal, and rewarded-service API exposure; pinned compatible AndroidX packages; added tests, CI, package verification, symbols, release documentation, and short repository-level artifact paths; kept NuGet publishing out of CI.
- Affected files: `README.md`, `RELEASING.md`, `.github/workflows/ci.yml`, `eng/verify-packages.ps1`, `Directory.Build.props`, the four package projects, MAUICross full-screen services/interfaces, the new test project, the solution file, and `.agents/2026-07-31-nuget-preview-readiness.md`.
- Checks performed and results: four package projects built with zero warnings/errors; full Release solution built with zero errors and six demo-app-only `XA4301` warnings; all seven lifecycle tests passed; four `.nupkg` and four `.snupkg` files passed package validation; a temporary local-package MAUI consumer built Android and Windows with zero warnings/errors; `git diff --check` passed apart from line-ending notices.
- Open issues and recommended next step: run physical Android/iOS ad flows and the existing real consumer against the exact local packages, run GitHub CI, investigate the demo-app duplicate-native-library warnings, then manually publish/tag the preview. Promote to `0.1.0` only after preview soak, warning resolution, and a complete gate rerun.
