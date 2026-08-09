# AMDev.IT AdMob Wrapper
[![License: Apache-2.0](https://img.shields.io/badge/license-Apache%20License%202.0-blue.svg)](LICENSE)
[![.NET](https://img.shields.io/badge/.NET-10-512BD4)](https://dotnet.microsoft.com)

A modern, lightweight AdMob wrapper for **.NET 10 Android**, **.NET 10 iOS**, and **.NET MAUI**, designed to solve the lack of working AdMob bindings in the current .NET ecosystem.

The library wraps the native Android and iOS SDKs in bindable Kotlin and Swift
layers and exposes them to .NET with both callback and `async/await` APIs.

---

## Highlights

- Google Mobile Ads SDK **Next-Gen** on Android.
- Native Swift XCFramework built with Google's official Mobile Ads Swift
  package on iOS.
- Google User Messaging Platform (UMP) consent flow on Android and iOS,
  including privacy options, current status, test reset, under-age settings,
  debug geography, and test-device configuration.
- Native and managed logging through `IDroidLogger`, `IAppleLogger`, and
  `Microsoft.Extensions.Logging` adapters in MAUI.
- Banner, interstitial, rewarded, and app-open ads with callback and
  `async/await` APIs.
- Adaptive and fixed banner sizes on Android and iOS.
- MAUI dependency-injection services and XAML banner control.
- Safe MAUI desktop behavior: configurable banner fallback and no-op consent
  service on Windows and Mac Catalyst.

## Platform support

| Capability | Android | iOS | Windows | Mac Catalyst |
|---|---:|---:|---:|---:|
| Native AdMob ads | Yes | Yes | No | No |
| UMP consent | Yes | Yes | No-op | No-op |
| `IAdMobConsentService.IsSupported` | `true` | `true` | `false` | `false` |
| MAUI banner | Native | Native | `FallbackTemplate` | `FallbackTemplate` |
| MAUI full-screen ads | Yes | Yes | Not supported | Not supported |
| Native logging bridge | Yes | Yes | Not applicable | Not applicable |

The Windows and Mac Catalyst consent implementation never throws. It reports a
neutral `NotRequired` state so a cross-platform application can skip UMP and
continue its non-advertising or fallback UI. Full-screen advertising services
remain mobile-only and throw `PlatformNotSupportedException` if called on a
desktop target.

## Documentation

The [project wiki](https://github.com/AMDevIT/AMDevITAdMobWrapper/wiki)
contains the complete getting-started guide, platform-specific setup, UMP
privacy workflow, MAUI examples, logging, desktop fallbacks, and troubleshooting.

---

## Packages

| Package | Description | NuGet | Downloads |
|---|---|---|---|
| `AMDevIT.Admob.Wrapper.Droid` | .NET binding for the native Kotlin AAR | [![NuGet](https://img.shields.io/nuget/v/AMDevIT.Admob.Wrapper.Droid)](https://www.nuget.org/packages/AMDevIT.Admob.Wrapper.Droid) | [![Downloads](https://img.shields.io/nuget/dt/AMDevIT.Admob.Wrapper.Droid)](https://www.nuget.org/packages/AMDevIT.Admob.Wrapper.Droid)|
| `AMDevIT.Admob.Wrapper.iOSNative` | .NET binding for the native Swift xcframework | [![NuGet](https://img.shields.io/nuget/v/AMDevIT.Admob.Wrapper.iOSNative)](https://www.nuget.org/packages/AMDevIT.Admob.Wrapper.iOSNative) |[![Downloads](https://img.shields.io/nuget/dt/AMDevIT.Admob.Wrapper.iOSNative)](https://www.nuget.org/packages/AMDevIT.Admob.Wrapper.iOSNative)|
| `AMDevIT.Admob.Wrapper` | Multi-platform wrapper with `async/await` extensions | [![NuGet](https://img.shields.io/nuget/v/AMDevIT.Admob.Wrapper)](https://www.nuget.org/packages/AMDevIT.Admob.Wrapper) |[![Downloads](https://img.shields.io/nuget/dt/AMDevIT.Admob.Wrapper)](https://www.nuget.org/packages/AMDevIT.Admob.Wrapper)
| `AMDevIT.Admob.Wrapper.MAUICross` | MAUI controls, handlers, and full-screen services | [![NuGet](https://img.shields.io/nuget/v/AMDevIT.Admob.Wrapper.MAUICross)](https://www.nuget.org/packages/AMDevIT.Admob.Wrapper.MAUICross) |[![Downloads](https://img.shields.io/nuget/dt/AMDevIT.Admob.Wrapper.MAUICross)](https://www.nuget.org/packages/AMDevIT.Admob.Wrapper.MAUICross)|

---

## Requirements

- .NET 10
- Android API 33+ (Android 13)
- iOS 15.0+
- Mac Catalyst 15.0+ or Windows 10 version 1809+ for the MAUI fallback UI

The Android binding embeds Google Mobile Ads SDK Next-Gen `1.3.1` and brings
Google UMP `4.0.0` through the official .NET Android bindings. Do not add the
legacy `Xamarin.GooglePlayServices.Ads` package.

The iOS XCFramework is built from Google's official
`swift-package-manager-google-mobile-ads` package (`13.7.0`). UMP is supplied
transitively by that package; do not add a second UMP Swift package.

---

## Installation

### Android project

```xml
<PackageReference Include="AMDevIT.Admob.Wrapper.Droid" Version="0.1.11" />
```

### iOS project

```xml
<PackageReference Include="AMDevIT.Admob.Wrapper.iOSNative" Version="0.1.11" />
```

### Android or iOS project with async/await support

```xml
<PackageReference Include="AMDevIT.Admob.Wrapper" Version="0.1.11" />
```

### MAUI project with async/await support and XAML controls

```xml
<PackageReference Include="AMDevIT.Admob.Wrapper.MAUICross" Version="0.1.11" />
```

Add `AMDevIT.Admob.Wrapper` as well only when the application uses the
lower-level native `async/await` extension methods.

### AndroidManifest.xml

Add your AdMob App ID inside the `<application>` tag:

```xml
<application ...>
    <meta-data
        android:name="com.google.android.gms.ads.APPLICATION_ID"
        android:value="ca-app-pub-XXXXXXXXXXXXXXXX~YYYYYYYYYY" />
</application>
```

### Info.plist (iOS)

Add your AdMob App ID:

```xml
<key>GADApplicationIdentifier</key>
<string>ca-app-pub-XXXXXXXXXXXXXXXX~YYYYYYYYYY</string>
```

> For testing, use the official Google test App ID: `ca-app-pub-3940256099942544~3347511713`

---

## Ad formats supported

| Format | Android class | iOS class |
|---|---|---|
| Banner | `BannerAdWrapper` | `BannerAdWrapper` |
| Interstitial | `InterstitialAdWrapper` | `InterstitialAdWrapper` |
| Rewarded | `RewardedAdWrapper` | `RewardedAdWrapper` |
| App Open | `AppOpenAdWrapper` | `AppOpenAdWrapper` |

---

## Usage — Android

### Consent and initialization

Request or refresh consent before initializing and loading ads. The async API
is available from `AMDevIT.Admob.Wrapper`:

```csharp
const string appId = "ca-app-pub-3940256099942544~3347511713";
AdMobManager manager = AdMobManager.Instance;

try
{
    ConsentGatheringResult consent = await manager.GatherConsentAsync(this);
    if (!consent.CanRequestAds)
        return;
}
catch (ConsentException exception) when (exception.CanRequestAds == true)
{
    // UMP failed, but a previous consent state still allows ad requests.
}

await manager.InitializeAsync(this.ApplicationContext!, appId);
```

Use `ConsentRequestOptions` to set the under-age flag. Debug geography and test
device IDs are also available through `ConsentDebugParameters`; never ship
debug consent settings in production.

```csharp
var options = new ConsentRequestOptions(
    TagForUnderAgeOfConsent: false,
    DebugParameters: new ConsentDebugParameters(
        ConsentDebugGeography.Eea,
        "TEST-DEVICE-HASH"));

ConsentGatheringResult consent = await manager.GatherConsentAsync(this, options);
```

Other UMP operations are exposed as
`UpdateCurrentConsentInformationAsync`, `ShowPrivacyOptionsFormAsync`,
`LoadAndShowConsentFormIfRequiredAsync`, `GetCurrentConsentInformation`,
`CanRequestAds`, and `ResetConsentForTesting`.

#### Callback style

```csharp
AdMobManager.Instance.Initialize(
    this.ApplicationContext!,
    "ca-app-pub-3940256099942544~3347511713",
    new MyInitListener());

private class MyInitListener : Java.Lang.Object, IOnInitializedListener
{
    public void OnInitialized()
    {
        // SDK ready, load ads
    }

    public void OnInitializationFailed(string error)
    {
        Console.WriteLine($"AdMob init failed: {error}");
    }
}
```

### Banner Ad

#### Callback style

```csharp
var bannerWrapper = new BannerAdWrapper(this, logger: null);
var adView = bannerWrapper.Load(
    adUnitId: "ca-app-pub-3940256099942544/6300978111",
    loadListener: new MyBannerLoadListener()
);
bannerContainer.AddView(adView);

private class MyBannerLoadListener : Java.Lang.Object, IOnAdLoadedListener
{
    public void OnAdLoaded() => Console.WriteLine("Banner loaded");
    public void OnAdFailedToLoad(int errorCode, string errorMessage) =>
        Console.WriteLine($"Banner failed: [{errorCode}] {errorMessage}");
}
```

#### Async style

```csharp
var bannerWrapper = new BannerAdWrapper(this, logger: null);
var adView = await bannerWrapper.LoadAsync("ca-app-pub-3940256099942544/6300978111");
bannerContainer.AddView(adView);
```

### Interstitial Ad

#### Callback style

```csharp
var interstitialWrapper = new InterstitialAdWrapper();
interstitialWrapper.Load(
    adUnitId: "ca-app-pub-3940256099942544/1033173712",
    loadListener: new MyLoadListener(),
    eventListener: new MyEventListener()
);

if (interstitialWrapper.IsLoaded)
    interstitialWrapper.Show(this, loadListener: null);

private class MyLoadListener : Java.Lang.Object, IOnAdLoadedListener
{
    public void OnAdLoaded() => Console.WriteLine("Interstitial loaded");
    public void OnAdFailedToLoad(int errorCode, string errorMessage) =>
        Console.WriteLine($"Interstitial failed: [{errorCode}] {errorMessage}");
}

private class MyEventListener : Java.Lang.Object, IOnAdEventListener
{
    public void OnAdShown()      => Console.WriteLine("Interstitial shown");
    public void OnAdDismissed()  => Console.WriteLine("Interstitial dismissed");
    public void OnAdClicked()    => Console.WriteLine("Interstitial clicked");
    public void OnAdImpression() => Console.WriteLine("Interstitial impression");
    public void OnAdFailedToShow(int errorCode, string errorMessage) =>
        Console.WriteLine($"Interstitial show failed: [{errorCode}] {errorMessage}");
}
```

> **Note**: Interstitial ads are one-shot. Once dismissed, you need to call `Load` again before showing. This is by design — it gives you full control over which Ad Unit ID to use on the next load.

### Rewarded Ad

#### Callback style

```csharp
var rewardedWrapper = new RewardedAdWrapper();
rewardedWrapper.Load(
    adUnitId: "ca-app-pub-3940256099942544/5224354917",
    loadListener: new MyLoadListener()
);

if (rewardedWrapper.IsLoaded)
    rewardedWrapper.Show(this, new MyRewardListener());

private class MyRewardListener : Java.Lang.Object, IOnRewardEarnedListener
{
    public void OnRewardEarned(string type, int amount) =>
        Console.WriteLine($"Reward earned: {amount} {type}");
}
```

### App Open Ad

#### Callback style

```csharp
var appOpenWrapper = new AppOpenAdWrapper();
appOpenWrapper.Load(
    adUnitId: "ca-app-pub-3940256099942544/9257395921",
    loadListener: new MyLoadListener(),
    eventListener: new MyEventListener()
);

if (appOpenWrapper.IsLoaded && !appOpenWrapper.IsShowing)
    appOpenWrapper.Show(this, loadListener: null);
```

---

## Usage — iOS

### Consent and initialization

Gather consent before initializing the SDK and loading ads:

```csharp
AdMobManager manager = AdMobManager.Instance;

try
{
    ConsentGatheringResult consent = await manager.GatherConsentAsync(this);
    if (!consent.CanRequestAds)
        return;
}
catch (ConsentException exception) when (exception.CanRequestAds == true)
{
    // UMP failed, but a previous consent state still allows ad requests.
}

await manager.InitializeAsync(this);
```

The iOS async API also exposes `UpdateCurrentConsentInformationAsync`,
`ShowPrivacyOptionsFormAsync`, `LoadAndShowConsentFormIfRequiredAsync`, and
`GetCurrentConsentInformation`. The native manager exposes `CanRequestAds()`
and the test-only `ResetConsentForTesting()` operation.

### Initialization

#### Callback style

```csharp
AdMobManager.Instance.InitializeWithViewController(this, new MyInitListener());

private class MyInitListener : NSObject, IOnInitializedListener
{
    public void OnInitialized()
    {
        // SDK ready, load ads
    }

    public void OnInitializationFailedWithError(string error)
    {
        Console.WriteLine($"AdMob init failed: {error}");
    }
}
```

#### Async style

```csharp
await AdMobManager.Instance.InitializeAsync(this);
```

### Banner Ad

#### Callback style

```csharp
var bannerWrapper = new BannerAdWrapper();
var adView = bannerWrapper.LoadWithAdUnitId(
    adUnitId: "ca-app-pub-3940256099942544/6300978111",
    viewController: this,
    loadListener: new MyBannerLoadListener(),
    eventListener: null
);
bannerContainer.AddSubview(adView);

private class MyBannerLoadListener : NSObject, IOnAdLoadedListener
{
    public void OnAdLoaded() => Console.WriteLine("Banner loaded");
    public void OnAdFailedToLoadWithErrorCode(nint errorCode, string errorMessage) =>
        Console.WriteLine($"Banner failed: [{errorCode}] {errorMessage}");
}
```

The low-level iOS ad wrappers use listener callbacks. MAUI consumers can use
the XAML banner events/commands and asynchronous full-screen services.

### Interstitial Ad

#### Callback style

```csharp
var interstitialWrapper = new InterstitialAdWrapper();
interstitialWrapper.LoadWithAdUnitId(
    adUnitId: "ca-app-pub-3940256099942544/1033173712",
    loadListener: new MyLoadListener(),
    eventListener: new MyEventListener()
);

if (interstitialWrapper.IsLoaded)
    interstitialWrapper.ShowWithViewController(this);
```

### Rewarded Ad

#### Callback style

```csharp
var rewardedWrapper = new RewardedAdWrapper();
rewardedWrapper.LoadWithAdUnitId(
    adUnitId: "ca-app-pub-3940256099942544/5224354917",
    loadListener: new MyLoadListener(),
    eventListener: null
);

if (rewardedWrapper.IsLoaded)
    rewardedWrapper.ShowWithViewController(this, new MyRewardListener());
```

### App Open Ad

#### Callback style

```csharp
var appOpenWrapper = new AppOpenAdWrapper();
appOpenWrapper.LoadWithAdUnitId(
    adUnitId: "ca-app-pub-3940256099942544/9257395921",
    loadListener: new MyLoadListener(),
    eventListener: null
);

if (appOpenWrapper.IsLoaded && !appOpenWrapper.IsShowing)
    appOpenWrapper.ShowWithViewController(this);
```

---

## Usage — MAUI (AMDevIT.Admob.Wrapper.MAUICross)

### Setup

Register the handler in `MauiProgram.cs`:

```csharp
builder.UseAMDevITAdMobWrapper();
```

Inject `IAdMobConsentService`, check whether the platform supports UMP, and
complete consent before initializing or loading ads:

```csharp
public sealed class AdMobStartup(IAdMobConsentService consentService)
{
    public async Task InitializeAsync(CancellationToken cancellationToken = default)
    {
        if (!consentService.IsSupported)
            return;

        bool canRequestAds;

        try
        {
            ConsentGatheringResult consent = await consentService.GatherConsentAsync(
                cancellationToken: cancellationToken);
            canRequestAds = consent.CanRequestAds;
        }
        catch (ConsentException exception) when (exception.CanRequestAds == true)
        {
            // UMP failed, but a previous consent state still allows ad requests.
            canRequestAds = true;
        }

        if (!canRequestAds)
            return;

        string applicationId = OperatingSystem.IsAndroid()
            ? "ca-app-pub-3940256099942544~3347511713"
            : string.Empty;
        await consentService.InitializeAsync(applicationId, cancellationToken);
    }
}
```

`IAdMobConsentService` also exposes the current consent snapshot, privacy
options form, required consent form, `CanRequestAds`, and the test-only reset
operation. It is registered on every MAUI target. Android and iOS report
`IsSupported == true`; Windows and Mac Catalyst receive a safe no-op service
that reports `IsSupported == false`, logs skipped operations, and returns a
neutral not-required consent state without throwing. The application ID
argument is required on Android; iOS ignores it and reads
`GADApplicationIdentifier` from `Info.plist`.

Run this workflow before making a banner visible or calling a full-screen load
method. Request updated consent information on every app launch, initialize
AdMob only when ads may be requested, and expose a persistent privacy-options
entry point whenever `PrivacyOptionsRequirementStatus.Required` is reported.

Call the privacy-options form from the app's privacy settings when required:

```csharp
await consentService.ShowPrivacyOptionsFormAsync(cancellationToken);
```

### Banner Ad in XAML

```xml
<?xml version="1.0" encoding="utf-8" ?>
<ContentPage xmlns="http://schemas.microsoft.com/dotnet/2021/maui"
             xmlns:x="http://schemas.microsoft.com/winfx/2009/xaml"
             xmlns:admob="clr-namespace:AMDevIT.Admob.Wrapper.MAUICross;assembly=AMDevIT.Admob.Wrapper.MAUICross"
             x:Class="YourApp.MainPage">

    <Grid RowDefinitions="*, Auto">

        <Label Grid.Row="0" Text="Hello MAUI!" />

        <admob:BannerAd Grid.Row="1"
                        AdUnitId="ca-app-pub-3940256099942544/6300978111"
                        AdSize="Adaptive"
                        AdLoaded="OnBannerLoaded"
                        AdFailed="OnBannerFailed">
            <admob:BannerAd.FallbackTemplate>
                <DataTemplate>
                    <Border Padding="12">
                        <Label Text="AdMob banner ads aren't supported on this platform." />
                    </Border>
                </DataTemplate>
            </admob:BannerAd.FallbackTemplate>
        </admob:BannerAd>

    </Grid>
</ContentPage>
```

```csharp
private void OnBannerLoaded(object sender, EventArgs e)
{
    Console.WriteLine("Banner loaded");
}

private void OnBannerFailed(object sender, AdFailedEventArgs e)
{
    Console.WriteLine($"Banner failed: [{e.ErrorCode}] {e.ErrorMessage}");
}
```

`FallbackTemplate` is rendered on Windows and Mac Catalyst, where AdMob isn't
supported. Its content is created lazily by the platform handler. If the
property isn't set, the default template creates an empty `ContentView`.
Android and iOS continue to render the native AdMob banner view and don't
instantiate the fallback template.

Full-screen ad services are available for dependency injection on every
supported MAUI target. Calling them on Windows or Mac Catalyst throws
`PlatformNotSupportedException`.

### Full-screen ads

Inject `IInterstitialAdService`, `IAppOpenAdService`, or
`IShowableRewardedAdService`, then await loading before showing the ad:

```csharp
public sealed class AdCoordinator(
    IInterstitialAdService interstitialAdService,
    IShowableRewardedAdService rewardedAdService)
{
    public Task ShowInterstitialAsync(CancellationToken cancellationToken = default)
    {
        return interstitialAdService.LoadAndShowAsync(
            "ca-app-pub-3940256099942544/1033173712",
            cancellationToken);
    }

    public async Task ShowRewardedAsync(CancellationToken cancellationToken = default)
    {
        rewardedAdService.AdRewardEarned += OnAdRewardEarned;

        await rewardedAdService.LoadAndShowAsync(
            "ca-app-pub-3940256099942544/5224354917",
            cancellationToken);
    }

    private static void OnAdRewardEarned(object? sender, AdReward reward)
    {
        Console.WriteLine($"Reward: {reward.Amount} {reward.Type}");
    }
}
```

Each registered service supports one native load operation at a time. A second
overlapping call throws `InvalidOperationException`. Cancelling the token
cancels the caller's wait, but it cannot cancel the native SDK operation; wait
for its load callback before starting another load on the same service.

### Banner Ad sizes

| Value | Description |
|---|---|
| `Adaptive` | Adapts to the container width (default) |
| `Banner` | Standard 320x50 |
| `LargeBanner` | 320x100 |
| `MediumRectangle` | 300x250 |
| `FullBanner` | 468x60 |
| `Leaderboard` | 728x90 |

### Logging and diagnostics

MAUICross automatically bridges native Android and iOS wrapper messages to the
configured `Microsoft.Extensions.Logging` providers. Register the wrapper and
the desired providers normally:

```csharp
builder.Logging.AddDebug();
builder.UseAMDevITAdMobWrapper();
```

Low-level native consumers can implement `IDroidLogger` or `IAppleLogger` and
pass the logger to `AdMobManager` and the individual ad-wrapper constructors.
Trace, debug, information, warning, error, and critical levels are supported.
Ad dismissal is raised only from the native `didDismiss` callback; the earlier
`willDismiss` callback is diagnostic-only.

---

## Error handling

The lower-level async extensions throw `AdException` on failure. MAUI
full-screen loading throws `AdLoadException`; both exceptions expose the native
error code:

```csharp
try
{
    ConsentGatheringResult consent = await AdMobManager.Instance.GatherConsentAsync(this);
    if (!consent.CanRequestAds)
        return;

    await AdMobManager.Instance.InitializeAsync(
        this.ApplicationContext!,
        "ca-app-pub-3940256099942544~3347511713");
    var adView = await bannerWrapper.LoadAsync(adUnitId);
    bannerContainer.AddView(adView);
}
catch (AdException ex)
{
    Console.WriteLine($"AdMob error [{ex.ErrorCode}]: {ex.Message}");
}
catch (ConsentException ex)
{
    Console.WriteLine($"Consent error [{ex.ErrorCode}]: {ex.Message}");
}
```

---

## Test Ad Unit IDs

Use these IDs during development. Never use real Ad Unit IDs on a device you own.

| Format | Test Ad Unit ID |
|---|---|
| App Open | `ca-app-pub-3940256099942544/9257395921` |
| Banner | `ca-app-pub-3940256099942544/6300978111` |
| Interstitial | `ca-app-pub-3940256099942544/1033173712` |
| Rewarded | `ca-app-pub-3940256099942544/5224354917` |
| Rewarded Interstitial | `ca-app-pub-3940256099942544/5354046379` |
| Native | `ca-app-pub-3940256099942544/2247696110` |

---

## Test applications

- `AMDevIT.Admob.Wrapper.DroidTestApp` exercises Android UMP, initialization,
  banners, interstitial, rewarded, and app-open ads.
- `AMDevIT.Admob.Wrapper.AppleTestApp` exercises iOS UMP, privacy options,
  native logging, adaptive banners, and every supported full-screen format.
- `AMDevIT.Admob.Wrapper.MAUITestApp` targets Android, iOS, Windows, and Mac
  Catalyst. It delays mobile ad materialization until consent succeeds and
  demonstrates the desktop no-op consent and banner fallback behavior.

Always use Google's test IDs while developing and perform final consent/ad-flow
checks on physical Android and iOS devices before publishing.

---

## Project structure

```
AMDevITAdMobWrapper/
├── sources/
│   ├── droid/                                      # Kotlin source (Android Studio)
│   │   └── admob-wrapper/
│   │       ├── AdMobManager.kt
│   │       └── ads/
│   │           ├── BannerAdWrapper.kt
│   │           ├── InterstitialAdWrapper.kt
│   │           ├── RewardedAdWrapper.kt
│   │           └── AppOpenAdWrapper.kt
│   ├── apple/ios/                                  # Swift source (Xcode)
│   │   ├── build_xcframework.sh
│   │   └── AdMobWrapper/
│   │       ├── AdMobManager.swift
│   │       └── Ads/
│   │           ├── BannerAdWrapper.swift
│   │           ├── InterstitialAdWrapper.swift
│   │           ├── RewardedAdWrapper.swift
│   │           └── AppOpenAdWrapper.swift
│   └── dotnet/AMDevIT.Admob.Wrapper/
│       ├── AMDevIT.Admob.Wrapper.Droid/            # .NET binding project (Android)
│       ├── AMDevIT.Admob.Wrapper.iOSNative/        # .NET binding project (iOS)
│       ├── AMDevIT.Admob.Wrapper/                  # Multi-platform wrapper + async extensions
│       ├── AMDevIT.Admob.Wrapper.MAUICross/        # MAUI controls and services
│       ├── AMDevIT.Admob.Wrapper.DroidTestApp/     # Android test app
│       ├── AMDevIT.Admob.Wrapper.AppleTestApp/     # iOS test app
│       ├── AMDevIT.Admob.Wrapper.MAUITestApp/      # Android/iOS/desktop MAUI test app
│       └── AMDevIT.Admob.Wrapper.MAUICross.Tests/  # async lifecycle tests
```

---

## Notes about building

### Android

The native Android SDK is built as an AAR using Gradle. When making changes to
the native code, rebuild the release AAR and replace
`AMDevIT.Admob.Wrapper.Droid/Jars/admob-wrapper-release.aar`.

### iOS

The native iOS SDK is built as an xcframework using Xcode. A build script is
provided at `sources/apple/ios/build_xcframework.sh`. Run it from that directory:

```bash
./build_xcframework.sh
```

Then replace
`sources/dotnet/AMDevIT.Admob.Wrapper/AMDevIT.Admob.Wrapper.iOSNative/libs/AdMobWrapper.xcframework`.

---

## Contributing

Contributions are welcome. Please open an issue before submitting a pull request for significant changes.

When updating the native Android SDK version:
1. Update `adsMobileSdkVersion` in `libs.versions.toml`
2. Recompile the AAR from Android Studio
3. Replace the release AAR in `AMDevIT.Admob.Wrapper.Droid/Jars/`
4. Update the `AndroidMavenLibrary` Next-Gen version and its explicit .NET
   Android dependency bindings in `AMDevIT.Admob.Wrapper.Droid.csproj`
5. Verify the generated NuGet embeds the expected Next-Gen AAR and does not
   depend on the legacy `Xamarin.GooglePlayServices.Ads` package
6. Bump the package version and publish

When updating the native iOS SDK version:
1. Update the SPM dependency version in Xcode
2. Run `./build_xcframework.sh` from `sources/apple/ios/`
3. Replace the xcframework in `AMDevIT.Admob.Wrapper.iOSNative/libs/`
4. Bump the package version and publish

---

## License

Apache 2.0 License — see [LICENSE](LICENSE) for details.

This library is not affiliated with or endorsed by Google. AdMob is a trademark of Google LLC.
