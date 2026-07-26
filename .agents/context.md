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
