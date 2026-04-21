#!/bin/bash

echo "Extracting classes and interfaces from xcframework"

sharpie bind \
  ./build/AdMobWrapper.xcframework/ios-arm64/AdMobWrapper.framework/Headers/*.h \
  -scope="./build/AdMobWrapper.xcframework/ios-arm64/AdMobWrapper.framework/Headers" \
  -o "./sharpie-output" \
  -n "AMDevIT.Admob.Wrapper" \
  -c -F "$HOME/Library/Developer/Xcode/DerivedData" \

