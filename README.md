# AMDev.IT AdMob Wrapper

A modern, lightweight AdMob wrapper for **.NET 10 Android**, **.NET 10 iOS**, and **.NET MAUI**, designed to solve the lack of working AdMob bindings in the current .NET ecosystem.

> **Why this exists**: As of 2025, there are no fully functional, up-to-date AdMob bindings for .NET MAUI. The official `Xamarin.GooglePlayServices.Ads` binding has broken classes (`InterstitialAdLoadCallback.OnAdLoaded` and others), and the iOS binding repository has been archived with no replacement. This library wraps the native Android and iOS SDKs in clean, bindable Kotlin and Swift layers and exposes them to .NET with both callback and `async/await` APIs.

---

## Packages

| Package | Description | NuGet |
|---|---|---|
| `AMDevIT.Admob.Wrapper.Android` | .NET binding for the native Kotlin AAR | [![NuGet](https://img.shields.io/nuget/v/AMDevIT.Admob.Wrapper.Android)](https://www.nuget.org/packages/AMDevIT.Admob.Wrapper.Android) |
| `AMDevIT.Admob.Wrapper.iOS` | .NET binding for the native Swift xcframework | [![NuGet](https://img.shields.io/nuget/v/AMDevIT.Admob.Wrapper.iOS)](https://www.nuget.org/packages/AMDevIT.Admob.Wrapper.iOS) |
| `AMDevIT.Admob.Wrapper` | Multi-platform wrapper with `async/await` extensions | [![NuGet](https://img.shields.io/nuget/v/AMDevIT.Admob.Wrapper)](https://www.nuget.org/packages/AMDevIT.Admob.Wrapper) |
| `AMDevIT.Admob.Wrapper.MAUICross` | MAUI controls and handlers (optional) | [![NuGet](https://img.shields.io/nuget/v/AMDevIT.Admob.Wrapper.MAUICross)](https://www.nuget.org/packages/AMDevIT.Admob.Wrapper.MAUICross) |

---

## Requirements

- .NET 10
- Android API 33+ (Android 13)
- iOS 15.0+
- `Xamarin.GooglePlayServices.Ads` 125.0.0.1

---

## Installation

### Android project

```xml
<PackageReference Include="AMDevIT.Admob.Wrapper.Android" Version="1.0.0" />
```

### iOS project

```xml
<PackageReference Include="AMDevIT.Admob.Wrapper.iOS" Version="1.0.0" />
```

### Android or iOS project with async/await support

```xml
<PackageReference Include="AMDevIT.Admob.Wrapper" Version="1.0.0" />
```

### MAUI project with async/await support and XAML controls

```xml
<PackageReference Include="AMDevIT.Admob.Wrapper" Version="1.0.0" />
<PackageReference Include="AMDevIT.Admob.Wrapper.MAUICross" Version="1.0.0" />
```

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

### Initialization

#### Callback style

```csharp
AdMobManager.Instance.Initialize(this, new MyInitListener());

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

#### Async style

```csharp
await AdMobManager.Instance.InitializeAsync(this);
```

### Banner Ad

#### Callback style

```csharp
var bannerWrapper = new BannerAdWrapper(this);
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
var bannerWrapper = new BannerAdWrapper(this);
var adView = await bannerWrapper.LoadAsync("ca-app-pub-3940256099942544/6300978111");
bannerContainer.AddView(adView);
```

### Interstitial Ad

#### Callback style

```csharp
var interstitialWrapper = new InterstitialAdWrapper(this);
interstitialWrapper.Load(
    adUnitId: "ca-app-pub-3940256099942544/1033173712",
    loadListener: new MyLoadListener(),
    eventListener: new MyEventListener()
);

if (interstitialWrapper.IsLoaded())
    interstitialWrapper.Show(this);

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

#### Async style

```csharp
var interstitialWrapper = new InterstitialAdWrapper(this);
await interstitialWrapper.LoadAsync("ca-app-pub-3940256099942544/1033173712");
interstitialWrapper.Show(this);
```

> **Note**: Interstitial ads are one-shot. Once dismissed, you need to call `Load` again before showing. This is by design — it gives you full control over which Ad Unit ID to use on the next load.

### Rewarded Ad

#### Callback style

```csharp
var rewardedWrapper = new RewardedAdWrapper(this);
rewardedWrapper.Load(
    adUnitId: "ca-app-pub-3940256099942544/5224354917",
    loadListener: new MyLoadListener()
);

if (rewardedWrapper.IsLoaded())
    rewardedWrapper.Show(this, new MyRewardListener());

private class MyRewardListener : Java.Lang.Object, IOnRewardEarnedListener
{
    public void OnRewardEarned(string type, int amount) =>
        Console.WriteLine($"Reward earned: {amount} {type}");
}
```

#### Async style

```csharp
var rewardedWrapper = new RewardedAdWrapper(this);
await rewardedWrapper.LoadAsync("ca-app-pub-3940256099942544/5224354917");
var (type, amount) = await rewardedWrapper.ShowAsync(this);
Console.WriteLine($"Reward earned: {amount} {type}");
```

### App Open Ad

#### Callback style

```csharp
var appOpenWrapper = new AppOpenAdWrapper(this);
appOpenWrapper.Load(
    adUnitId: "ca-app-pub-3940256099942544/9257395921",
    loadListener: new MyLoadListener(),
    eventListener: new MyEventListener()
);

if (appOpenWrapper.IsLoaded() && !appOpenWrapper.IsShowing())
    appOpenWrapper.Show(this);
```

#### Async style

```csharp
var appOpenWrapper = new AppOpenAdWrapper(this);
await appOpenWrapper.LoadAsync("ca-app-pub-3940256099942544/9257395921");
if (!appOpenWrapper.IsShowing())
    appOpenWrapper.Show(this);
```

---

## Usage — iOS

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

#### Async style

```csharp
var bannerWrapper = new BannerAdWrapper();
var adView = await bannerWrapper.LoadAsync("ca-app-pub-3940256099942544/6300978111", this);
bannerContainer.AddSubview(adView);
```

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

#### Async style

```csharp
var interstitialWrapper = new InterstitialAdWrapper();
await interstitialWrapper.LoadAsync("ca-app-pub-3940256099942544/1033173712");
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

#### Async style

```csharp
var rewardedWrapper = new RewardedAdWrapper();
await rewardedWrapper.LoadAsync("ca-app-pub-3940256099942544/5224354917");
var (type, amount) = await rewardedWrapper.ShowAsync(this);
Console.WriteLine($"Reward earned: {amount} {type}");
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

#### Async style

```csharp
var appOpenWrapper = new AppOpenAdWrapper();
await appOpenWrapper.LoadAsync("ca-app-pub-3940256099942544/9257395921");
if (!appOpenWrapper.IsShowing)
    appOpenWrapper.ShowWithViewController(this);
```

---

## Usage — MAUI (AMDevIT.Admob.Wrapper.MAUICross)

### Setup

Register the handler in `MauiProgram.cs`:

```csharp
builder.UseAdMobWrapper();
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
                        AdFailed="OnBannerFailed" />

    </Grid>
</ContentPage>
```

```csharp
private void OnBannerLoaded(object sender, EventArgs e)
{
    Console.WriteLine("Banner loaded");
}

private void OnBannerFailed(object sender, BannerAdFailedEventArgs e)
{
    Console.WriteLine($"Banner failed: [{e.ErrorCode}] {e.ErrorMessage}");
}
```

### Banner Ad sizes

| Value | Description |
|---|---|
| `Adaptive` | Adapts to the container width (default) |
| `Banner` | Standard 320x50 |
| `LargeBanner` | 320x100 |
| `MediumRectangle` | 300x250 |
| `FullBanner` | 468x60 |
| `Leaderboard` | 728x90 |

---

## Error handling

All async methods throw `AdException` on failure:

```csharp
try
{
    await AdMobManager.Instance.InitializeAsync(this);
    var adView = await bannerWrapper.LoadAsync(adUnitId);
    bannerContainer.AddView(adView);
}
catch (AdException ex)
{
    Console.WriteLine($"AdMob error [{ex.ErrorCode}]: {ex.Message}");
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

## Project structure

```
AMDevIT.Admob.Wrapper/
├── native/
│   ├── android/                                    # Kotlin source (Android Studio)
│   │   └── admob-wrapper/
│   │       ├── AdMobManager.kt
│   │       └── ads/
│   │           ├── BannerAdWrapper.kt
│   │           ├── InterstitialAdWrapper.kt
│   │           ├── RewardedAdWrapper.kt
│   │           └── AppOpenAdWrapper.kt
│   └── ios/                                        # Swift source (Xcode)
│       ├── build_xcframework.sh
│       └── AdMobWrapper/
│           ├── AdMobManager.swift
│           └── Ads/
│               ├── BannerAdWrapper.swift
│               ├── InterstitialAdWrapper.swift
│               ├── RewardedAdWrapper.swift
│               └── AppOpenAdWrapper.swift
└── dotnet/
    ├── AMDevIT.Admob.Wrapper.Android/              # .NET binding project (Android)
    ├── AMDevIT.Admob.Wrapper.iOSNative/            # .NET binding project (iOS)
    ├── AMDevIT.Admob.Wrapper/                      # Multi-platform wrapper + async extensions
    ├── AMDevIT.Admob.Wrapper.MAUICross/            # MAUI controls and handlers (optional)
    ├── AMDevIT.Admob.Wrapper.DroidTestApp/         # Android test app
    └── AMDevIT.Admob.Wrapper.iOSTestApp/           # iOS test app
```

---

## Notes about building

### Android

The native Android SDK is built as an AAR using Gradle. The binding project references both debug and release flavors — the correct one is selected automatically based on the build configuration. When making changes to the native code, recompile the AAR from Android Studio and update the files in `AMDevIT.Admob.Wrapper.Android/Jars/`.

### iOS

The native iOS SDK is built as an xcframework using Xcode. A build script is provided at `native/ios/build_xcframework.sh` to automate the process. When making changes to the Swift code, run the script from the `native/ios/` directory:

```bash
./build_xcframework.sh
```

Then copy the generated `build/AdMobWrapper.xcframework` to `AMDevIT.Admob.Wrapper.iOSNative/native/`.

---

## Contributing

Contributions are welcome. Please open an issue before submitting a pull request for significant changes.

When updating the native Android SDK version:
1. Update `playServicesAdsVersion` in `libs.versions.toml`
2. Recompile the AAR from Android Studio
3. Replace the AARs in `AMDevIT.Admob.Wrapper.Android/Jars/`
4. Update the `Xamarin.GooglePlayServices.Ads` NuGet version accordingly
5. Bump the package version and publish

When updating the native iOS SDK version:
1. Update the SPM dependency version in Xcode
2. Run `./build_xcframework.sh` from `native/ios/`
3. Replace the xcframework in `AMDevIT.Admob.Wrapper.iOSNative/native/`
4. Bump the package version and publish

---

## License

MIT License — see [LICENSE](LICENSE) for details.

This library is not affiliated with or endorsed by Google. AdMob is a trademark of Google LLC.