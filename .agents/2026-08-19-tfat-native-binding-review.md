# TFAT native binding review

## Objective and status

Review the regenerated iOS XCFramework and Objective Sharpie output against the
curated .NET iOS binding, then verify that TFAT Teen is consumable through every
native, shared, and MAUICross layer.

The iOS review is complete and passed. The Android source and all managed layers
are wired for TFAT Teen, but the AAR embedded in the .NET Android binding is
stale and must still be rebuilt and replaced before Android support is complete.

## Decisions made

- Kept the curated iOS binding instead of replacing it wholesale with Sharpie
  output. All functional Objective-C selectors match.
- Retained the existing omission of the generated `description` properties on
  `ConsentInformationRequestDebugParameters` and `ConsentStatusData`; these are
  NSObject descriptions and are not wrapper functionality.
- Accepted the protocol parameter spelling difference between raw Sharpie
  output and the curated binding: .NET binding protocols are consumed through
  their generated `I...` interfaces.
- Treated source-level Android support as incomplete until the checked-in AAR
  exposes the new native enum and initialization overload.

## Affected files

- This review note and `.agents/context.md` only.
- No product source or generated binding file required correction during the
  iOS comparison.

## Checks performed and results

- Compared exported selectors for all wrapper types in Sharpie `ApiDefinition.cs`
  against the curated iOS binding. Only the two intentionally omitted
  `description` selectors differ.
- Confirmed `AdMobAgeTreatment` values `Unspecified = 0`, `Child = 1`, and
  `Teen = 2` in Sharpie output and the curated .NET enum.
- Confirmed the TFAT initialization selector in Sharpie output and the curated
  binding.
- Confirmed the enum and method in both XCFramework Objective-C headers and in
  the arm64 device, arm64 simulator, and x86_64 simulator Swift interfaces.
- Confirmed shared .NET extensions, MAUICross Android/iOS/unsupported services,
  tests, and mobile samples reference the shared TFAT enum.
- Inspected the nested `classes.jar` in the checked-in Android AAR. It contains
  `AdMobManager` but does not contain `AdMobAgeTreatment`, proving the binary is
  older than the Kotlin TFAT source.
- No build, restore, or test was run because repository instructions require
  separate user authorization.

## Open issues and recommended next step

After authorization, run the Android release build, replace the AAR in the .NET
binding, verify its class surface, then restore/build the .NET solution and run
the relevant tests. Physical Android and iOS validation should follow before
release.
