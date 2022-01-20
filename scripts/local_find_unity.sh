#!/usr/bin/env bash

unity_version=$(sed -n 's/^\m_EditorVersion: //p'< ./ProjectSettings/ProjectVersion.txt)
echo "Found Unity version: $unity_version"

if [[ "$OSTYPE" == "darwin"* ]] && [ -z "$UNITY_EXECUTABLE" ]; then
    unity_default_path="/Applications/Unity/Hub/Editor/${unity_version}/Unity.app/Contents/MacOS/Unity"
    if [ -f "${unity_default_path}" ]; then
        export UNITY_EXECUTABLE="${unity_default_path}"
        echo "Found 'UNITY_EXECUTABLE': '${UNITY_EXECUTABLE}'"
    else
        echo "Expected to find unity at '${unity_default_path}'. Please ensure Unity is installed and set 'UNITY_EXECUTABLE' env var to its location."
        exit 1
    fi
elif [[ "$OSTYPE" == "msys"* ]] && [ -z "$UNITY_EXECUTABLE" ]; then
    unity_default_path="C:/Program Files/Unity/Editor/${unity_version}/Editor/Unity.exe"
    if [ -f "${unity_default_path}" ]; then
        export UNITY_EXECUTABLE="${unity_default_path}"
        echo "Found 'UNITY_EXECUTABLE': '${UNITY_EXECUTABLE}'"
    else
        echo "Expected to find unity at '${unity_default_path}'. Please ensure Unity is installed and set 'UNITY_EXECUTABLE' env var to its location."
        exit 1
    fi
elif [[ "$OSTYPE" == "linux-gnu"* ]] && [ -z "$UNITY_EXECUTABLE" ]; then
    unity_default_path="/opt/Unity/Editor/${unity_version}/Editor/Unity"
    if [ -f "${unity_default_path}" ]; then
        export UNITY_EXECUTABLE="${unity_default_path}"
        echo "Found 'UNITY_EXECUTABLE': '${UNITY_EXECUTABLE}'"
    else
        echo "Expected to find unity at '${unity_default_path}'. Please ensure Unity is installed and set 'UNITY_EXECUTABLE' env var to its location."
        exit 1
    fi
elif [ -z "$UNITY_EXECUTABLE" ]; then
    echo "'UNITY_EXECUTABLE' env var not set, please provide path to Unity executable"
    exit 1
else
    echo "'UNITY_EXECUTABLE' env var set to '${UNITY_EXECUTABLE}'"
    if [ ! -f "${UNITY_EXECUTABLE}" ]; then
        echo "'UNITY_EXECUTABLE' does not exist at '${UNITY_EXECUTABLE}'"
        exit 1
    fi
fi
