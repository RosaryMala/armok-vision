#!/bin/sh

# Unity plugins on OS X need to be in the .bundle format, but enet builds a
# .dylib by default. So, this script compiles the .dylibs into a .bundle.
# It's kinda terrible.
cd "$(readlink "$0")"

if [ ! -e ../enet/.libs/libenet.dylib ]; then
    echo 'Please build enet first!'
    exit 1
fi

cp ../enet/.libs/libenet.dylib ../enet-osx/

rm -rf build
mkdir build

xcodebuild -project ./Enet.xcodeproj -configuration Release clean build CONFIGURATION_BUILD_DIR=./build || exit 1

echo "All set! Copy build/Enet.bundle to the Unity assets folder and you're good to go."
