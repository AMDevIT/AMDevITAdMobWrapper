# Changelog

## 0.2.1 - Unreleased

- Restore Windows compatibility through complete MAUICross release packages.
- Block incomplete MAUICross packaging and require all four platform assemblies during package verification.
- Reserve macOS output for native preparation; produce final packages on Windows.

## 0.2.0 - Released

### SDK and dependency updates

- Update Android Google Mobile Ads Next-Gen from 1.3.1 to **1.5.0**, the latest
  version shared by Google Maven and the official .NET binding at preparation time.
- Use `Xamarin.Google.Android.Libraries.Ads.Mobile.Sdk` to manage the Android SDK
  and its transitive dependencies, including UMP binding **4.0.0.4** for native
  UMP **4.0.0**. Remove the former manual Maven dependency list.
- Align MAUI AndroidX Fragment.Ktx and AppCompat with the SDK dependency graph.
- Update iOS Google Mobile Ads from 13.7.0 to **13.11.0** through Google's official
  Swift Package. The resolved transitive iOS UMP package is **3.1.0**.
- Preserve the existing public advertising and consent APIs.

### Release preparation

- Add macOS tooling to rebuild the native AAR/XCFramework, replace binding
  binaries, build Release, and validate all four NuGet packages and their symbols.
- Check Xcode availability without comparing its version.
- Use explicit project/solution paths during workload restoration and .NET builds.
- Coordinate the four NuGet packages and native wrapper source versions at
  **0.2.0**, with centralized NuGet release notes and updated installation guidance.
- Align the Windows CI release checks with JDK 21 and the VSTest test project.

### Validation notes

The native binaries must be rebuilt after the version change before publishing
0.2.0. Package validation and builds do not replace physical Android/iOS consent,
ad-format, and lifecycle smoke tests. Android binding warnings BG8605/BG8606 for
Kotlin compiler-generated dollar-sign members remain documented; new resolution
failures require investigation. Windows targets/tests require a Windows host.
