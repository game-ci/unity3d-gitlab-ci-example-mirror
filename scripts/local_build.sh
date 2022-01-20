#!/usr/bin/env bash

source ./scripts/local_find_unity.sh

set -x

export BUILD_NAME=${BUILD_NAME:-"ExampleProjectName"}

BUILD_TARGET=StandaloneLinux64 ./ci/build.sh
BUILD_TARGET=StandaloneOSX ./ci/build.sh
BUILD_TARGET=StandaloneWindows64 ./ci/build.sh
BUILD_TARGET=WebGL ./ci/build.sh
