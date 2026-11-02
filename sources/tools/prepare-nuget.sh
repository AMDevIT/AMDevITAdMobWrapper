#!/bin/bash

# Rebuild native wrappers and prepare NuGet packages on macOS. Never publishes.
set -euo pipefail

SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
REPO_ROOT="$(cd "$SCRIPT_DIR/../.." && pwd)"
SOLUTION_DIR="$REPO_ROOT/sources/dotnet/AMDevIT.Admob.Wrapper"
SOLUTION_PATH="$SOLUTION_DIR/AMDevIT.Admob.Wrapper.slnx"
WORKLOAD_PROJECT="$SOLUTION_DIR/AMDevIT.Admob.Wrapper.MAUICross/AMDevIT.Admob.Wrapper.MAUICross.csproj"
ANDROID_DIR="$REPO_ROOT/sources/droid/AdMobWapperApp"
IOS_DIR="$REPO_ROOT/sources/apple/ios"
OUTPUT_ROOT="$REPO_ROOT/artifacts/native-preparation"
STAGING_DIR=""

fail() {
    printf 'Error: %s\n' "$*" >&2
    exit 1
}

usage() {
    cat <<'EOF'
Usage: bash sources/tools/prepare-nuget.sh [--output DIRECTORY]

Rebuilds the Android AAR and iOS XCFramework, replaces the binaries in the
.NET binding projects, restores/builds Release, runs Kotlin tests, and creates
three base .nupkg files and their .snupkg symbol packages. Validates metadata.

Output: artifacts/native-preparation/<Version>/ (or DIRECTORY/<Version>/).
Version is read from the .NET projects. An existing version directory is refused.
Relative output paths are resolved from the current working directory.

Requires macOS, Xcode with iOS SDKs, .NET 10, JDK 21, Android SDK, Python 3,
and xcpretty. Xcode availability is checked; its version is not checked.
JAVA_HOME and ANDROID_HOME/ANDROID_SDK_ROOT may override the default SDK paths.
.NET workload restore may install missing workloads and require permissions.

MAUICross is built but never packed here. Prepare the complete four-platform
MAUICross package on Windows using the rebuilt native binaries. Existing .NET
tests target Windows and must also run there. Nothing is published or pushed to NuGet or Git.
EOF
}

on_exit() {
    local status=$?
    if [[ $status -ne 0 && -n "$STAGING_DIR" ]]; then
        printf 'Preparation failed. Partial output retained at: %s\n' "$STAGING_DIR" >&2
    fi
}
trap on_exit EXIT

while [[ $# -gt 0 ]]; do
    case "$1" in
        --output)
            [[ $# -ge 2 && -n "$2" ]] || fail '--output requires a directory.'
            OUTPUT_ROOT="$2"
            shift 2
            ;;
        --help|-h)
            usage
            exit 0
            ;;
        *) fail "Unknown argument: $1 (use --help)." ;;
    esac
done

[[ "$(uname -s)" == Darwin ]] || fail 'This script requires macOS.'
for tool in xcodebuild xcrun dotnet python3 xcpretty ditto; do
    command -v "$tool" >/dev/null 2>&1 || fail "Required tool is not available: $tool"
done
# Check SDK availability without querying or comparing the Xcode version.
xcrun --sdk iphoneos --show-sdk-path >/dev/null
xcrun --sdk iphonesimulator --show-sdk-path >/dev/null

if [[ -z "${JAVA_HOME:-}" ]]; then
    JAVA_HOME="$(/usr/libexec/java_home -v 21)"
fi
[[ -x "$JAVA_HOME/bin/java" ]] || fail "Java executable not found under JAVA_HOME: $JAVA_HOME"
JAVA_VERSION="$("$JAVA_HOME/bin/java" -version 2>&1)"
[[ "$JAVA_VERSION" == *'version "21.'* ]] || fail 'JDK 21 is required; set JAVA_HOME to a JDK 21 installation.'
ANDROID_HOME="${ANDROID_HOME:-${ANDROID_SDK_ROOT:-$HOME/Library/Android/sdk}}"
[[ -d "$ANDROID_HOME/platforms" && -d "$ANDROID_HOME/build-tools" ]] || fail "Android SDK is incomplete or missing: $ANDROID_HOME"
export JAVA_HOME ANDROID_HOME
export PATH="$JAVA_HOME/bin:$PATH"

cd "$SOLUTION_DIR"
[[ -f "$SOLUTION_PATH" ]] || fail "Solution file not found: $SOLUTION_PATH"
[[ -f "$WORKLOAD_PROJECT" ]] || fail "Workload project not found: $WORKLOAD_PROJECT"
DOTNET_VERSION="$(dotnet --version)"
[[ "$DOTNET_VERSION" == 10.* ]] || fail "The selected .NET SDK must be 10.x (found $DOTNET_VERSION)."
PROJECTS=(
    AMDevIT.Admob.Wrapper.Droid
    AMDevIT.Admob.Wrapper.iOSNative
    AMDevIT.Admob.Wrapper
    AMDevIT.Admob.Wrapper.MAUICross
)
BUILD_ARGS=("-p:JavaSdkDirectory=$JAVA_HOME" "-p:AndroidSdkDirectory=$ANDROID_HOME" "-p:ValidateXcodeVersion=false")
VERSION="$(dotnet msbuild "AMDevIT.Admob.Wrapper/AMDevIT.Admob.Wrapper.csproj" -getProperty:Version -nologo)"
VERSION="${VERSION//$'\r'/}"
[[ "$VERSION" =~ ^[0-9]+\.[0-9]+\.[0-9]+([.-][0-9A-Za-z.-]+)?$ ]] || fail "Invalid package version: $VERSION"
for project in "${PROJECTS[@]}"; do
    project_version="$(dotnet msbuild "$project/$project.csproj" -getProperty:Version -nologo)"
    [[ "${project_version//$'\r'/}" == "$VERSION" ]] || fail "Package versions differ: $project"
done

mkdir -p "$OUTPUT_ROOT"
OUTPUT_ROOT="$(cd "$OUTPUT_ROOT" && pwd)"
OUTPUT_DIR="$OUTPUT_ROOT/$VERSION"
[[ ! -e "$OUTPUT_DIR" ]] || fail "Output already exists: $OUTPUT_DIR. Choose another --output directory."
STAGING_DIR="$(mktemp -d "$OUTPUT_ROOT/.prepare-nuget.XXXXXX")"

printf '\nBuilding Android wrapper and running Kotlin tests...\n'
(
    cd "$ANDROID_DIR"
    bash ./gradlew :admob-wrapper:test :admob-wrapper:assembleRelease --console=plain
)
AAR_PATH="$ANDROID_DIR/admob-wrapper/build/outputs/aar/admob-wrapper-release.aar"
[[ -s "$AAR_PATH" ]] || fail "Android build did not produce $AAR_PATH"

printf '\nResolving Swift packages and building the iOS XCFramework...\n'
(
    cd "$IOS_DIR"
    xcodebuild -resolvePackageDependencies -project AdMobWrapper.xcodeproj -scheme AdMobWrapper
    bash ./build_xcframework.sh
)
XCFRAMEWORK_PATH="$IOS_DIR/build/AdMobWrapper.xcframework"
python3 - "$XCFRAMEWORK_PATH" <<'PY'
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
    raise SystemExit("XCFramework must contain iOS device and simulator slices.")
PY

printf '\nReplacing native binaries in the .NET bindings...\n'
mkdir -p "$SOLUTION_DIR/AMDevIT.Admob.Wrapper.Droid/Jars"
cp "$AAR_PATH" "$SOLUTION_DIR/AMDevIT.Admob.Wrapper.Droid/Jars/admob-wrapper-release.aar"
IOS_BINDING_LIBS="$SOLUTION_DIR/AMDevIT.Admob.Wrapper.iOSNative/libs"
mkdir -p "$IOS_BINDING_LIBS"
# Replace the complete framework so removed slices/files cannot survive an update.
rm -rf "$IOS_BINDING_LIBS/AdMobWrapper.xcframework"
ditto "$XCFRAMEWORK_PATH" "$IOS_BINDING_LIBS/AdMobWrapper.xcframework"

printf '\nRestoring workloads for the MAUI project...\n'
# The MAUI project covers Android, iOS and Mac Catalyst, including its bindings.
# Pass a project file: workload restore does not reliably discover .slnx from '.'.
dotnet workload restore "$WORKLOAD_PROJECT"
printf '\nRestoring solution packages...\n'
dotnet restore "$SOLUTION_PATH" "${BUILD_ARGS[@]}"
printf '\nBuilding solution in Release...\n'
dotnet build "$SOLUTION_PATH" --configuration Release --no-restore --no-incremental -m:1 "${BUILD_ARGS[@]}"

printf '\nPacking version %s...\n' "$VERSION"
for project in "${PROJECTS[@]}"; do
    [[ "$project" != AMDevIT.Admob.Wrapper.MAUICross ]] || continue
    dotnet pack "$project/$project.csproj" --configuration Release --no-build --no-restore --output "$STAGING_DIR" "${BUILD_ARGS[@]}"
done

printf '\nChecking packages...\n'
python3 "$SCRIPT_DIR/verify-nuget.py" "$STAGING_DIR" "$VERSION" "$SOLUTION_DIR" --native-preparation
[[ ! -e "$OUTPUT_DIR" ]] || fail "Output appeared during the build: $OUTPUT_DIR"
mv "$STAGING_DIR" "$OUTPUT_DIR"
STAGING_DIR=""
printf '\nPackages prepared at: %s\n' "$OUTPUT_DIR"
printf 'Intermediate native preparation only. MAUICross must be packed and verified on Windows before publication.\n'
printf 'Native binaries in the working tree were replaced. Review them before committing.\n'
