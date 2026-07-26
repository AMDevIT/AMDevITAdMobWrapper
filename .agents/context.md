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
