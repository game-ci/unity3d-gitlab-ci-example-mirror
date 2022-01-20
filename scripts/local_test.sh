#!/usr/bin/env bash

source ./scripts/local_find_unity.sh

set -x

TEST_PLATFORM=editmode ./ci/test.sh
TEST_PLATFORM=playmode ./ci/test.sh
