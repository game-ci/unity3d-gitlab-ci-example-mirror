#!/usr/bin/env bash

set -x

echo "Testing for $TEST_PLATFORM"

CODE_COVERAGE_PACKAGE="com.unity.testtools.codecoverage"
PACKAGE_MANIFEST_PATH="Packages/manifest.json"

${UNITY_EXECUTABLE:-xvfb-run --auto-servernum --server-args='-screen 0 640x480x24' unity-editor} \
  -projectPath $UNITY_DIR \
  -runTests \
  -testPlatform $TEST_PLATFORM \
  -testResults $UNITY_DIR/$TEST_PLATFORM-results.xml \
  -logFile /dev/stdout \
  -batchmode \
  -nographics \
  -enableCodeCoverage \
  -coverageResultsPath $UNITY_DIR/$TEST_PLATFORM-coverage \
  -coverageOptions "generateAdditionalMetrics;generateHtmlReport;generateHtmlReportHistory;generateBadgeReport;assemblyFilters:+Assembly-CSharp" \
  -debugCodeOptimization

UNITY_EXIT_CODE=$?

if [ $UNITY_EXIT_CODE -eq 0 ]; then
  echo "Run succeeded, no failures occurred";
elif [ $UNITY_EXIT_CODE -eq 2 ]; then
  echo "Run succeeded, some tests failed";
elif [ $UNITY_EXIT_CODE -eq 3 ]; then
  echo "Run failure (other failure)";
else
  echo "Unexpected exit code $UNITY_EXIT_CODE";
fi

if grep $CODE_COVERAGE_PACKAGE $PACKAGE_MANIFEST_PATH; then
  cat $UNITY_DIR/$TEST_PLATFORM-coverage/Report/Summary.xml | grep Linecoverage
  mv $UNITY_DIR/$TEST_PLATFORM-coverage/$CI_PROJECT_NAME-opencov/*Mode/TestCoverageResults_*.xml $UNITY_DIR/$TEST_PLATFORM-coverage/coverage.xml
  rm -r $UNITY_DIR/$TEST_PLATFORM-coverage/$CI_PROJECT_NAME-opencov/
else
  {
    echo -e "\033[33mCode Coverage package not found in $PACKAGE_MANIFEST_PATH. Please install the package \"Code Coverage\" through Unity's Package Manager to enable coverage reports.\033[0m"
  } 2> /dev/null
fi

cat $UNITY_DIR/$TEST_PLATFORM-results.xml | grep test-run | grep Passed
exit $UNITY_EXIT_CODE
