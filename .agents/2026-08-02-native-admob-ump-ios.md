# Native AdMob and UMP alignment — 2026-08-02

## Objective and status

Verify the native Android implementation of Google Mobile Ads Next-Gen and
User Messaging Platform, then align the native Swift framework with the
Android consent, diagnostics, and lifecycle surface.

Status: native source changes are complete. Android verification passed. The
iOS framework still requires an Xcode build and XCFramework regeneration on
macOS, which the user will perform separately. No AAR, XCFramework, or .NET
project was changed.

## Decisions made

- Kept Android on Google Mobile Ads Next-Gen `1.3.1` and did not add an
  explicit Gradle UMP dependency. Gradle resolves UMP `4.0.0` transitively from
  the Next-Gen SDK.
- Kept the iOS project on Google's official
  `swift-package-manager-google-mobile-ads` repository and updated its minimum
  version from `13.2.0` to `13.7.0`.
- Did not add a second UMP Swift package reference. The official Google Mobile
  Ads package already declares `GoogleUserMessagingPlatform` as a transitive
  dependency and currently resolves UMP `3.1.0`.
- Ported the Android consent API to Swift: consent information refresh and
  snapshot, required form presentation, privacy options, `canRequestAds`,
  combined consent gathering, debug geography/device configuration, and test
  reset.
- Kept all UMP UI and state access on the main thread. Current Swift UMP form
  APIs use `async`/`await` on `MainActor`.
- Added `IAppleLogger` and `LogLevel` as Objective-C-visible counterparts of
  `IDroidLogger` and its log level.
- Added logger-aware initializers to the manager and every ad wrapper.
- Removed `onAdDismissed` from every `willDismiss` callback. Those callbacks
  now only emit `logDebug`; `onAdDismissed` is emitted by `didDismiss`.
- Aligned the iOS framework marketing version with the Android native module:
  `0.1.10`.
- Changed the XCFramework script to use `pipefail` and stopped suppressing
  `xcodebuild` failures piped through `xcpretty`.

## Affected files

- `sources/apple/ios/AdMobWrapper.xcodeproj/project.pbxproj`
- `sources/apple/ios/AdMobWrapper/AdMobManager.swift`
- `sources/apple/ios/AdMobWrapper/Diagnostics/IAppleLogger.swift`
- `sources/apple/ios/AdMobWrapper/Diagnostics/LogLevel.swift`
- `sources/apple/ios/AdMobWrapper/Listeners/OnConsentFormEventListener.swift`
- `sources/apple/ios/AdMobWrapper/Listeners/OnConsentGatheringListener.swift`
- `sources/apple/ios/AdMobWrapper/Listeners/OnConsentInformationRequestListener.swift`
- `sources/apple/ios/AdMobWrapper/Privacy/ConsentInformationRequestDebugParameters.swift`
- `sources/apple/ios/AdMobWrapper/Privacy/ConsentStatusData.swift`
- All four Swift files under `sources/apple/ios/AdMobWrapper/Ads`.
- `sources/apple/ios/build_xcframework.sh`

## Checks performed and results

- `:admob-wrapper:assembleRelease`: succeeded with 26 tasks up to date.
- `dependencyInsight` for `com.google.android.ump:user-messaging-platform`:
  resolved `4.0.0` through
  `com.google.android.libraries.ads.mobile.sdk:ads-mobile-sdk:1.3.1`.
- Verified that the Android Gradle module has no explicit UMP dependency.
- Verified that the Xcode project contains only the official Google Mobile Ads
  SPM repository, with minimum version `13.7.0`.
- Verified textually that each `willDismiss` contains `logDebug` and no
  `onAdDismissed`, while each corresponding `didDismiss` emits
  `onAdDismissed`.
- Swift source brace checks passed, and the scoped Git diff check reported no
  whitespace errors.
- Xcode/Swift compilation could not be performed on the Windows host because
  neither `swift` nor `xcodebuild` is installed. .NET checks were intentionally
  not run because the user limited this step to native projects.

## Open issues and recommended next step

1. On the Mac Air, resolve the Swift package and build the framework with the
   Xcode version required by Google Mobile Ads `13.7.0`.
2. Run `sources/apple/ios/build_xcframework.sh` and verify device and simulator
   archives, Objective-C interface generation, UMP callbacks, and every ad
   dismissal lifecycle.
3. Regenerate and replace the checked-in XCFramework only after the native
   source build succeeds. Update .NET bindings in a later, explicitly scoped
   step.
