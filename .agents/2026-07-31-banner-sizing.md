# Cross-platform banner sizing — 2026-07-31

## Objective and status

Fix MAUI banner ads being clipped on the right, center fixed-size banners, size
adaptive banners from the actual MAUI container, and support every
`BannerAdSize` value on iOS.

Status: the Android native wrapper, Android AAR, .NET bindings, and Android/iOS
MAUI handlers are updated and compile successfully. The Swift source and .NET
iOS binding API are updated, but the checked-in iOS xcframework still needs to
be rebuilt on macOS with Xcode before the new selector can run on a device.

## Decisions made

- Fixed banner sizes now report their exact logical dimensions to MAUI:
  320x50, 320x100, 300x250, 468x60, and 728x90.
- Android renders every native banner inside a centered `FrameLayout` instead
  of exposing the `AdView` directly as the handler platform view.
- Android adaptive ads are loaded only after the container has a real width.
  Pixel width is converted to density-independent width and the ad is reloaded
  when that width changes.
- Added a native Android `loadAdaptive` API which accepts the available width.
  The MAUI handler uses Google's large anchored adaptive size API; the optional
  maximum height retains inline-adaptive behavior for direct native consumers.
- Rebuilt and replaced `admob-wrapper-release.aar` and filtered the Kotlin
  compiler-only `BannerAdWrapper.WhenMappings` class from .NET binding output.
- Added an Objective-C-visible Swift `BannerAdViewSize` enum and a new banner
  load overload accepting the selected size and available width.
- iOS maps all six MAUI values to Google Mobile Ads sizes. Adaptive uses the
  actual arranged container width; fixed ads use their corresponding constants.
- iOS fixed banners are centered with intrinsic width instead of being stretched
  between leading and trailing edges. Boundary constraints prevent a banner
  that fits from escaping its container.
- Both handlers reload when `AdUnitId` or `AdSize` changes and invalidate MAUI
  measurement after native content becomes available.

## Affected files

- `sources/droid/AdMobWapperApp/admob-wrapper/src/main/java/it/amdev/admob/wrapper/ads/BannerAdWrapper.kt`
- `sources/dotnet/AMDevIT.Admob.Wrapper/AMDevIT.Admob.Wrapper.Droid/Jars/admob-wrapper-release.aar`
- `sources/dotnet/AMDevIT.Admob.Wrapper/AMDevIT.Admob.Wrapper.Droid/Transforms/Metadata.xml`
- `sources/apple/ios/AdMobWrapper/Ads/BannerAdWrapper.swift`
- `sources/dotnet/AMDevIT.Admob.Wrapper/AMDevIT.Admob.Wrapper.iOSNative/ApiDefinition.cs`
- `sources/dotnet/AMDevIT.Admob.Wrapper/AMDevIT.Admob.Wrapper.iOSNative/StructsAndEnums.cs`
- `sources/dotnet/AMDevIT.Admob.Wrapper/AMDevIT.Admob.Wrapper.MAUICross/BannerAdSize.cs`
- Android and iOS `BannerAdHandler.cs` platform implementations.

## Checks performed and results

- Gradle `:admob-wrapper:assembleRelease`: succeeded with no compilation
  warnings after switching to the current large anchored adaptive API.
- MAUI Android target build: succeeded with zero warnings and zero errors after
  regenerating the AAR and its .NET binding.
- MAUI iOS target build: succeeded with zero warnings and zero errors.
- Required repository restore: succeeded.
- Complete solution build, run serially after the first parallel invocation
  exceeded its five-minute command limit: succeeded for all projects and
  targets with zero warnings and zero errors.
- The MSTest executable completed all seven tests successfully. The first
  `dotnet test` invocation could not use the legacy VSTest target with the .NET
  10 Microsoft Testing Platform project, so the already-built MTP executable
  was run directly.
- `git diff --check`: succeeded; only line-ending conversion notices were
  reported.

## Open issues and recommended next step

1. On a macOS machine with Xcode, run
   `sources/apple/ios/build_xcframework.sh`, replace the checked-in
   `AMDevIT.Admob.Wrapper.iOSNative/libs/AdMobWrapper.xcframework`, and rebuild
   the iOS binding/package. Until this is done, the checked-in binary does not
   implement the new size-aware selector even though its source and .NET API do.
2. On physical Android and iOS phones/tablets, exercise all sizes that fit the
   device class, adaptive resizing/orientation changes, layouts with horizontal
   padding, and runtime `AdSize` changes. Full banner and leaderboard sizes are
   tablet-oriented and cannot fit a typical phone viewport.

