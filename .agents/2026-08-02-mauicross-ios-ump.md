# MAUICross iOS UMP and native logging alignment

## Objective and status

Extend the MAUICross package to use the updated iOS AdMob binding and expose a
MAUI-level privacy consent flow on both Android and iOS. Completed.

## Decisions made

- Made `IAdMobConsentService` available on Android and iOS and registered its
  platform implementation through `UseAMDevITAdMobWrapper` on both targets.
- Preserved the existing `InitializeAsync(string applicationId, ...)` API. The
  Android implementation requires the application ID; iOS ignores the argument
  because `GADApplicationIdentifier` is read from `Info.plist`.
- Added an iOS consent service backed by the shared asynchronous iOS manager
  extensions. It exposes consent gathering, status refresh, current status,
  privacy-options form, required consent form, ad-request eligibility, SDK
  initialization, and test reset.
- Added `AppleLoggerAdapter`, mapping `IAppleLogger` and `AppleLogLevel` to
  `Microsoft.Extensions.Logging` with the same native tag/message structure as
  the Android logger adapter.
- Supplied the logger adapter to the iOS manager and all four iOS ad wrappers:
  banner, app-open, interstitial, and rewarded.
- Corrected the rewarded iOS service constructor to request
  `ILogger<RewardedAdService>` instead of `ILogger<AppOpenAdService>`.
- Kept both the shared wrapper and direct iOSNative project references on the
  iOS target. The shared wrapper supplies async UMP models/extensions, while
  the direct reference is required by the MAUI compiler for native binding
  types already used by handlers and services.
- Left Mac Catalyst and Windows as unsupported AdMob fallback targets; the
  consent interface remains mobile-only.
- Updated package release notes and README MAUI guidance for the Android/iOS
  consent-before-initialization flow and privacy-options form.

## Affected files

- `README.md`
- `sources/dotnet/AMDevIT.Admob.Wrapper/Directory.Build.props`
- `AMDevIT.Admob.Wrapper.MAUICross/AMDevIT.Admob.Wrapper.MAUICross.csproj`
- `AMDevIT.Admob.Wrapper.MAUICross/IAdMobConsentService.cs`
- `AMDevIT.Admob.Wrapper.MAUICross/MauiAppBuilderExtensions.cs`
- New iOS `Diagnostics/AppleLoggerAdapter.cs`
- New iOS `Services/AdMobConsentService.cs`
- iOS `BannerAdHandler.cs`
- iOS app-open, interstitial, and rewarded service implementations
- `.agents/context.md`
- This file

## Checks performed

- MAUICross build across Android, iOS, Mac Catalyst, and Windows: succeeded
  with zero warnings/errors.
- Required aggregate restore: succeeded.
- Required aggregate build: succeeded for all library targets with zero
  warnings/errors after explicitly selecting Android Studio's JDK.
- MAUICross tests: all 7 passed. The Windows resource tool emitted the existing
  external `PRI263` warning for `MSTestAdapter.PlatformServices.resources.dll`.
- Release pack `AMDevIT.Admob.Wrapper.MAUICross.0.1.10.nupkg`: succeeded.
  The final Release pack retained the two previously documented Android
  binding-generator warnings for Kotlin synthetic `$` members.
- Package inspection confirmed Android and iOS assemblies and confirmed that
  the iOS dependency group contains both `AMDevIT.Admob.Wrapper.iOSNative` and
  `AMDevIT.Admob.Wrapper` at version `0.1.10`.
- Targeted diff checks found no whitespace errors; Windows only reports the
  expected line-ending notices.

## Open issues and recommended next step

- Run the MAUI consent flow on physical Android and iOS devices, covering first
  launch, returning users, consent errors with a previously valid state,
  privacy-options form, under-age settings, and test reset.
- Verify that native logs from all four ad formats and UMP reach the configured
  `Microsoft.Extensions.Logging` providers on both platforms.
- Do not use consent debug geography or test-device identifiers in production.
