#!/usr/bin/env bash

echo "Building for $BUILD_TARGET"
export BUILD_PATH=./Builds/$BUILD_TARGET/
mkdir -p $BUILD_PATH

xvfb-run --auto-servernum --server-args='-screen 0 640x480x24' \
  /opt/Unity/Editor/Unity \
    -projectPath $(pwd) \
    -quit \
    -batchmode \
    -buildTarget $BUILD_TARGET \
    -customBuildTarget $BUILD_TARGET \
    -customBuildName $BUILD_NAME \
    -customBuildPath $BUILD_PATH \
    -customBuildOptions AcceptExternalModificationsToPlayer \
    -executeMethod BuildCommand.PerformBuild \
    -logFile

ls -la $BUILD_PATH
[ -n "$(ls -A $BUILD_PATH)" ] # fail job if build folder is empty
