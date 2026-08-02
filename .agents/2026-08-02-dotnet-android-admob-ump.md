# .NET Android AdMob Next-Gen and UMP alignment

## Objective and status

Align the .NET Android binding, async wrapper, MAUI layer, samples, tests, and
NuGet metadata with the native Android AdMob Next-Gen `1.3.1` implementation
and its UMP consent API. Completed for Android. The .NET iOS binding was
explicitly left unchanged until the user regenerates the XCFramework on macOS.

## Decisions made

- Replaced the checked-in legacy .NET AAR with the native Android `0.1.10`
  release AAR.
- Removed `Xamarin.GooglePlayServices.Ads`; embedded
  `com.google.android.libraries.ads.mobile.sdk:ads-mobile-sdk:1.3.1` with
  `AndroidMavenLibrary` and added its explicit official .NET dependency
  bindings because `AndroidMavenLibrary` does not resolve Maven transitives.
- Added `com.google.android.play:hsdp:2.0.1`, introduced by Next-Gen `1.3.1`,
  as a non-bindable runtime artifact. Added its AndroidX AppCompat resources
  dependency.
- Added the official `Xamarin.Google.UserMessagingPlatform 4.0.0.3` binding,
  matching the native UMP `4.0.0` artifact. This is a .NET binding dependency;
  no explicit UMP dependency was added to the native Gradle project.
- Retained the two binding-generator warnings for Kotlin compiler synthetic
  members containing `$`. The resolution report confirms only synthetic
  access/default/switch members are dropped; public wrapper APIs bind and
  compile. Duplicate `Companion` warnings were removed through binding
  metadata.
- Added platform-neutral consent models and Android async extensions for
  update, gather, required form, privacy options, status, and initialization.
  `ConsentException` preserves UMP error code and the available ad-request and
  privacy-option state.
- Added Android-only `IAdMobConsentService` to MAUI DI. It performs consent and
  Next-Gen initialization against the current activity.
- Added a `Microsoft.Extensions.Logging` to `IDroidLogger` adapter and supplied
  it to all Android MAUI ad wrappers and the consent manager.
- Updated native-wrapper construction for Next-Gen: only banners retain a
  `Context`; app-open, interstitial, and rewarded wrappers no longer do.
- Updated Android and MAUI samples to gather consent before SDK initialization
  and to continue using a previous valid consent state when UMP reports an
  error with `CanRequestAds == true`.
- Aligned public .NET package versions to `0.1.10` and updated the README.
- Added solution-local Microsoft Testing Platform selection in `global.json`
  so `dotnet test` works with .NET 10 and `MSTest.Sdk 4.3.3`.

## Affected files

- `README.md`
- `sources/dotnet/AMDevIT.Admob.Wrapper/global.json`
- `sources/dotnet/AMDevIT.Admob.Wrapper/Directory.Build.props`
- Android binding project, AAR, and `Transforms/Metadata.xml`
- Shared wrapper consent models and Android `AdMobManager` extensions
- MAUICross project reference, DI registration, Android consent service,
  logging adapter, banner handler, and full-screen services
- Android test app and MAUI test app Android flow
- `.agents/context.md` and this file

No file under `AMDevIT.Admob.Wrapper.iOSNative`, no Apple test app source, and
no native iOS source or XCFramework was changed in this step.

## Checks performed

- Binding restore/build: succeeded; zero errors. Only `BG8605`/`BG8606` for
  confirmed Kotlin synthetic members remain.
- Shared Android wrapper build: succeeded.
- MAUICross Android build: succeeded with zero warnings/errors.
- Android test-app APK build: succeeded with zero warnings/errors.
- MAUI Android test-app APK build: succeeded with zero warnings/errors.
- Required aggregate restore and build: succeeded across Android, iOS, Mac
  Catalyst, and Windows library targets; zero errors and the two documented
  Kotlin binding warnings.
- MAUICross tests: all 7 passed. Windows PRI emitted the pre-existing/external
  `PRI263` neutral-resource warning for `MSTestAdapter.PlatformServices`.
- Release packs for the Android binding, shared wrapper, and MAUICross were
  created as `0.1.10` packages.
- Inspected the Android binding NuGet: it embeds
  `admob-wrapper-release.aar`, `ads-mobile-sdk-1.3.1.aar`, and
  `hsdp-2.0.1.aar`; its dependency group includes UMP `4.0.0.3`; it does not
  contain or depend on legacy `Xamarin.GooglePlayServices.Ads`.
- `git diff --check`: passed apart from expected Windows line-ending notices.

## Open issues and recommended next step

- The user must regenerate the updated iOS XCFramework on macOS. After it is
  supplied, update the .NET iOS binding and add the iOS implementation of the
  shared consent/MAUI APIs.
- Run physical Android tests for first-run consent, returning-user consent,
  privacy-options form, under-age configuration, consent-error fallback, and
  every ad format before publishing `0.1.10`.
- The official Privacy Sandbox AdsServices .NET binding currently has only the
  prerelease NuGet version `1.1.0.6-beta12`, although it belongs to Google's
  stable Next-Gen dependency graph. `NU5104` is suppressed locally with an
  explanatory project comment; revisit when Microsoft publishes a stable
  binding.
- `BG8605`/`BG8606` can be re-evaluated after a future .NET Android binding
  generator update; they currently describe intentionally unbound Kotlin
  compiler artifacts, not missing public APIs.
