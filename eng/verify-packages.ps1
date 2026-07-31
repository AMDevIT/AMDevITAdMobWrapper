param(
    [Parameter(Mandatory = $true)]
    [string]$PackageDirectory,

    [Parameter(Mandatory = $true)]
    [string]$Version,

    [string]$JavaSdkDirectory,

    [string]$AndroidSdkDirectory
)

$ErrorActionPreference = 'Stop'

$packageDirectoryPath = [System.IO.Path]::GetFullPath($PackageDirectory)
$expectedPackageIds = @(
    'AMDevIT.Admob.Wrapper.Droid',
    'AMDevIT.Admob.Wrapper.iOSNative',
    'AMDevIT.Admob.Wrapper',
    'AMDevIT.Admob.Wrapper.MAUICross'
)
$expectedDependencies = @{
    'AMDevIT.Admob.Wrapper.Droid' = @(
        @{ Id = 'Xamarin.GooglePlayServices.Ads'; Version = '125.2.0' }
    )
    'AMDevIT.Admob.Wrapper.iOSNative' = @()
    'AMDevIT.Admob.Wrapper' = @(
        @{ Id = 'AMDevIT.Admob.Wrapper.Droid'; Version = $Version }
        @{ Id = 'AMDevIT.Admob.Wrapper.iOSNative'; Version = $Version }
    )
    'AMDevIT.Admob.Wrapper.MAUICross' = @(
        @{ Id = 'AMDevIT.Admob.Wrapper.Droid'; Version = $Version }
        @{ Id = 'AMDevIT.Admob.Wrapper.iOSNative'; Version = $Version }
        @{ Id = 'Microsoft.Maui.Controls'; Version = '10.0.90' }
    )
}

foreach ($packageId in $expectedPackageIds) {
    $packagePath = Join-Path $packageDirectoryPath "$packageId.$Version.nupkg"
    $symbolPackagePath = Join-Path $packageDirectoryPath "$packageId.$Version.snupkg"

    if (-not (Test-Path -LiteralPath $packagePath -PathType Leaf)) {
        throw "Expected package was not found: $packagePath"
    }

    if (-not (Test-Path -LiteralPath $symbolPackagePath -PathType Leaf)) {
        throw "Expected symbol package was not found: $symbolPackagePath"
    }

    $archive = [System.IO.Compression.ZipFile]::OpenRead($packagePath)

    try {
        $entryNames = $archive.Entries.FullName

        foreach ($requiredEntry in @('README.md', 'admob_maui_icon_v4_wallet_block.png')) {
            if ($requiredEntry -notin $entryNames) {
                throw "$packageId does not contain $requiredEntry."
            }
        }

        $nuspecEntry = $archive.Entries |
            Where-Object FullName -Like '*.nuspec' |
            Select-Object -First 1

        if ($null -eq $nuspecEntry) {
            throw "$packageId does not contain a nuspec file."
        }

        $reader = [System.IO.StreamReader]::new($nuspecEntry.Open())

        try {
            [xml]$nuspec = $reader.ReadToEnd()
        }
        finally {
            $reader.Dispose()
        }

        $metadata = $nuspec.package.metadata

        if ($metadata.id -ne $packageId -or $metadata.version -ne $Version) {
            throw "Unexpected identity in $packagePath."
        }

        if ($metadata.license.InnerText -ne 'Apache-2.0' -or
            $metadata.license.type -ne 'expression' -or
            [string]$metadata.readme -ne 'README.md') {
            throw "Incomplete license or readme metadata in $packagePath."
        }

        if ($metadata.repository.url -ne 'https://github.com/AMDevIT/AMDevITAdMobWrapper' -or
            [string]::IsNullOrWhiteSpace($metadata.repository.commit)) {
            throw "Missing repository metadata in $packagePath."
        }

        $dependencies = @($metadata.dependencies.group.dependency)

        foreach ($expectedDependency in $expectedDependencies[$packageId]) {
            $matchingDependency = $dependencies | Where-Object {
                $_.id -eq $expectedDependency.Id -and
                $_.version -eq $expectedDependency.Version
            }

            if ($null -eq $matchingDependency) {
                throw "$packageId does not contain dependency $($expectedDependency.Id) $($expectedDependency.Version)."
            }
        }
    }
    finally {
        $archive.Dispose()
    }
}

$consumerRoot = Join-Path ([System.IO.Path]::GetTempPath()) "AMDevITAdMobPackageSmoke-$([guid]::NewGuid())"
$resolvedConsumerRoot = [System.IO.Path]::GetFullPath($consumerRoot)

try {
    New-Item -ItemType Directory -Path $resolvedConsumerRoot | Out-Null

    $escapedPackageDirectory = [System.Security.SecurityElement]::Escape($packageDirectoryPath)
    $projectContent = @"
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <TargetFrameworks>net10.0-android;net10.0-windows10.0.26100.0</TargetFrameworks>
    <UseMaui>true</UseMaui>
    <ImplicitUsings>enable</ImplicitUsings>
    <Nullable>enable</Nullable>
    <WindowsPackageType Condition="`$([MSBuild]::GetTargetPlatformIdentifier('`$(TargetFramework)')) == 'windows'">None</WindowsPackageType>
    <SupportedOSPlatformVersion Condition="`$([MSBuild]::GetTargetPlatformIdentifier('`$(TargetFramework)')) == 'android'">33.0</SupportedOSPlatformVersion>
    <SupportedOSPlatformVersion Condition="`$([MSBuild]::GetTargetPlatformIdentifier('`$(TargetFramework)')) == 'windows'">10.0.17763.0</SupportedOSPlatformVersion>
    <TargetPlatformMinVersion Condition="`$([MSBuild]::GetTargetPlatformIdentifier('`$(TargetFramework)')) == 'windows'">10.0.17763.0</TargetPlatformMinVersion>
    <RestoreSources>$escapedPackageDirectory;https://api.nuget.org/v3/index.json</RestoreSources>
  </PropertyGroup>
  <ItemGroup>
    <PackageReference Include="AMDevIT.Admob.Wrapper.MAUICross" Version="$Version" />
    <PackageReference Include="Microsoft.Maui.Controls" Version="10.0.90" />
  </ItemGroup>
</Project>
"@

    $sourceContent = @'
using AMDevIT.Admob.Wrapper.MAUICross;
using Microsoft.Maui.Hosting;

namespace PackageConsumerSmoke;

public static class RegistrationSmokeTest
{
    public static MauiAppBuilder Register(MauiAppBuilder builder)
    {
        return builder.UseAMDevITAdMobWrapper();
    }

    public static void ReferenceServices(IInterstitialAdService interstitial,
                                         IAppOpenAdService appOpen,
                                         IShowableRewardedAdService rewarded)
    {
        _ = interstitial.IsLoaded;
        _ = appOpen.IsShowing;
        rewarded.AdRewardEarned += static (_, _) => { };
        rewarded.Show();
    }
}
'@

    Set-Content -LiteralPath (Join-Path $resolvedConsumerRoot 'PackageConsumerSmoke.csproj') -Value $projectContent
    Set-Content -LiteralPath (Join-Path $resolvedConsumerRoot 'RegistrationSmokeTest.cs') -Value $sourceContent

    dotnet restore (Join-Path $resolvedConsumerRoot 'PackageConsumerSmoke.csproj')

    if ($LASTEXITCODE -ne 0) {
        throw 'Consumer package restore failed.'
    }

    $buildArguments = @(
        'build',
        (Join-Path $resolvedConsumerRoot 'PackageConsumerSmoke.csproj'),
        '--configuration',
        'Release',
        '--no-restore',
        '--disable-build-servers'
    )

    if (-not [string]::IsNullOrWhiteSpace($JavaSdkDirectory)) {
        $buildArguments += "-p:JavaSdkDirectory=$([System.IO.Path]::GetFullPath($JavaSdkDirectory))"
    }

    if (-not [string]::IsNullOrWhiteSpace($AndroidSdkDirectory)) {
        $buildArguments += "-p:AndroidSdkDirectory=$([System.IO.Path]::GetFullPath($AndroidSdkDirectory))"
    }

    & dotnet $buildArguments

    if ($LASTEXITCODE -ne 0) {
        throw 'Consumer package build failed.'
    }
}
finally {
    $tempRoot = [System.IO.Path]::GetFullPath([System.IO.Path]::GetTempPath())

    if ($resolvedConsumerRoot.StartsWith($tempRoot, [System.StringComparison]::OrdinalIgnoreCase) -and
        (Test-Path -LiteralPath $resolvedConsumerRoot)) {
        Remove-Item -LiteralPath $resolvedConsumerRoot -Recurse -Force
    }
}

Write-Host "Verified $($expectedPackageIds.Count) packages and the MAUI consumer smoke project."
