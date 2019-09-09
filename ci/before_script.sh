#!/usr/bin/env bash

set -e
set -x
mkdir -p /root/.cache/unity3d
mkdir -p /root/.local/share/unity3d/Unity/
set +x

LICENSE="UNITY_LICENSE_CONTENT_"${BUILD_TARGET^^}

if [ -z "${!LICENSE}" ]
then
    echo "$LICENSE env var not found, using default UNITY_LICENSE_CONTENT env var"
    LICENSE=UNITY_LICENSE_CONTENT
else
    echo "Using $LICENSE env var"
fi

echo "Writing $LICENSE to license file /root/.local/share/unity3d/Unity/Unity_lic.ulf"
echo "${!LICENSE}" | tr -d '\r' > /root/.local/share/unity3d/Unity/Unity_lic.ulf

set -x