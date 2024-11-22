#!/usr/bin/env bash

set -euo pipefail

if [[ -n "${UNITY_LICENSING_SERVER:-}" ]]; then
  if [[ -z "${FLOATING_LICENSE:-}" ]]; then
    echo "Error: FLOATING_LICENSE environment variable is not set" >&2
    exit 1
  fi

  echo "Returning floating license: \"$FLOATING_LICENSE\""
  if ! /opt/unity/Editor/Data/Resources/Licensing/Client/Unity.Licensing.Client --return-floating "$FLOATING_LICENSE"; then
    echo "Error: Failed to return floating license" >&2
    exit 1
  fi
elif [[ -n "${UNITY_SERIAL:-}" ]]; then
  # Validate required environment variables
  for var in UNITY_EMAIL UNITY_PASSWORD; do
    if [[ -z "${!var:-}" ]]; then
      echo "Error: $var environment variable is not set" >&2
      exit 1
    fi
  done

  echo "Returning serial license for user: $UNITY_EMAIL"
  project_path="../unity-builder/dist/BlankProject"
  if [[ ! -d "$project_path" ]]; then
    echo "Error: Project path not found. Ensure before_script.sh was executed successfully" >&2
    exit 1
  fi

  unity-editor \
    -logFile /dev/stdout \
    -quit \
    -returnlicense \
    -username "$UNITY_EMAIL" \
    -password "$UNITY_PASSWORD" \
    -projectPath "$project_path" || {
      echo "Error: Failed to return serial license" >&2
      exit 1
    }
else
  echo "Error: Neither UNITY_LICENSING_SERVER nor UNITY_SERIAL is set" >&2
  exit 1
fi
