# Test application platform and consent alignment

## Objective and status

Align the native Apple test application with the current iOS AdMob/UMP binding
and restore Windows and Mac Catalyst targets in the MAUI test application.
Completed.

## Decisions made

- Added the shared wrapper reference to `AppleTestApp` so it can exercise the
  asynchronous consent and initialization extensions in addition to the native
  iOS binding.
- Replaced the inactive Apple startup screen with the real
  `MainViewController`. The previous scene delegate only displayed a static
  `Hello, iOS!` label and never instantiated the existing ad controller.
- Made the Apple test UI programmatic and removed the unused main-storyboard
  declaration. The test now gathers consent before SDK initialization, honors
  the previous-valid-consent error fallback, exposes required privacy options,
  tests the native `IAppleLogger`, uses the adaptive banner overload, and keeps
  interstitial, rewarded, and app-open controls disabled until initialization.
- Restored `net10.0-maccatalyst` and conditionally restored
  `net10.0-windows10.0.26100.0` in `MAUITestApp`, with Windows 10 1809 as the
  minimum runtime to match MAUICross.
- Moved MAUI consent gathering and SDK initialization to page startup. The
  banner and full-screen command remain unavailable until the mobile consent
  flow succeeds. Windows and Mac Catalyst detect the no-op consent service,
  report the desktop-placeholder state, show the banner fallback, and never
  call unsupported full-screen services.
- Removed the test app's duplicate transient interstitial registration so the
  registration supplied by `UseAMDevITAdMobWrapper` remains authoritative.

## Affected files

- `AMDevIT.Admob.Wrapper.AppleTestApp/AMDevIT.Admob.Wrapper.AppleTestApp.csproj`
- `AMDevIT.Admob.Wrapper.AppleTestApp/Info.plist`
- `AMDevIT.Admob.Wrapper.AppleTestApp/SceneDelegate.cs`
- `AMDevIT.Admob.Wrapper.AppleTestApp/Controllers/MainViewController.cs`
- New `AMDevIT.Admob.Wrapper.AppleTestApp/Diagnostics/ConsoleAppleLogger.cs`
- `AMDevIT.Admob.Wrapper.MAUITestApp/AMDevIT.Admob.Wrapper.MAUITestApp.csproj`
- `AMDevIT.Admob.Wrapper.MAUITestApp/MainPage.xaml`
- `AMDevIT.Admob.Wrapper.MAUITestApp/MainPage.xaml.cs`
- `AMDevIT.Admob.Wrapper.MAUITestApp/MauiProgram.cs`
- `AMDevIT.Admob.Wrapper.MAUITestApp/ViewModels/MainPageViewModel.cs`
- `.agents/context.md`
- This file

## Checks performed

- Required aggregate restore: succeeded. A forced MAUI test-app restore was
  also run after changing `TargetFrameworks` so its desktop assets were
  regenerated.
- `AppleTestApp` `net10.0-ios` build: succeeded with zero warnings/errors.
- `MAUITestApp` builds for Android, iOS, Mac Catalyst, and Windows: all
  succeeded with zero warnings/errors. The first Android attempt was terminated
  by the known local toolchain timeout; after the official build-server
  shutdown, the isolated serial retry succeeded.
- Final serial aggregate solution build: succeeded with zero warnings/errors.
- MAUICross tests: all 9 passed. An initial run with
  `--disable-build-servers` was incompatible with test discovery and returned
  no tests; the repository's standard `dotnet test --no-restore` command passed.
- Scoped `git diff --check`: passed with only expected Windows line-ending
  notices.

## Open issues and recommended next step

- Run `AppleTestApp` on an iOS device or simulator and verify first-run consent,
  privacy-options presentation, logs, adaptive banner layout, and every
  full-screen format.
- Run the Mac Catalyst build on macOS and the Windows build locally to verify
  the visible placeholder/status text and unavailable-consent warning.
- Complete the existing physical Android/iOS release smoke-test matrix before
  publishing `0.1.10`.
