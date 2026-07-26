# MAUI desktop fallback support

## Objective and status

Add Windows and Mac Catalyst targets to the MAUI wrapper while preserving the
native AdMob banner behavior on Android and iOS. On unsupported desktop
platforms, render a XAML-configurable fallback view. Completed.

## Decisions made

- Kept `BannerAd` derived from `View`, so Android and iOS handlers continue to
  own and render the native AdMob binding views.
- Added a shared `FallbackTemplate` bindable property of type `DataTemplate`.
- The default value is a template that creates an empty MAUI `ContentView`.
- Windows and Mac Catalyst handlers create the template content lazily, keep it
  inside a stable native container, propagate the banner binding context, and
  recreate the content when the template changes.
- Android and iOS have no implementation for the fallback partial method, so
  they never instantiate the fallback template.
- Added unsupported desktop implementations for context resolution and
  full-screen services because the shared dependency-injection registration
  requires concrete implementations on every target. Full-screen calls fail
  explicitly with `PlatformNotSupportedException`.

## Affected files

- `README.md`
- `sources/dotnet/AMDevIT.Admob.Wrapper/AMDevIT.Admob.Wrapper.MAUICross/AMDevIT.Admob.Wrapper.MAUICross.csproj`
- `sources/dotnet/AMDevIT.Admob.Wrapper/AMDevIT.Admob.Wrapper.MAUICross/BannerAd.cs`
- `sources/dotnet/AMDevIT.Admob.Wrapper/AMDevIT.Admob.Wrapper.MAUICross/BannerAdHandler.cs`
- `sources/dotnet/AMDevIT.Admob.Wrapper/AMDevIT.Admob.Wrapper.MAUICross/Platforms/MacCatalyst/BannerAdHandler.cs`
- `sources/dotnet/AMDevIT.Admob.Wrapper/AMDevIT.Admob.Wrapper.MAUICross/Platforms/Windows/BannerAdHandler.cs`
- `sources/dotnet/AMDevIT.Admob.Wrapper/AMDevIT.Admob.Wrapper.MAUICross/Services/ContextResolverService.Unsupported.cs`
- `sources/dotnet/AMDevIT.Admob.Wrapper/AMDevIT.Admob.Wrapper.MAUICross/Services/FullScreenAdServices.Unsupported.cs`
- `sources/dotnet/AMDevIT.Admob.Wrapper/AMDevIT.Admob.Wrapper.MAUITestApp/AMDevIT.Admob.Wrapper.MAUITestApp.csproj`
- `sources/dotnet/AMDevIT.Admob.Wrapper/AMDevIT.Admob.Wrapper.MAUITestApp/MainPage.xaml`

## Checks performed

- `dotnet restore sources\dotnet\AMDevIT.Admob.Wrapper`: succeeded with the
  existing `NU1608` AndroidX dependency-version warnings.
- MAUI wrapper builds for Windows, Mac Catalyst, and iOS: succeeded with zero
  errors.
- MAUI test app builds for Windows, Mac Catalyst, and iOS: succeeded with zero
  errors, including XAML source generation for `FallbackTemplate`.
- Aggregate `dotnet build sources\dotnet\AMDevIT.Admob.Wrapper`: desktop and iOS
  targets succeeded, but the command failed for Android projects with `XA5300`
  because no Java SDK is installed in the current environment.
- `git -c core.longpaths=true diff --check`: succeeded; only line-ending
  conversion warnings were reported.

## Open issues and recommended next step

- Re-run the Android wrapper and test-app builds in an environment with a Java
  SDK configured through `JavaSdkDirectory` or the standard .NET Android setup.
- Review the public `FallbackTemplate` API and desktop unsupported-service
  behavior before committing.
