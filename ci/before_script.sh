#!/usr/bin/env bash

mkdir -p /root/.cache/unity3d
mkdir -p /root/.local/share/unity3d/Unity/
echo "$UNITY_LICENSE_CONTENT" | tr -d '\r' > /root/.local/share/unity3d/Unity/Unity_lic.ulf