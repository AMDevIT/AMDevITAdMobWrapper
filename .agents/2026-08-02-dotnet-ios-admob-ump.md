# .NET iOS AdMob and UMP binding alignment

## Objective and status

Update the .NET iOS native binding and the shared multi-platform wrapper after
the regenerated XCFramework was supplied. Completed. The binding now exposes
the native diagnostics, consent, banner sizing, and AdMob APIs, and the shared
wrapper provides the same asynchronous UMP workflow already available on
Android.

## Decisions made

- Used the public Objective-C header inside the regenerated XCFramework as the
  binding source of truth.
- Removed generated Swift extension categories for Google Mobile Ads delegate
  protocols. They are native implementation details and would incorrectly
  require direct .NET bindings for Google SDK types.
- Restored the public binding namespace to
  `AMDevIT.Admob.Wrapper.iOSNative` so existing consumers remain compatible.
- Preserved the established managed API names for initialization, ad loading,
  loaded/showing state, and reward callbacks.
- Added logger-aware constructors, `IAppleLogger`, `AppleLogLevel`, UMP DTOs,
  consent listener protocols, manager consent operations, and the size-aware
  banner overload.
- Named the managed logging enum `AppleLogLevel` to avoid ambiguity with
  `Microsoft.Extensions.Logging.LogLevel` in existing iOS consumers. This does
  not change the native selector or enum values.
- Added iOS asynchronous manager extensions for initialization, consent
  information refresh, consent gathering, privacy-options form, required
  consent form, and current status mapping. Cancellation cancels the managed
  wait while the already-started native operation is allowed to finish, matching
  the Android implementation.
- Kept MAUICross source unchanged. Its iOS target was built as an integration
  check and consumes the updated binding successfully.
- Updated the README to remove the obsolete statement that the iOS UMP binding
  was still pending and added an iOS consent-before-initialization example.

## Affected files

- `README.md`
- `sources/dotnet/AMDevIT.Admob.Wrapper/AMDevIT.Admob.Wrapper.iOSNative/ApiDefinition.cs`
- `sources/dotnet/AMDevIT.Admob.Wrapper/AMDevIT.Admob.Wrapper.iOSNative/StructsAndEnums.cs`
- `sources/dotnet/AMDevIT.Admob.Wrapper/AMDevIT.Admob.Wrapper/Extensions/iOSNative/AdMobManagerExtensions.iOSNative.cs`
- `.agents/context.md`
- This file

The user-supplied XCFramework was inspected and packaged but not modified in
this step. No MAUICross, Android, or native Apple source file was changed.

## Checks performed

- iOS native binding build: succeeded with zero warnings/errors.
- Shared wrapper `net10.0-ios` build: succeeded with zero warnings/errors.
- MAUICross `net10.0-ios` integration build: succeeded with zero
  warnings/errors.
- Apple test app build: succeeded; it retains one pre-existing unused-field
  warning.
- Required aggregate restore: succeeded.
- Required aggregate build: succeeded for Android, iOS, Mac Catalyst, and
  Windows after supplying the Android Studio JDK path explicitly; zero
  warnings/errors.
- MAUICross tests: all 7 passed using the solution-local Microsoft Testing
  Platform configuration.
- Release packages created:
  `AMDevIT.Admob.Wrapper.iOSNative.0.1.10.nupkg` and
  `AMDevIT.Admob.Wrapper.0.1.10.nupkg`.
- Package inspection confirmed that the iOS binding resource archive contains
  both device and simulator XCFramework binaries, headers, and metadata, and
  that the shared package contains both Android and iOS assemblies.
- Targeted `git diff --check`: passed; only Windows line-ending notices remain.

## Open issues and recommended next step

- Run UMP first-run, returning-user, error fallback, privacy-options, and reset
  flows on a physical iOS device.
- Exercise logger callbacks, every ad format, banner sizes, adaptive rotation,
  and dismissal timing on a physical iOS device before publishing `0.1.10`.
- The MAUI consent service is still intentionally Android-only. iOS consumers
  can use the shared async extensions on the native `AdMobManager`; a future
  task may add an iOS MAUI service if a single DI abstraction is desired.
