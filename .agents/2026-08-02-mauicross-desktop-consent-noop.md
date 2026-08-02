# MAUICross desktop consent no-op service

## Objective and status

Make `IAdMobConsentService` safely resolvable on every MAUICross target and let
applications detect unsupported consent platforms without exceptions.
Completed.

## Decisions made

- Added `IsSupported` to `IAdMobConsentService`. Android and iOS return `true`;
  Windows and Mac Catalyst return `false`.
- Registered `IAdMobConsentService` unconditionally through
  `UseAMDevITAdMobWrapper`.
- Added one Windows/Mac Catalyst implementation that logs a warning when the
  no-op service is created and a debug message for each skipped operation.
- Defined a neutral unsupported state: initialization is considered complete,
  ads are not blocked, consent and privacy options are `NotRequired`, gathering
  returns `CanRequestAds == true`, and all operations complete without throwing
  even when passed an already-cancelled token.
- Added `net10.0` to the shared wrapper so consent contracts and models are
  available to desktop MAUI targets. MAUICross now references the shared wrapper
  for every target while retaining the direct iOS native binding reference only
  on iOS.
- Updated the sample and README to check `IsSupported` before starting UMP.

## Affected files

- `README.md`
- `sources/dotnet/AMDevIT.Admob.Wrapper/Directory.Build.props`
- Shared wrapper and MAUICross project files
- `AMDevIT.Admob.Wrapper.MAUICross/IAdMobConsentService.cs`
- `AMDevIT.Admob.Wrapper.MAUICross/MauiAppBuilderExtensions.cs`
- Android and iOS consent services
- New `AMDevIT.Admob.Wrapper.MAUICross/Services/AdMobConsentService.Unsupported.cs`
- MAUI test app `MainPageViewModel.cs`
- MAUICross test project and new unsupported-consent tests
- `.agents/context.md`
- This file

## Checks performed

- Required aggregate restore: succeeded.
- Final serial aggregate build: succeeded for Android, iOS, Mac Catalyst,
  Windows, and the shared `net10.0` target with zero warnings/errors.
- MAUI sample builds: Android and iOS both succeeded with zero warnings/errors.
- MAUICross tests: all 9 passed. The existing external `PRI263` warning remains
  for an MSTest adapter resource.
- Release packs for the shared wrapper and MAUICross: succeeded. The shared
  package contains `lib/net10.0/AMDevIT.Admob.Wrapper.dll`; the MAUICross Mac
  Catalyst and Windows dependency groups both require the shared wrapper at
  version `0.1.10`.
- Release packing retained the two already documented Android binding warnings
  for dropped Kotlin synthetic `$` members.
- `git diff --check` reported no whitespace errors; Windows still reports the
  known long XCFramework filenames and line-ending notices.

## Open issues and recommended next step

- Verify the warning/debug log output once in real Windows and Mac Catalyst
  consumers and confirm those apps skip the UMP UI while continuing normally.
- Complete the previously planned physical Android/iOS UMP and ads checks before
  publishing `0.1.10`.
