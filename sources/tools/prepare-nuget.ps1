#Requires -Version 7.0

<#
.SYNOPSIS
Build and validate the four complete NuGet packages on Windows. Never publishes.
.DESCRIPTION
Rebuilds Android and runs Kotlin tests, reuses the iOS XCFramework prepared on
macOS, restores workloads/packages, builds Release, runs .NET tests, and packs
four libraries with symbols. Validates metadata and an Android/Windows consumer.
Existing version output directories are refused. Failed staging output is kept.
.PARAMETER OutputDirectory
Output root, relative to the calling directory. Defaults to artifacts/packages.
.PARAMETER JavaSdkDirectory
JDK 21 directory. Defaults to JAVA_HOME, then the standard Java/jdk-21 location.
.PARAMETER AndroidSdkDirectory
Android SDK directory. Defaults to ANDROID_HOME, ANDROID_SDK_ROOT, or standard
Android Studio/Visual Studio locations.
.PARAMETER SkipWorkloadRestore
Reuse already installed .NET workloads. Package restore, builds, tests and all
package checks still run. Missing workloads cause the build to fail normally.
.EXAMPLE
./sources/tools/prepare-nuget.ps1 -JavaSdkDirectory 'C:\Program Files\Java\jdk-21'
#>
[CmdletBinding()]
param(
    [string]$OutputDirectory,
    [string]$JavaSdkDirectory,
    [string]$AndroidSdkDirectory,
    [switch]$SkipWorkloadRestore
)

Set-StrictMode -Version Latest
$ErrorActionPreference = 'Stop'
$PSNativeCommandUseErrorActionPreference = $false

function Invoke-CheckedCommand {
    param([string]$Command, [string[]]$Arguments)

    & $Command @Arguments
    if ($LASTEXITCODE -ne 0) {
        throw "Command failed with exit code ${LASTEXITCODE}: $Command $($Arguments -join ' ')"
    }
}

function Get-PackageVersion {
    param([string]$ProjectPath)

    $result = & dotnet msbuild $ProjectPath -getProperty:Version -nologo
    if ($LASTEXITCODE -ne 0) {
        throw "Cannot read package version: $ProjectPath"
    }
    $version = ($result -join "`n").Trim()
    if ($version -notmatch '^\d+\.\d+\.\d+([.-][0-9A-Za-z.-]+)?$') {
        throw "Invalid package version: $version"
    }
    return $version
}

if (-not $IsWindows) {
    throw 'This script requires Windows. Use prepare-nuget.sh on macOS.'
}

$repoRoot = [System.IO.Path]::GetFullPath((Join-Path $PSScriptRoot '../..'))
$solutionDirectory = Join-Path $repoRoot 'sources/dotnet/AMDevIT.Admob.Wrapper'
$solutionPath = Join-Path $solutionDirectory 'AMDevIT.Admob.Wrapper.slnx'
$androidDirectory = Join-Path $repoRoot 'sources/droid/AdMobWapperApp'
$frameworkPath = Join-Path $solutionDirectory 'AMDevIT.Admob.Wrapper.iOSNative/libs/AdMobWrapper.xcframework'
$projects = @('AMDevIT.Admob.Wrapper.Droid', 'AMDevIT.Admob.Wrapper.iOSNative',
              'AMDevIT.Admob.Wrapper', 'AMDevIT.Admob.Wrapper.MAUICross')
$workloadProject = Join-Path $solutionDirectory 'AMDevIT.Admob.Wrapper.MAUICross/AMDevIT.Admob.Wrapper.MAUICross.csproj'
$testProject = Join-Path $solutionDirectory 'AMDevIT.Admob.Wrapper.MAUICross.Tests/AMDevIT.Admob.Wrapper.MAUICross.Tests.csproj'
$stagingDirectory = $null
$previousJavaHome = $env:JAVA_HOME
$previousAndroidHome = $env:ANDROID_HOME
$previousAndroidSdkRoot = $env:ANDROID_SDK_ROOT
$previousPath = $env:PATH

foreach ($tool in @('dotnet', 'python')) {
    Get-Command $tool -CommandType Application -ErrorAction Stop | Out-Null
}
if ([string]::IsNullOrWhiteSpace($JavaSdkDirectory)) {
    $JavaSdkDirectory = $env:JAVA_HOME
    if ([string]::IsNullOrWhiteSpace($JavaSdkDirectory)) {
        $JavaSdkDirectory = Join-Path $env:ProgramFiles 'Java/jdk-21'
    }
}
$JavaSdkDirectory = $ExecutionContext.SessionState.Path.GetUnresolvedProviderPathFromPSPath($JavaSdkDirectory)
$javaExecutable = Join-Path $JavaSdkDirectory 'bin/java.exe'
if (-not (Test-Path -LiteralPath $javaExecutable -PathType Leaf)) {
    throw "Java executable not found: $javaExecutable. Set -JavaSdkDirectory to JDK 21."
}
$javaVersion = & $javaExecutable -version 2>&1
if ($LASTEXITCODE -ne 0 -or ($javaVersion -join "`n") -notmatch 'version "21\.') {
    throw 'JDK 21 is required; set -JavaSdkDirectory to a JDK 21 installation.'
}
if ([string]::IsNullOrWhiteSpace($AndroidSdkDirectory)) {
    $sdkCandidates = @($env:ANDROID_HOME, $env:ANDROID_SDK_ROOT,
                       (Join-Path $env:LOCALAPPDATA 'Android/Sdk'),
                       (Join-Path ${env:ProgramFiles(x86)} 'Android/android-sdk'))
    $AndroidSdkDirectory = $sdkCandidates | Where-Object {
        -not [string]::IsNullOrWhiteSpace($_) -and
        (Test-Path -LiteralPath (Join-Path $_ 'platforms') -PathType Container) -and
        (Test-Path -LiteralPath (Join-Path $_ 'build-tools') -PathType Container)
    } | Select-Object -First 1
}
if ([string]::IsNullOrWhiteSpace($AndroidSdkDirectory)) {
    throw 'Android SDK not found; set -AndroidSdkDirectory.'
}
$AndroidSdkDirectory = $ExecutionContext.SessionState.Path.GetUnresolvedProviderPathFromPSPath($AndroidSdkDirectory)
foreach ($sdkFolder in @('platforms', 'build-tools')) {
    if (-not (Test-Path -LiteralPath (Join-Path $AndroidSdkDirectory $sdkFolder) -PathType Container)) {
        throw "Android SDK is incomplete: $AndroidSdkDirectory"
    }
}
foreach ($requiredFile in @($solutionPath, $workloadProject, $testProject,
                            (Join-Path $androidDirectory 'gradlew.bat'),
                            (Join-Path $frameworkPath 'Info.plist'))) {
    if (-not (Test-Path -LiteralPath $requiredFile -PathType Leaf)) {
        throw "Required file not found: $requiredFile. Prepare iOS binaries on macOS before Windows packaging."
    }
}
if ([string]::IsNullOrWhiteSpace($OutputDirectory)) {
    $OutputDirectory = Join-Path $repoRoot 'artifacts/packages'
}
$outputRoot = [System.IO.Path]::TrimEndingDirectorySeparator($ExecutionContext.SessionState.Path.GetUnresolvedProviderPathFromPSPath($OutputDirectory))
$buildArguments = @("-p:JavaSdkDirectory=$JavaSdkDirectory",
                    "-p:AndroidSdkDirectory=$AndroidSdkDirectory", '-p:ValidateXcodeVersion=false')

Push-Location $solutionDirectory
try {
    $env:JAVA_HOME = $JavaSdkDirectory
    $env:ANDROID_HOME = $AndroidSdkDirectory
    $env:ANDROID_SDK_ROOT = $AndroidSdkDirectory
    $env:PATH = "$(Join-Path $JavaSdkDirectory 'bin');$previousPath"
    $dotnetVersion = & dotnet --version
    if ($LASTEXITCODE -ne 0 -or $dotnetVersion -notlike '10.*') {
        throw "The selected .NET SDK must be 10.x (found $dotnetVersion)."
    }
    Invoke-CheckedCommand python @('--version')
    $version = Get-PackageVersion (Join-Path $solutionDirectory 'AMDevIT.Admob.Wrapper/AMDevIT.Admob.Wrapper.csproj')
    foreach ($project in $projects) {
        if ((Get-PackageVersion (Join-Path $solutionDirectory "$project/$project.csproj")) -ne $version) {
            throw "Package versions differ: $project"
        }
    }
    $outputPath = Join-Path $outputRoot $version
    if (Test-Path -LiteralPath $outputPath) {
        throw "Output already exists: $outputPath. Choose another -OutputDirectory."
    }

    $frameworkCheck = @'
from pathlib import Path
import plistlib
import sys
framework = Path(sys.argv[1])
with (framework / "Info.plist").open("rb") as source:
    libraries = plistlib.load(source)["AvailableLibraries"]
variants = set()
for library in libraries:
    if library.get("SupportedPlatform") != "ios":
        continue
    binary = framework / library["LibraryIdentifier"] / library["LibraryPath"]
    if not (binary / "AdMobWrapper").is_file() or not (binary / "Headers/AdMobWrapper-Swift.h").is_file():
        raise SystemExit(f"Incomplete XCFramework slice: {binary}")
    variants.add(library.get("SupportedPlatformVariant", "device"))
if not {"device", "simulator"}.issubset(variants):
    raise SystemExit("XCFramework must contain iOS device and simulator slices. Rebuild it on macOS.")
'@
    Invoke-CheckedCommand python @('-c', $frameworkCheck, $frameworkPath)
    New-Item -ItemType Directory -Path $outputRoot -Force | Out-Null
    $stagingDirectory = Join-Path $outputRoot ".prepare-nuget.$([guid]::NewGuid())"
    New-Item -ItemType Directory -Path $stagingDirectory | Out-Null

    Write-Host "`nBuilding Android wrapper and running Kotlin tests..."
    Push-Location $androidDirectory
    try {
        Invoke-CheckedCommand (Join-Path $androidDirectory 'gradlew.bat') @(':admob-wrapper:test', ':admob-wrapper:assembleRelease', '--console=plain')
    }
    finally {
        Pop-Location
    }
    $aarPath = Join-Path $androidDirectory 'admob-wrapper/build/outputs/aar/admob-wrapper-release.aar'
    if (-not (Test-Path -LiteralPath $aarPath -PathType Leaf) -or (Get-Item -LiteralPath $aarPath).Length -eq 0) {
        throw "Android build did not produce $aarPath"
    }
    $bindingJars = Join-Path $solutionDirectory 'AMDevIT.Admob.Wrapper.Droid/Jars'
    New-Item -ItemType Directory -Path $bindingJars -Force | Out-Null
    Copy-Item -LiteralPath $aarPath -Destination (Join-Path $bindingJars 'admob-wrapper-release.aar') -Force

    Write-Host "`nReusing the iOS XCFramework prepared on macOS: $frameworkPath"
    Write-Host "`nRestoring workloads and solution packages..."
    if ($SkipWorkloadRestore) {
        Write-Host 'Skipping workload installation; using the installed workloads.'
    }
    else {
        Invoke-CheckedCommand dotnet @('workload', 'restore', $workloadProject)
    }
    Invoke-CheckedCommand dotnet (@('restore', $solutionPath) + $buildArguments)
    Write-Host "`nBuilding solution in Release..."
    Invoke-CheckedCommand dotnet (@('build', $solutionPath, '--configuration', 'Release', '--no-restore', '--no-incremental', '-m:1') + $buildArguments)

    Write-Host "`nRunning .NET tests..."
    Invoke-CheckedCommand dotnet (@('restore', $testProject) + $buildArguments)
    Invoke-CheckedCommand dotnet (@('build', $testProject, '--configuration', 'Release', '--no-restore') + $buildArguments)
    $testAssembly = Join-Path $solutionDirectory 'AMDevIT.Admob.Wrapper.MAUICross.Tests/bin/Release/net10.0-windows10.0.26100.0/win-x64/AMDevIT.Admob.Wrapper.MAUICross.Tests.dll'
    Invoke-CheckedCommand dotnet @('vstest', $testAssembly)

    Write-Host "`nPacking version $version..."
    foreach ($project in $projects) {
        Invoke-CheckedCommand dotnet (@('pack', (Join-Path $solutionDirectory "$project/$project.csproj"), '--configuration', 'Release', '--no-build', '--no-restore', '--output', $stagingDirectory) + $buildArguments)
    }
    Write-Host "`nChecking packages and consumer..."
    Invoke-CheckedCommand python @((Join-Path $PSScriptRoot 'verify-nuget.py'), $stagingDirectory, $version, $solutionDirectory)
    & (Join-Path $repoRoot 'eng/verify-packages.ps1') -PackageDirectory $stagingDirectory -Version $version -JavaSdkDirectory $JavaSdkDirectory -AndroidSdkDirectory $AndroidSdkDirectory
    if (-not $?) {
        throw 'Package consumer verification failed.'
    }
    if (Test-Path -LiteralPath $outputPath) {
        throw "Output appeared during the build: $outputPath"
    }
    foreach ($movePath in @($stagingDirectory, $outputPath)) {
        if (-not [System.IO.Path]::GetDirectoryName([System.IO.Path]::GetFullPath($movePath)).Equals($outputRoot, [System.StringComparison]::OrdinalIgnoreCase)) {
            throw "Package move path is outside the selected output root: $movePath"
        }
    }
    # Directory.Move refuses an existing destination instead of nesting output.
    [System.IO.Directory]::Move($stagingDirectory, $outputPath)
    $stagingDirectory = $null
    Write-Host "`nPackages prepared at: $outputPath"
    Write-Host 'All four MAUI targets are included. Nothing was published. Physical-device checks remain separate.'
    Write-Host 'The Android binding AAR was replaced. Review the native binary diff before committing.'
}
catch {
    if ($null -ne $stagingDirectory) {
        Write-Warning "Preparation failed. Partial output retained at: $stagingDirectory"
    }
    throw
}
finally {
    $env:JAVA_HOME = $previousJavaHome
    $env:ANDROID_HOME = $previousAndroidHome
    $env:ANDROID_SDK_ROOT = $previousAndroidSdkRoot
    $env:PATH = $previousPath
    Pop-Location
}
