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

## 2026-07-31 — Cross-platform banner sizing

- Objective and status: fixed right-side banner clipping in the MAUI handlers, centered fixed banners, based adaptive banners on actual container width, and added every `BannerAdSize` mapping to the iOS source/binding; Android is complete including a regenerated AAR, while the iOS xcframework must still be regenerated on macOS.
- Decisions made: report exact fixed dimensions to MAUI; host Android ads in a centered container; reload adaptive ads when arranged width changes; add explicit native adaptive-width and iOS size-aware APIs; center iOS ads using intrinsic width instead of stretching them edge-to-edge.
- Affected files: Android Kotlin wrapper/AAR/binding metadata, Swift banner wrapper, iOS binding API and enum, shared banner-size helpers, Android/iOS MAUI handlers, and `.agents/2026-07-31-banner-sizing.md`.
- Checks performed and results: Gradle release AAR build succeeded; targeted Android and iOS builds succeeded; required restore succeeded; complete serial solution build succeeded with zero warnings/errors; all seven existing tests passed through the MTP executable; `diff --check` passed apart from line-ending notices.
- Open issues and recommended next step: regenerate and replace the iOS xcframework on macOS, then physically verify every device-appropriate fixed size, adaptive rotation/resizing, padded layouts, and runtime size changes on Android and iOS.

## 2026-07-31 — Manual CI and Windows-only tests

- Objective and status: prevented the Windows-only MAUICross test project from participating in aggregate cross-platform solution builds and changed GitHub Actions to manual execution only; completed.
- Decisions made: retained the test project in the `.slnx` with `IsBuildable="false"`, preserved its explicit Windows execution in CI, and removed `push`/`pull_request` triggers in favor of `workflow_dispatch` only.
- Affected files: `.github/workflows/ci.yml`, `AMDevIT.Admob.Wrapper.slnx`, `.agents/2026-07-31-manual-ci-windows-tests.md`, and `.agents/context.md`.
- Checks performed and results: the solution list and `ValidateSolutionConfiguration` parsed successfully; direct execution passed all seven tests; the full aggregate build exceeded the local six-minute limit while processing Android sample apps, and an attempted aggregate `Compile` target was not applicable to MAUI/binding projects.
- Open issues and recommended next step: launch the workflow manually on GitHub and inspect the hosted build and uploaded NuGet artifacts.

## 2026-08-02 — Native AdMob Next-Gen and iOS UMP alignment

- Objective and status: verified Android AdMob Next-Gen and UMP, then ported the missing consent, diagnostics, and dismissal behavior to the native Swift framework; native source work is complete, while the macOS Xcode build and XCFramework regeneration remain pending with the user.
- Decisions made: retained Android Next-Gen `1.3.1` with transitively supplied UMP `4.0.0`; retained only Google's official Mobile Ads Swift package, raised it to `13.7.0`, and relied on its transitive UMP `3.1.0`; added Objective-C-visible Apple logging and consent APIs; moved `onAdDismissed` exclusively to `didDismiss`; aligned the iOS framework version to Android `0.1.10`; made the XCFramework script propagate piped build failures.
- Affected files: the native iOS Xcode project, `AdMobManager.swift`, all four Swift ad wrappers, new Diagnostics/Privacy/consent-listener Swift files, `build_xcframework.sh`, and `.agents/2026-08-02-native-admob-ump-ios.md`. No Android source, AAR, XCFramework, or .NET project was modified.
- Checks performed and results: Android release assembly succeeded; Gradle dependency insight confirmed UMP `4.0.0` only through Mobile Ads Next-Gen `1.3.1`; package URL/version, lifecycle callbacks, source structure, and scoped diffs passed static checks. Swift/Xcode compilation is unavailable on this Windows host; .NET checks were intentionally omitted by scope.
- Open issues and recommended next step: build and regenerate the XCFramework on the Mac Air with the required Xcode version, smoke-test UMP and ad dismissal callbacks, then handle the binary and .NET bindings in a later explicitly authorized step.

## 2026-08-02 — .NET Android AdMob Next-Gen and UMP alignment

- Objective and status: aligned the .NET Android binding, shared async wrapper, MAUI services, logging, samples, package metadata, tests, and documentation with native AdMob Next-Gen `1.3.1` and UMP `4.0.0`; completed for Android. The .NET iOS binding remains intentionally pending until the user supplies the regenerated XCFramework.
- Decisions made: embedded the Next-Gen and HSDP Maven artifacts; removed the legacy Google Play Services Ads binding; added explicit official .NET transitive bindings including UMP `4.0.0.3`; introduced shared consent models, async Android APIs, Android-only MAUI consent DI, and an `IDroidLogger` adapter; aligned packages to `0.1.10`; enabled Microsoft Testing Platform through solution-local `global.json`.
- Affected files: `README.md`, `.NET Directory.Build.props`, `global.json`, the Android binding project/AAR/metadata, shared consent types and Android extensions, Android MAUI handlers/services/DI, Android and MAUI test apps, `.agents/2026-08-02-dotnet-android-admob-ump.md`, and this file. No .NET iOS binding or Apple test-app file was modified.
- Checks performed and results: Android binding/shared/MAUI libraries and both Android apps built; required aggregate restore/build succeeded across all library targets; all 7 tests passed; three `0.1.10` NuGet packages were created and the Android package was inspected to confirm embedded Next-Gen `1.3.1`, HSDP `2.0.1`, UMP `4.0.0.3`, and absence of legacy Ads. Two binding warnings remain only for dropped Kotlin synthetic `$` members; the Windows test PRI warning comes from MSTest adapter resources.
- Open issues and recommended next step: receive/regenerate the iOS XCFramework, then implement the .NET iOS binding and iOS MAUI consent service; physically validate Android consent and ad flows before publishing `0.1.10`; revisit the prerelease Privacy Sandbox .NET binding and Kotlin synthetic warnings when upstream stable/tooling updates become available.
