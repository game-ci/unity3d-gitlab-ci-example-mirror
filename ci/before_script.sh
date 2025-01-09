#!/usr/bin/env bash

set -euo pipefail

UNITY_BUILDER=../unity-builder

# Function for retrying commands with exponential backoff
retry_with_backoff() {
  local cmd="$1"
  local max_retries=5
  local delay=15
  local retry_count=0

  # Temporarily disable 'set -e' for this block
  set +e
  while [[ $retry_count -lt $max_retries ]]; do
    eval "$cmd"
    local exit_code=$?

    if [[ $exit_code -eq 0 ]]; then
      echo "Command succeeded"
      set -e
      return 0
    fi

    ((retry_count++))
    echo "::warning ::Command failed, attempting retry #$retry_count"
    echo "Retrying in $delay seconds..."
    sleep $delay
    delay=$((delay * 2))
  done
  set -e
  echo "Activation failed after $max_retries retries."
  return 1
}

# Clone unity-builder if not cached
if [ ! -d "$UNITY_BUILDER" ]; then
  git clone https://github.com/game-ci/unity-builder.git --depth 1 --branch v4.1.3 "$UNITY_BUILDER" && \
    cd "$UNITY_BUILDER" && \
    git verify-commit v4.1.3 && \
    cd -
fi

if [[ -n "$UNITY_SERIAL" && -n "$UNITY_EMAIL" && -n "$UNITY_PASSWORD" ]]; then
  #
  # SERIAL LICENSE MODE
  #
  # This will activate unity, using the serial activation process.
  #
  echo "Requesting activation by Serial Number"

  retry_with_backoff "unity-editor \
   -logFile /dev/stdout \
   -quit \
   -serial \"$UNITY_SERIAL\" \
   -username \"$UNITY_EMAIL\" \
   -password \"$UNITY_PASSWORD\" \
   -projectPath \"$UNITY_BUILDER/dist/BlankProject\""

  UNITY_EXIT_CODE=$? 

elif [[ -n "$UNITY_LICENSING_SERVER" ]]; then
  #
  # Custom Unity License Server
  #
  echo "Adding licensing server config"

  # Create temporary file with cleanup trap
  license_file=$(mktemp)
  trap 'rm -f "$license_file"' EXIT

  /opt/unity/Editor/Data/Resources/Licensing/Client/Unity.Licensing.Client --acquire-floating > "$license_file"
  UNITY_EXIT_CODE=$?

  # More robust parsing with validation
  PARSED_FILE=$(grep -oP '\".*?\"' < "$license_file" | tr -d '"')
  FLOATING_LICENSE=$(sed -n 2p <<< "$PARSED_FILE")
  FLOATING_LICENSE_TIMEOUT=$(sed -n 4p <<< "$PARSED_FILE")

  # Validate parsed values
  if [[ -z "$FLOATING_LICENSE" || -z "$FLOATING_LICENSE_TIMEOUT" ]]; then
    echo "::error ::Failed to parse license information"
    exit 1
  fi
  export FLOATING_LICENSE
  export FLOATING_LICENSE_TIMEOUT

  echo "Acquired floating license: \"$FLOATING_LICENSE\" with timeout $FLOATING_LICENSE_TIMEOUT"
else
  #
  # NO LICENSE ACTIVATION STRATEGY MATCHED
  #
  # This will exit since no activation strategies could be matched.
  #
  echo "License activation strategy could not be determined."
  echo ""
  echo "Visit https://game.ci/docs/github/activation for more"
  echo "details on how to set up one of the possible activation strategies."

  echo "::error ::No valid license activation strategy could be determined. Make sure to provide UNITY_EMAIL, UNITY_PASSWORD, and either a UNITY_SERIAL \
or UNITY_LICENSE. Otherwise please use UNITY_LICENSING_SERVER. See more info at https://game.ci/docs/github/activation"

  # Immediately exit as no UNITY_EXIT_CODE can be derived.
  exit 1;
fi

#
# Display information about the result
#
if [ $UNITY_EXIT_CODE -eq 0 ]; then
  # Activation was a success
  echo "Activation complete."
else
  # Activation failed so exit with the code from the license verification step
  echo "Unclassified error occured while trying to activate license."
  echo "Exit code was: $UNITY_EXIT_CODE"
  echo "::error ::There was an error while trying to activate the Unity license."
  exit $UNITY_EXIT_CODE
fi
