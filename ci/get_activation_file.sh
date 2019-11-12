#!/usr/bin/env bash

activation_file=${UNITY_ACTIVATION_FILE:-./unity3d.alf}

if [[ -z "${UNITY_USERNAME}" ]] || [[ -z "${UNITY_PASSWORD}" ]]; then
  echo "UNITY_USERNAME or UNITY_PASSWORD environment variables are not set, please refer to instructions in the readme and add these to your secret environment variables."
  exit 1
fi

xvfb-run --auto-servernum --server-args='-screen 0 640x480x24' \
  /opt/Unity/Editor/Unity \
    -logFile /dev/stdout \
    -batchmode \
    -username "$UNITY_USERNAME" -password "$UNITY_PASSWORD" |
      tee ./unity-output.log

cat ./unity-output.log |
  grep 'LICENSE SYSTEM .* Posting *' |
  sed 's/.*Posting *//' > "${activation_file}"

# Fail job if unity.alf is empty
ls "${UNITY_ACTIVATION_FILE:-./unity3d.alf}"
exit_code=$?

if [[ ${exit_code} -eq 0 ]]; then
  echo ""
  echo ""
  echo "### Congratulations! ###"
  echo "${activation_file} was generated successfully!"
  echo ""
  echo "### Next steps ###"
  echo "From here, you have two options:"
  echo ""
  echo "a. Complete the activation process manually"
  echo "   difficulty: easy"
  echo ""
  echo "   1. Download the artifact which should contain ${activation_file}"
  echo "   2. Visit https://license.unity3d.com/manual and answer questions"
  echo "   3. Upload ${activation_file} in the form (after you answered the questions)"
  echo "   4. Download 'Unity_v2019.x.ulf' file (year should match your unity version here, 'Unity_v2018.x.ulf' for 2018, etc.)"
  echo "   5. Copy the content of 'Unity_v2019.x.ulf' license file to your CI's environment variable 'UNITY_LICENSE_CONTENT'. (Open your project's parameters > CI/CD > Variables and add 'UNITY_LICENSE_CONTENT' as the key and paste the content of the license file into the value)"
  echo ""
  echo "b. Complete the activation process using docker gableroux/unity3d-activator"
  echo "   difficulty: intermediate"
  echo ""
  echo "   1. Download the artifact which should contain ${activation_file}"
  echo "   2. Open a shell in the folder where you downloaded the activation file"
  echo "   3. Run these commands:"
  echo ""
  echo "git clone https://gitlab.com/gableroux/unity3d-activator"
  echo "cp ${activation_file} ./unity3d-activator/"
  echo "cd ./unity3d-activator"
  echo "cp .env.example .env"
  echo "# Edit and fill in your secrets in .env (open with an editor or whatever) and make sure UNITY_ACTIVATION_FILE is set to ${activation_file}, then"
  echo "docker-compose run activate"
  echo ""
  echo "   4. Locate the newly generated license file in the current folder"
  echo "   5. Copy the content of 'Unity_v2019.x.ulf' license file to your CI's environment variable 'UNITY_LICENSE_CONTENT'. (Open your project's parameters > CI/CD > Variables and add 'UNITY_LICENSE_CONTENT' as the key and paste the content of the license file into the value)"
  echo ""
  echo "   You can find more details about the unity activator image here:"
  echo "   https://gitlab.com/gableroux/unity3d-activator"
  echo ""
  echo "Visit https://gitlab.com/gableroux/unity3d-gitlab-ci-example/issues/73 for more details"
else
  echo "License file could not be found at ${UNITY_ACTIVATION_FILE:-./unity3d.alf}"
fi
exit $exit_code
