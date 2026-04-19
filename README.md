# AMDev.IT Admob Wrapper

A modern, lightweight AdMob wrapper for **.NET 10 Android** and **.NET MAUI**, designed to solve the lack of working AdMob bindings in the current .NET ecosystem.

> **Why this exists**: As of 2025, there are no fully functional, up-to-date AdMob bindings for .NET MAUI. Based on my research, the official `Xamarin.GooglePlayServices.Ads` binding has broken classes (`InterstitialAdLoadCallback.OnAdLoaded` and others), and the iOS binding repository has been archived with no replacement. This library wraps the native Android SDK in a clean, bindable Kotlin layer and exposes it to .NET with both callback and `async/await` APIs.

---

## Packages

| Package | Description | NuGet |
|---|---|---|
| `AMDevIT.Admob.Wrapper.Android` | .NET binding for the native Kotlin AAR | [![NuGet](https://img.shields.io/nuget/v/AMDevIT.Admob.Wrapper.Android)](https://www.nuget.org/packages/AMDevIT.Admob.Wrapper.Android) |
| `AMDevIT.Admob.Wrapper` | Multi-platform wrapper with `async/await` extensions | [![NuGet](https://img.shields.io/nuget/v/AMDevIT.Admob.Wrapper)](https://www.nuget.org/packages/AMDevIT.Admob.Wrapper) |

---

## Requirements

- .NET 10
- Android API 33+ (Android 13)
- `Xamarin.GooglePlayServices.Ads` 125.0.0.1

---

## Installation

### Android project

For Android, you can use the `AMDevIT.Admob.Wrapper.Android` package directly, which includes the native AAR and bindings:

```xml
<PackageReference Include="AMDevIT.Admob.Wrapper.Android" Version="1.0.0" />
```

or you can use the higher-level `AMDevIT.Admob.Wrapper` package, which depends on the Android bindings and also includes async extensions:
```xml
<PackageReference Include="AMDevIT.Admob.Wrapper" Version="1.0.0" />
```

### MAUI project

For the MAUI project, use the `AMDevIT.Admob.Wrapper` package, which includes the Android bindings and async extensions. 
The iOS part of the wrapper is currently a no-op, so it won't cause any issues on that platform.

```xml
<PackageReference Include="AMDevIT.Admob.Wrapper" Version="1.0.0" />
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

> For testing, use the official Google test App ID: `ca-app-pub-3940256099942544~3347511713`

---

## Ad formats supported

| Format | Class |
|---|---|
| Banner | `BannerAdWrapper` |
| Interstitial | `InterstitialAdWrapper` |
| Rewarded | `RewardedAdWrapper` |
| App Open | `AppOpenAdWrapper` |

---

## Usage

Actually, only Android bindings are supported. The iOS part of the wrapper is currently a no-op, so you can call the same APIs on both platforms without conditional compilation.

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

---

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
    public void OnAdLoaded()
    {
        Console.WriteLine("Banner loaded");
    }

    public void OnAdFailedToLoad(int errorCode, string errorMessage)
    {
        Console.WriteLine($"Banner failed: [{errorCode}] {errorMessage}");
    }
}
```

#### Async style

```csharp
var bannerWrapper = new BannerAdWrapper(this);
var adView = await bannerWrapper.LoadAsync("ca-app-pub-3940256099942544/6300978111");
bannerContainer.AddView(adView);
```

---

### Interstitial Ad

#### Callback style

```csharp
var interstitialWrapper = new InterstitialAdWrapper(this);
interstitialWrapper.Load(
    adUnitId: "ca-app-pub-3940256099942544/1033173712",
    loadListener: new MyLoadListener(),
    eventListener: new MyEventListener()
);

// Later, when ready to show
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

---

### Rewarded Ad

#### Callback style

```csharp
var rewardedWrapper = new RewardedAdWrapper(this);
rewardedWrapper.Load(
    adUnitId: "ca-app-pub-3940256099942544/5224354917",
    loadListener: new MyLoadListener()
);

// Later, when ready to show
if (rewardedWrapper.IsLoaded())
{
    rewardedWrapper.Show(this, new MyRewardListener());
}

private class MyRewardListener : Java.Lang.Object, IOnRewardEarnedListener
{
    public void OnRewardEarned(string type, int amount)
    {
        Console.WriteLine($"Reward earned: {amount} {type}");
    }
}
```

#### Async style

```csharp
var rewardedWrapper = new RewardedAdWrapper(this);

await rewardedWrapper.LoadAsync("ca-app-pub-3940256099942544/5224354917");

var (type, amount) = await rewardedWrapper.ShowAsync(this);
Console.WriteLine($"Reward earned: {amount} {type}");
```

---

### App Open Ad

#### Callback style

```csharp
var appOpenWrapper = new AppOpenAdWrapper(this);
appOpenWrapper.Load(
    adUnitId: "ca-app-pub-3940256099942544/9257395921",
    loadListener: new MyLoadListener(),
    eventListener: new MyEventListener()
);

// Show when app comes to foreground
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

## Logging

To log AdMob events and errors, use the application implemented listener events. 
For example, for interstitial ads:
```csharp
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

You can also implement logging using dependency injection, just implement listeners that depends from ILogger and register them in the container, then pass them to the
ad wrappers when loading ads.

---

## Test Ad Unit IDs

Use these IDs during development. Never use real Ad Unit IDs on a device you own.

| Format | Test Ad Unit ID |
|---|---|
| App Open | `ca-app-pub-3940256099942544/9257395921` |
| Banner | `ca-app-pub-3940256099942544/6300978111` |
| Interstitial | `ca-app-pub-3940256099942544/1033173712` |
| Rewarded | `ca-app-pub-3940256099942544/5224354917` |
| Rewarded Interstitial | `ca-app-pub-5354046379` |
| Native | `ca-app-pub-3940256099942544/2247696110` |

---

## Project structure

```
AMDevIT.Admob.Wrapper/
├── native/
│   └── android/                              # Kotlin source (Android Studio)
│       └── admob-wrapper/
│           ├── AdMobManager.kt
│           └── ads/
│               ├── BannerAdWrapper.kt
│               ├── InterstitialAdWrapper.kt
│               ├── RewardedAdWrapper.kt
│               └── AppOpenAdWrapper.kt
└── dotnet/
    ├── AMDevIT.Admob.Wrapper.Android/        # .NET binding project
    ├── AMDevIT.Admob.Wrapper/                # Multi-platform wrapper + async extensions
    └── AMDevIT.Admob.Wrapper.TestApp/        # Android test app
```

---

## Notes about building

The native Android SDK is built as an AAR using Gradle. The .NET binding project references the AAR and generates C# bindings automatically. 
When making changes to the native code, you need to recompile the AAR and update the reference in the .NET project.
Actually, it's pretty difficult to automate the AAR build and reference update, so for now it's a manual process. The binding library is 
actually including the latest release flavor of the AAR bacuse it is the better choice for the NuGET package. 

## Contributing

Contributions are welcome. Please open an issue before submitting a pull request for significant changes.

When updating the native Android SDK version:
1. Update `playServicesAdsVersion` in `libs.versions.toml`
2. Recompile the AAR: `./gradlew :admob-wrapper:assembleRelease`
3. Replace the AAR in `AMDevIT.Admob.Wrapper.Android/Jars/`
4. Update the `Xamarin.GooglePlayServices.Ads` NuGet version accordingly
5. Bump the package version and publish

---

## License

MIT License — see [LICENSE](LICENSE) for details.

This library is not affiliated with or endorsed by Google. AdMob is a trademark of Google LLC.