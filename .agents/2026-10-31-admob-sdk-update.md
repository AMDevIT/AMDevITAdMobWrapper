# AdMob SDK dependency update — 2026-10-31

## Objective and status

Align native Android and .NET with the latest common Maven/NuGet AdMob SDK,
and verify the native iOS Swift Package integration. Android and .NET changes
are implemented; native iOS binary regeneration requires macOS and Xcode.

## Decisions made

- Google Maven publishes Next-Gen 1.6.0, while the official
  Xamarin.Google.Android.Libraries.Ads.Mobile.Sdk NuGet index ends at 1.5.0.
  Selected 1.5.0 for both native Android and .NET, as requested by the user.
- Replaced the manual AndroidMavenLibrary/dependency list with the official
  1.5.0 NuGet. The custom Kotlin wrapper remains bound from its checked-in AAR.
  NuGet now manages HSDP 2.1.0, UMP binding 4.0.0.4 (native 4.0.0), and the
  remaining SDK dependencies. Removed the obsolete Privacy Sandbox beta binding
  and NU5104 suppression along with the manual graph.
- Updated MAUICross Fragment.Ktx to 1.9.1 and pinned AppCompat 1.8.0 to resolve
  the version conflicts introduced by the official SDK dependency graph.
- Rebuilt and replaced the Android wrapper AAR. SHA-256 matches the Gradle
  output: 8484A35BD15E2D9C82B0D5A201F360774CBD6385643C053EE075CEE2761507C0.
  TFAT enum remains present.
- Confirmed the latest official iOS SDK/tag is 13.11.0; no 14.x tag exists.
  Raised the Swift Package minimum from 13.7.0 to 13.11.0 while retaining
  upToNextMajorVersion. This does not update the precompiled .NET XCFramework.
- Updated README and replaced the package verifier's obsolete legacy Ads
  dependency expectation with the official Next-Gen package 1.5.0.
- Preserved package version 0.1.40 and the user's AGENTS.md edits. No commit,
  push, publication, or release package generation was performed.

## Affected files

- README.md
- eng/verify-packages.ps1
- sources/droid/AdMobWapperApp/gradle/libs.versions.toml
- sources/apple/ios/AdMobWrapper.xcodeproj/project.pbxproj
- .NET Droid project and Jars/admob-wrapper-release.aar
- .NET MAUICross project
- This note and .agents/context.md

## Checks performed and results

- SSH fetch failed because GitHub rejected the configured key. HTTPS fetch
  succeeded; local HEAD and remote main have zero commits of divergence.
- Gradle test and assembleRelease succeeded: 7 tests, zero failures.
  Existing Kotlin exhaustive-when redundant-else warnings remain.
- Gradle dependencyInsight confirmed ads-mobile-sdk 1.5.0.
- Aggregate dotnet restore succeeded after resolving Fragment/AppCompat conflicts.
- Aggregate dotnet build succeeded for all solution library targets after AAR
  replacement: zero errors, zero warnings in the final incremental build.
  An earlier binding regeneration emitted the documented BG8605/BG8606 warnings
  for Kotlin synthetic dollar-sign members; resolution report was inspected.
- Android Studio now supplies JDK 25, which .NET Android rejects with XA0030;
  explicitly selecting C:/Program Files/Java/jdk-21 resolved the build failure.
- All 10 .NET tests passed through dotnet vstest after building the test project.
  dotnet test cannot run this existing VSTest project because global.json
  selects Microsoft.Testing.Platform. Existing MSTest analyzer and Windows PRI
  resource warnings remain in the test build.
- Restored assets confirm SDK 1.5.0, HSDP 2.1.0, UMP 4.0.0.4, and absence of
  legacy Xamarin.GooglePlayServices.Ads and Privacy Sandbox bindings.
- MAUI Android test app restored and built its APK successfully: zero warnings
  and zero errors. Resolved app assets confirm SDK 1.5.0 and UMP 4.0.0.4.
- Modified project XML and package verifier PowerShell syntax parsed successfully.
- Scoped git diff --check passed.

## Open issues and recommended next step

- On macOS resolve/update the Swift Package, run sources/apple/ios/build_xcframework.sh,
  and replace the .NET iOSNative/libs/AdMobWrapper.xcframework binary. Native
  Swift compilation and XCFramework regeneration are unavailable on Windows.
- Physically validate consent, banners, full-screen ads, and lifecycle behavior.
- The full release package verifier was not executed because this task does
  not generate or publish a release; its dependency expectation was updated.
