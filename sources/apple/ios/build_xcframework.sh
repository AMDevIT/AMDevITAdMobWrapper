#!/bin/bash

# build_xcframework.sh
# Compiles iOS archives and creates the AdMobWrapper xcframework

set -e

SCHEME="AdMobWrapper"
PROJECT_PATH="AdMobWrapper.xcodeproj"
OUTPUT_DIR="build"
XCFRAMEWORK_NAME="AdMobWrapper"

# Check and install xcpretty if needed
if ! command -v xcpretty &> /dev/null; then
    echo "⚙️ Installing xcpretty..."
    gem install xcpretty
fi

echo "🧹 Cleaning previous build..."
rm -rf "$OUTPUT_DIR/$XCFRAMEWORK_NAME.xcframework"
rm -rf "$OUTPUT_DIR/$XCFRAMEWORK_NAME-iOS.xcarchive"
rm -rf "$OUTPUT_DIR/$XCFRAMEWORK_NAME-iOSSimulator.xcarchive"

echo "📱 Building archive for device (arm64)..."
xcodebuild archive \
    -scheme "$SCHEME" \
    -project "$PROJECT_PATH" \
    -destination "generic/platform=iOS" \
    -archivePath "$OUTPUT_DIR/$XCFRAMEWORK_NAME-iOS" \
    SKIP_INSTALL=NO \
    BUILD_LIBRARY_FOR_DISTRIBUTION=YES \
    SWIFT_INSTALL_OBJC_HEADER=YES \
    SWIFT_OBJC_INTERFACE_HEADER_NAME="$XCFRAMEWORK_NAME-Swift.h" \
    | xcpretty || true

echo "🖥️ Building archive for simulator (arm64 + x86_64)..."
xcodebuild archive \
    -scheme "$SCHEME" \
    -project "$PROJECT_PATH" \
    -destination "generic/platform=iOS Simulator" \
    -archivePath "$OUTPUT_DIR/$XCFRAMEWORK_NAME-iOSSimulator" \
    SKIP_INSTALL=NO \
    BUILD_LIBRARY_FOR_DISTRIBUTION=YES \
    SWIFT_INSTALL_OBJC_HEADER=YES \
    SWIFT_OBJC_INTERFACE_HEADER_NAME="$XCFRAMEWORK_NAME-Swift.h" \
    | xcpretty || true

echo "📦 Creating xcframework..."
xcodebuild -create-xcframework \
    -framework "$OUTPUT_DIR/$XCFRAMEWORK_NAME-iOS.xcarchive/Products/Library/Frameworks/$XCFRAMEWORK_NAME.framework" \
    -framework "$OUTPUT_DIR/$XCFRAMEWORK_NAME-iOSSimulator.xcarchive/Products/Library/Frameworks/$XCFRAMEWORK_NAME.framework" \
    -output "$OUTPUT_DIR/$XCFRAMEWORK_NAME.xcframework"

echo "✅ xcframework created at $OUTPUT_DIR/$XCFRAMEWORK_NAME.xcframework"