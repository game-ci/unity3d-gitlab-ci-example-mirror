#!/usr/bin/env bash

set -e
set -x
mkdir -p /root/.cache/unity3d
mkdir -p /root/.local/share/unity3d/Unity/
set +x
echo 'Writing $UNITY_LICENSE_CONTENT to license file /root/.local/share/unity3d/Unity/Unity_lic.ulf'
echo "$UNITY_LICENSE_CONTENT" | tr -d '\r' > /root/.local/share/unity3d/Unity/Unity_lic.ulf

set -x

# Workaround for https://gitlab.com/gableroux/unity3d/issues/35
# Prevents ALSA lib errors
cp ci/etc/asound.conf /etc/asound.conf

# Workaround for https://gitlab.com/gableroux/unity3d-gitlab-ci-example/issues/36
# webgl build fails due to missing ffmpeg otherwise
apt-get update && apt-get install -y ffmpeg
