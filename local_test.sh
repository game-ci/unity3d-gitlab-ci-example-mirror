#!/usr/bin/env sh

set -x

export UNITY_EXECUTABLE=${UNITY_EXECUTABLE:-"/Applications/Unity/Hub/Editor/2020.1.17f1/Unity.app/Contents/MacOS/Unity"}

TEST_PLATFORM=editmode ./ci/test.sh
TEST_PLATFORM=playmode ./ci/test.sh
