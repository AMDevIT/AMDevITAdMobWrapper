# Cross-platform TFAT age treatment support — 2026-08-18

## Objective and status

Add Google Tag for age treatment (TFAT) support for child, teen, and
unspecified users across the native Android and iOS wrappers, the shared .NET
API, MAUICross, samples, README, and GitHub wiki.

Status: source implementation and static documentation checks are complete.
The Android AAR, iOS XCFramework, restore, builds, and automated tests remain
pending because repository instructions require separate authorization for
build and verification commands.

## Decisions made

- Added the platform-neutral `AdMobAgeTreatment` values `Unspecified = 0`,
  `Child = 1`, and `Teen = 2`, matching Google's TFAT wire values.
- Kept TFAT independent from UMP's `TagForUnderAgeOfConsent`. UMP configures
  consent collection; TFAT configures global Mobile Ads request treatment.
- Preserved every existing initialization signature and added overloads that
  accept `AdMobAgeTreatment`.
- Android supplies a `RequestConfiguration` through Next-Gen
  `InitializationConfig.Builder.setRequestConfiguration`; calls made after
  initialization update the global request configuration for future ads.
- iOS sets `MobileAds.shared.requestConfiguration.ageRestrictedTreatment` on
  the main thread before starting the SDK.
- Updated samples to exercise `Teen` and documented that initialization is
  performed once and must complete before any banner or full-screen load, not
  once per individual ad.
- Re-cloned the separately hosted GitHub wiki, updated it locally, and did not
  commit or push it.
- Left package and native framework versions unchanged because their current
  metadata is already inconsistent and version alignment was not authorized as
  part of this targeted feature.

## Affected files

- Native Android: `AdMobManager.kt`, new privacy `AdMobAgeTreatment.kt`, and
  Android binding `Transforms/Metadata.xml`.
- Native iOS: `AdMobManager.swift` and new privacy
  `AdMobAgeTreatment.swift`.
- .NET binding/shared API: iOS `ApiDefinition.cs` and `StructsAndEnums.cs`, new
  shared `AdMobAgeTreatment.cs`, and Android/iOS async manager extensions.
- MAUICross: `IAdMobConsentService`, Android/iOS/unsupported implementations,
  unsupported-service tests, MAUI sample, Android sample, and iOS sample.
- Documentation: root `README.md` and six files in the separately cloned
  `AMDevITAdMobWrapper.wiki` repository.
- Progressive context: this file and `.agents/context.md`.

## Checks performed and results

- Confirmed the current Google Android Next-Gen and iOS SDK versions expose
  child, teen, and unspecified age treatment and require configuration before
  initialization/ad loading.
- Reviewed all native-to-managed mappings and preserved legacy overloads.
- `git diff --check` passed for the main repository and wiki; only expected
  Windows LF-to-CRLF notices were emitted.
- Confirmed README/wiki examples distinguish TFAT from UMP and place the first
  argument on the same line in modified multiline invocations.
- No restore, Gradle build, .NET build, test, AAR replacement, or XCFramework
  generation was performed without the required authorization.

## Open issues and recommended next step

1. After authorization, build the Android release AAR and replace the checked-
   in binding AAR, then restore/build the .NET solution and run tests.
2. On macOS, build the Swift framework and regenerate the XCFramework before
   shipping the updated iOS binding; the current binary does not contain the
   new selector or enum.
3. Inspect the generated Android binding API to confirm the new Kotlin enum and
   overload names, then run the Android and MAUI test apps with `Teen`.
4. Physically confirm that every ad request is made only after initialization
   and that Ad Inspector/network diagnostics show the expected TFAT value.
5. Review, commit, and push the separately cloned wiki repository independently
   from the main repository.
