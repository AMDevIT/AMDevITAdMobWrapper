# Android JNI logging and ad callback lifecycle hardening

## Objective and status

Fix the MAUICross Android crash caused by a late banner refresh callback
reaching a disposed `DroidLoggerAdapter`, and apply equivalent lifecycle
protection to banner, interstitial, rewarded, and app-open wrappers.

Status: completed. Native Android source, AAR, .NET binding, shared Android
helpers, and MAUICross are updated. Kotlin/.NET tests, aggregate builds, Release
packaging, and a trimmed/R8 MAUI Android build passed. Physical-device stress
testing remains recommended before publication.

## Exact crash cause and race

- `BannerAdWrapper.destroy()` previously destroyed only the `AdView`. The
  loaded `BannerAd` retained its event and refresh callbacks, and those
  callbacks captured the constructor logger and managed listeners directly.
- `BannerAdHandler.DisconnectHandler` called native destroy and then disposed
  the wrapper and `DroidLoggerAdapter`. A Google refresh coroutine already
  completing could therefore call `onAdFailedToRefresh` after the managed JNI
  peer had been disposed.
- The generated Java `DroidLoggerAdapter()` proxy activates its managed type
  through `TypeManager.Activate` with an empty signature. The old managed class
  exposed only `DroidLoggerAdapter(ILogger)`, so reactivation failed with
  `NotSupportedException` before logging could occur.
- Interstitial, rewarded, and app-open wrappers had the same late load/event
  callback exposure and no explicit native teardown API.

## Decisions made

- Kept every existing public method and signature. Added compatible `destroy()`
  methods to the three full-screen Kotlin wrappers.
- Added a synchronized generation gate shared by all four native wrappers.
  Loading increments the generation; reload invalidates the previous one;
  destroy marks the wrapper terminal and invalidates every pending callback.
- Callback validity and the complete managed logger/listener invocation occur
  under the same monitor. Destroy therefore waits for a callback already in
  progress and prevents callbacks that have not entered from reaching JNI.
- Banner teardown replaces event/refresh callbacks with no-op callbacks,
  destroys the `AdView`, and clears the ad, view, logger, and listener fields.
  Full-screen teardown similarly replaces native event callbacks and clears
  every managed reference.
- Google owns the internal load/refresh coroutines and exposes no wrapper-owned
  `Job` to cancel. `AdView.destroy()` performs the available native cancellation;
  generation invalidation is the synchronous barrier for callbacks that are
  already queued or racing with teardown.
- All calls into managed logger/listener proxies are guarded against
  `Throwable`, so diagnostic-provider failures cannot escape a Java callback
  thread.
- Added both a safe empty constructor and the JNI handle constructor to
  `DroidLoggerAdapter`. Both use `NullLogger`; normal construction still uses
  the injected `ILogger`. All logger methods also suppress provider failures.
- MAUI banner listeners are retained as handler fields, deactivated before
  disposal, protected against already-queued main-thread work, and expose empty
  plus JNI-handle constructors.
- Async Android proxy listeners with constructor state are retained strongly
  until their native terminal callback and provide safe empty/JNI constructors.
- Full-screen services now call native `Destroy()` before disposing wrappers,
  listeners, and loggers.
- Added consumer R8 rules preserving wrapper JNI class names and callback
  interfaces. The internal lifecycle helper is excluded from the .NET binding.
- Advanced the coordinated NuGet prerelease from `0.1.40-alpha1` to
  `0.1.40-alpha2` and updated package examples/release notes.

## Affected files

- Native Android ad wrappers: `BannerAdWrapper.kt`, `InterstitialAdWrapper.kt`,
  `RewardedAdWrapper.kt`, and `AppOpenAdWrapper.kt`.
- New native lifecycle gate and tests: `CallbackLifecycle.kt` and
  `CallbackLifecycleTest.kt`.
- Android consumer rules and regenerated binding AAR.
- Android binding `Transforms/Metadata.xml`.
- Shared Android callback retention helper and async extension listeners.
- MAUICross Android `DroidLoggerAdapter`, `BannerAdHandler`, and all three
  full-screen service implementations.
- Central package metadata, MAUICross release notes, README package examples,
  this file, and `.agents/context.md`.

## Checks performed and results

- Required SSH fetch was attempted first but GitHub rejected the configured
  key. HTTPS `ls-remote` confirmed the local starting commit matched the remote
  `Task-TeenSupport` head; the working branch itself had no remote counterpart,
  so no pull was required.
- Gradle `:admob-wrapper:test`: passed. Six new lifecycle tests and the existing
  example test passed with zero failures.
- Gradle `:admob-wrapper:assembleRelease`: passed; the regenerated AAR was
  copied into the .NET binding and SHA-256 equality was verified.
- AAR inspection confirmed TFAT Teen remains present and all four wrappers
  expose `destroy()`.
- Required `dotnet restore .`: passed.
- Android binding, shared Android wrapper, and MAUICross Android targeted builds
  passed. Only the two documented binding warnings for Kotlin synthetic `$`
  members appeared in isolated binding builds.
- MAUICross tests: 10 passed, zero failed. Pre-existing MSTest analyzer and
  Windows PRI resource warnings remain external to this change.
- Aggregate `dotnet build . --no-restore`: passed with zero warnings and zero
  errors across Android, iOS, Mac Catalyst, Windows, and neutral targets.
- MAUI Android Release build with trimming/R8: passed. Its six `XA4301` warnings
  are the existing duplicate `libdatastore_shared_counter.so` sample-app
  warnings.
- Generated Java inspection confirmed the logger proxy still activates with an
  empty signature and the managed class now supplies a safe empty constructor.
- DEX inspection of the signed Release APK confirmed R8 preserved
  `it.amdev.admob.wrapper.ads.BannerAdWrapper` and
  `crc64a4fb93f70a458fb3.DroidLoggerAdapter`.
- MAUICross `0.1.40-alpha2` `.nupkg` and `.snupkg` packing passed.
- `git diff --check`: passed; only expected Windows line-ending notices were
  reported.

## Open issues and recommended next step

1. Install `AMDevIT.Admob.Wrapper.MAUICross 0.1.40-alpha2` in the affected MAUI
   application and reproduce repeated banner refresh/disconnect/reconnect flows
   with aggressive GC on a physical Android device.
2. Stress concurrent page removal, adaptive reloads, rotation, no-fill refresh
   errors, and double disconnect while observing that no managed callback is
   emitted after handler teardown.
3. Repeat late-load/dispose tests for interstitial, rewarded, and app-open on a
   device; their lifecycle is hardened and compile-tested but Google SDK
   callbacks cannot be synthesized faithfully by JVM unit tests.
4. Publish `0.1.40-alpha2` only after the physical-device gate passes. Keep the
   existing duplicate native-library sample warning investigation separate.
