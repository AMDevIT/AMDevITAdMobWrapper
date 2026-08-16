# iOS banner lifecycle crash hardening — 2026-08-09

## Objective and status

Diagnose two TestFlight crashes from MyCurrencyConverter 1.0.4 build 2 and
harden the reusable iOS banner lifecycle in MAUICross without modifying the
consumer application.

Status: source implementation completed. Static checks passed. Restore, build,
automated tests, XCFramework regeneration, and physical-device verification
remain pending.

## Crash diagnosis

- Every supplied dSYM matches its crash-log binary UUID.
- The first crash reaches `UIView.dealloc` through
  `-[PlatformGraphicsView release]` and `+[__NSObject_Disposer drain:]`. It is a
  MAUI-managed native-view disposal crash, but the stack does not directly
  identify the banner view.
- The second crash reaches `_mono_invoke_unhandled_exception_hook` before
  aborting. `AdMobWrapper + 830496` symbolicates as
  `_GADRegisterSignalHandlers + 756`, which is Google Mobile Ads crash-reporting
  infrastructure rather than `BannerAdWrapper` application logic.
- The logs do not prove that both crashes share one cause. They do confirm that
  view disposal and an unhandled managed exception occur while the consumer's
  dynamic banner and page-template lifecycle is active.
- Independently of attribution, the 0.1.10 handler had observable lifecycle
  defects: duplicate load entry points, unmanaged active constraints, disposal
  of the managed view before native ownership was cleared, no stale-callback
  generation check, and no idempotent disconnect guard.

## Decisions made

- Keep the public banner API unchanged.
- Route native `load` and `destroy` work synchronously to the main queue when a
  direct native consumer calls from another thread.
- Detach the Google banner delegate and root view controller before removing
  and releasing the native banner.
- Ignore native callbacks unless their banner is still the current instance.
- Serialize and coalesce MAUI initialization requests on the main thread.
- Deduplicate loads by ad unit ID, banner size, and effective adaptive width.
- Store, deactivate, and dispose every installed Auto Layout constraint before
  destroying the native banner and disposing the managed `UIView` wrapper.
- Give each managed listener pair a generation and weak handler reference so
  queued callbacks after reload or disconnect are ignored.
- Make `DisconnectHandler` idempotent and preserve teardown order: invalidate
  callbacks, deactivate constraints, destroy native ownership, dispose the
  native view wrapper, then dispose listeners and wrapper objects.
- Advance all coordinated package and native framework versions to `0.1.11`.

## Affected files

- `README.md`
- `sources/apple/ios/AdMobWrapper.xcodeproj/project.pbxproj`
- `sources/apple/ios/AdMobWrapper/Ads/BannerAdWrapper.swift`
- `sources/dotnet/AMDevIT.Admob.Wrapper/Directory.Build.props`
- `sources/dotnet/AMDevIT.Admob.Wrapper/AMDevIT.Admob.Wrapper.MAUICross/Platforms/iOS/BannerAdHandler.cs`
- `.agents/context.md`
- This file

MyCurrencyConverter was inspected read-only and was not modified.

## Checks performed and results

- Confirmed matching UUIDs for MyCurrencyConverter, AdMobWrapper, SkiaSharp,
  and HarfBuzzSharp using the supplied dSYMs.
- Symbolicated the relevant native frames from both crash logs.
- Verified balanced Swift and C# braces.
- Verified removal of the old `lastAdaptiveWidth`-only reload guard and presence
  of explicit constraint deactivation, callback generations, stale native
  callback filtering, and main-thread routing.
- Verified coordinated `0.1.11` values in NuGet metadata, README examples, and
  both Xcode configurations.
- Scoped whitespace validation passed; repository-wide validation remains
  affected by the user's unrelated `AGENTS.md` whitespace and known Windows
  long-path notices for the checked-in XCFramework.
- Restore, build, and tests were not run because the repository instructions
  require separate user confirmation before those checks.

## Open issues and recommended next step

1. After authorization, restore and build the .NET solution and run the
   MAUICross tests.
2. On macOS with the supported Xcode version, compile the Swift framework and
   regenerate the checked-in XCFramework and dSYM. The current binary still
   contains the 0.1.10 Swift implementation.
3. Rebuild and inspect the 0.1.11 NuGet packages only after replacing the
   XCFramework.
4. Exercise repeated adaptive layouts, rotations, ad unit and size changes,
   consent/entitlement changes, page unload/reload, refresh operations, and
   delayed callbacks on iOS 16 and iOS 26 physical devices.
5. Preserve the MyCurrencyConverter unhandled-exception logs from a diagnostic
   build because interpreted managed frames cannot identify the original
   exception method from the Apple crash report alone.
