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

## 2026-08-02 — .NET iOS AdMob and UMP binding alignment

- Objective and status: updated the .NET iOS native binding and shared wrapper against the regenerated XCFramework; completed. Logger, UMP, banner sizing, and async consent APIs are now available to .NET iOS consumers.
- Decisions made: bound only the wrapper's public Objective-C API and excluded Google delegate extension categories; restored the `AMDevIT.Admob.Wrapper.iOSNative` namespace; preserved existing managed ad API names; exposed `IAppleLogger` with `AppleLogLevel`; mirrored the Android async consent workflow and cancellation semantics; left MAUICross source unchanged.
- Affected files: `README.md`, iOSNative `ApiDefinition.cs` and `StructsAndEnums.cs`, shared iOS `AdMobManagerExtensions.iOSNative.cs`, `.agents/2026-08-02-dotnet-ios-admob-ump.md`, and this file. The supplied XCFramework was inspected and packaged but not modified.
- Checks performed and results: iOSNative, shared iOS wrapper, MAUICross iOS, and Apple test app built; required restore succeeded; aggregate build succeeded for all library targets with zero warnings/errors after explicitly selecting Android Studio's JDK; all 7 tests passed; both `0.1.10` Release packages were created and inspected, including both XCFramework slices in the iOS binding resource archive.
- Open issues and recommended next step: physically test iOS UMP, logging, all ad formats, banner sizing/rotation, and dismissal timing before publishing. MAUI consent DI remains intentionally Android-only; iOS uses the shared async manager extensions directly.

## 2026-08-02 — MAUICross iOS UMP and native logging alignment

- Objective and status: extended MAUICross to expose the updated AdMob and UMP flow through `IAdMobConsentService` on both Android and iOS; completed.
- Decisions made: added an iOS consent service and `IAppleLogger` adapter; injected native logging into the iOS manager and every ad wrapper; preserved the existing initialization signature, with the application ID required only by Android; retained direct iOSNative and shared-wrapper references because both are required at compile/package time; left desktop fallback targets unchanged.
- Affected files: `README.md`, `Directory.Build.props`, MAUICross project/DI/consent interface, new iOS consent and diagnostics files, iOS banner/full-screen implementations, `.agents/2026-08-02-mauicross-ios-ump.md`, and this file.
- Checks performed and results: all four MAUICross targets built with zero warnings/errors; required restore and aggregate build succeeded; all 7 tests passed with only the existing external Windows PRI resource warning; the `0.1.10` MAUICross package was created and its iOS dependency group contains both the native binding and shared wrapper. The final pack retained the two documented Android binding warnings for Kotlin synthetic `$` members.
- Open issues and recommended next step: physically test Android/iOS consent, privacy-options, error fallback, logging, and every ad format before publishing; keep debug consent settings out of production.

## 2026-08-02 — MAUICross desktop consent no-op

- Objective and status: made `IAdMobConsentService` available on every MAUICross target with explicit availability detection and safe no-op behavior on Windows and Mac Catalyst; completed.
- Decisions made: added `IsSupported`; retained real UMP on Android/iOS; registered consent DI everywhere; made desktop initialization/gathering/forms/reset non-throwing with neutral `NotRequired` state, `CanRequestAds == true`, warning-on-creation, and debug logs per skipped operation; added a neutral `net10.0` shared-wrapper target.
- Affected files: shared and MAUICross project files, consent interface and all platform implementations, MAUI DI and test-app flow, MAUICross tests, README/release notes, `.agents/2026-08-02-mauicross-desktop-consent-noop.md`, and this file.
- Checks performed and results: required restore succeeded; final aggregate build and Android/iOS sample builds succeeded with zero warnings/errors; all 9 Windows tests passed with only the existing MSTest `PRI263` resource warning; both Release packages were created and inspected, confirming the shared `net10.0` assembly and shared-wrapper dependencies in Windows/Mac Catalyst groups; known Android binding `$` warnings remain during Release pack.
- Open issues and recommended next step: smoke-test desktop log output and skip behavior, then complete physical Android/iOS UMP and ad-flow checks before publishing `0.1.10`.

## 2026-08-02 — Test application platform and consent alignment

- Objective and status: aligned `AppleTestApp` with the current iOS UMP/logging/ad surface and restored Windows and Mac Catalyst targets in `MAUITestApp`; completed.
- Decisions made: made the real Apple ad controller the scene root; added async consent-before-initialization, privacy-options UI, `IAppleLogger`, adaptive banner and all full-screen formats; restored desktop MAUI TFMs and Windows 10 1809 minimum; gated MAUI ad materialization on consent while letting desktop no-op consent proceed to the banner placeholder without invoking unsupported full-screen services; removed duplicate interstitial DI registration.
- Affected files: Apple test project/startup/controller/plist and new console logger; MAUI test project/page/startup/view model; `.agents/2026-08-02-test-app-platform-alignment.md`; and this file.
- Checks performed and results: required restore succeeded; AppleTestApp and all four MAUITestApp targets built with zero warnings/errors; final aggregate build succeeded with zero warnings/errors; all 9 tests passed; the Android retry required build-server cleanup after the known local timeout, and Mac Catalyst/iOS execution still requires Apple hardware/tooling.
- Open issues and recommended next step: physically exercise Apple UMP/privacy/logging/ads and verify Windows/Mac Catalyst placeholder and no-op logs before publishing `0.1.10`.

## 2026-08-02 — README and wiki first version

- Objective and status: updated the README for the current AdMob Next-Gen, UMP, logging, MAUI, test-app, and desktop behavior and created the first usage-oriented GitHub wiki; completed.
- Decisions made: kept documentation in English; used the README as a concise feature and platform overview; split detailed guidance across getting-started, privacy, MAUI, Android, iOS, ad-format, logging, desktop, and troubleshooting wiki pages; corrected obsolete low-level iOS async examples to the callback APIs actually exposed by the binding; edited the wiki on its local `master` branch without committing or pushing.
- Affected files: `README.md`, eleven Markdown files in `AMDevITAdMobWrapper.wiki`, `.agents/2026-08-02-readme-wiki-first-version.md`, and this file.
- Checks performed and results: package/SDK versions and public APIs were cross-checked against project sources; wiki page targets, balanced code fences, trailing whitespace, and tracked diff whitespace checks passed; no build was run because only documentation changed.
- Open issues and recommended next step: review the rendered wiki after publication; commit the wiki repository first and then the parent repository's resulting submodule pointer. No commit or push was performed.

## 2026-08-09 — iOS banner lifecycle crash hardening

- Objective and status: analyzed two MyCurrencyConverter TestFlight crashes
  with matching dSYMs and implemented reusable iOS banner lifecycle hardening;
  source work is complete, while restore/build/tests, XCFramework regeneration,
  and device verification remain pending.
- Decisions made: retained the public API; serialized and deduplicated banner
  initialization on the main thread; tracked and explicitly deactivated Auto
  Layout constraints; invalidated callback generations before teardown;
  detached native delegates and controller references; ignored callbacks from
  replaced banners; made disconnect idempotent; advanced coordinated versions
  to `0.1.11`.
- Affected files: native Swift `BannerAdWrapper`, the MAUICross iOS
  `BannerAdHandler`, Xcode and NuGet version metadata, README package examples,
  `.agents/2026-08-09-ios-banner-lifecycle-crashes.md`, and this file.
  MyCurrencyConverter was inspected read-only and was not modified.
- Checks performed and results: all supplied UUIDs matched; the first crash
  symbolicated to MAUI `PlatformGraphicsView` disposal and the second to Mono's
  unhandled-exception hook with Google's signal handler only reporting it;
  Swift/C# brace checks and targeted lifecycle/version checks passed. Scoped
  whitespace validation passed. Restore, build, and tests await separate user
  authorization.
- Open issues and recommended next step: authorize .NET restore/build/tests,
  then rebuild the native XCFramework and dSYM on macOS before packing 0.1.11;
  physically stress banner refresh, resize, unload, consent changes, and late
  callbacks on iOS 16 and iOS 26.
