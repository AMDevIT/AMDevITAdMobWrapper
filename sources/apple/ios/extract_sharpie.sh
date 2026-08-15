#!/bin/bash

set -euo pipefail

SCHEME="AdMobWrapper"
PROJECT_PATH="AdMobWrapper.xcodeproj"
FRAMEWORK_PATH="./build/AdMobWrapper.xcframework/ios-arm64/AdMobWrapper.framework"
HEADER_PATH="$FRAMEWORK_PATH/Headers/AdMobWrapper-Swift.h"
OUTPUT_PATH="./sharpie-output"

if ! command -v sharpie > /dev/null 2>&1; then
    echo "Objective Sharpie is not installed or is not available in PATH." >&2
    exit 1
fi

if [[ ! -d "$FRAMEWORK_PATH" ]]; then
    echo "Framework not found at $FRAMEWORK_PATH. Run build_xcframework.sh first." >&2
    exit 1
fi

if [[ ! -f "$HEADER_PATH" ]]; then
    echo "Framework header not found at $HEADER_PATH." >&2
    exit 1
fi

echo "Resolving Xcode DerivedData directory..."

BUILD_DIR="$(
    xcodebuild \
        -project "$PROJECT_PATH" \
        -scheme "$SCHEME" \
        -destination "generic/platform=iOS" \
        -showBuildSettings 2>/dev/null |
    awk -F ' = ' '/^[[:space:]]*BUILD_DIR = / { print $2; exit }'
)"

if [[ -z "$BUILD_DIR" ]]; then
    echo "Unable to determine Xcode BUILD_DIR." >&2
    exit 1
fi

DERIVED_DATA_DIR="${BUILD_DIR%%/Build/*}"
SOURCE_PACKAGES_DIR="$DERIVED_DATA_DIR/SourcePackages"

if [[ ! -d "$SOURCE_PACKAGES_DIR" ]]; then
    echo "Swift Package Manager directory not found at $SOURCE_PACKAGES_DIR." >&2
    exit 1
fi

echo "DerivedData: $DERIVED_DATA_DIR"
echo "Swift packages: $SOURCE_PACKAGES_DIR"

CLANG_ARGS=()

while IFS= read -r framework; do
    framework_dir="$(dirname "$framework")"

    echo "Adding framework search path: $framework_dir"

    CLANG_ARGS+=("-F$framework_dir")
done < <(
    find "$SOURCE_PACKAGES_DIR/artifacts" \
        -type d \
        -path '*.xcframework/ios-arm64/*.framework' \
        2>/dev/null |
    sort -u
)

if [[ ${#CLANG_ARGS[@]} -eq 0 ]]; then
    echo "No SwiftPM binary framework dependencies found." >&2
    exit 1
fi

echo
echo "Extracting classes and interfaces from xcframework"

sharpie bind --header "$HEADER_PATH" -o "$OUTPUT_PATH" -n "AMDevIT.Admob.Wrapper" -sdk iphoneos -v -c "${CLANG_ARGS[@]}"

if [[ -z "$(find "$OUTPUT_PATH" -maxdepth 1 -type f -name '*.cs' -print -quit)" ]]; then
    echo "Objective Sharpie completed without generating C# files in $OUTPUT_PATH." >&2
    exit 1
fi
