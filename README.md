# unity3d gitlab-ci example

[![pipeline status](https://gitlab.com/gableroux/unity3d-gitlab-ci-example/badges/master/pipeline.svg)](https://gitlab.com/gableroux/unity3d-gitlab-ci-example/commits/master)

This project is a PoC to run unity3d tests and builds inside gitlab-ci using [gableroux/unity3d docker image](https://hub.docker.com/r/gableroux/unity3d/). It currently creates builds for Windows, Linux, MacOS and webgl. The webgl build gets published to [gitlab-pages](https://about.gitlab.com/features/pages/)!

## Points of interest

### gitlab-ci

This is probably what you're looking for, have a look to [`.gitlab-ci.yml`](.gitlab-ci.yml)

### build script

File passed to the unity3d command line as argument to create builds

See [`BuildScript.cs`](Assets/Scripts/Editor/BuildCommand.cs)

### test files

Very basic `editmode` and `playmode` tests (all passing) can be found in [Assets/Scripts/Editor/EditModeTests](Assets/Scripts/Editor/EditModeTests) and [Assets/Scripts/Editor/PlayModeTests](Assets/Scripts/Editor/PlayModeTests)

### CI Pipelines and artifacts

See [project's pipelines](https://gitlab.com/gableroux/unity3d-gitlab-ci-example/pipelines) which contains **built artifacts** 🎉 .

## How to execute the tests

For current project, outside of docker, one can run the tests from command line as usual this way:

```bash
path/to/unity -runTests -projectPath $(pwd) -testResults $(pwd)/editmode-results.xml -testPlatform editmode
path/to/unity -runTests -projectPath $(pwd) -testResults $(pwd)/playmode-results.xml -testPlatform playmode
```

## How to activate

You'll first need to run this locally. All you need is [docker](https://www.docker.com/) installed on your machine.

1. Clone this project
2. Pull the docker image and run bash inside, passing unity username and password to env

    _hint: you should write this to a shell script and execute the shell script so you don't have your credentials stored in your bash history_

    ```bash
    docker run -it --rm \
    -e "UNITY_USERNAME=username@example.com" \
    -e "UNITY_PASSWORD=example_password" \
    -e "TEST_PLATFORM=linux" \
    -e "WORKDIR=/root/project" \
    -v "$(pwd):/root/project" \
    gableroux/unity3d:latest \
    bash
    ```

3. In Unity docker container's bash, run once like this, it will try to activate

    ```bash
    xvfb-run --auto-servernum --server-args='-screen 0 640x480x24' \
    /opt/Unity/Editor/Unity \
    -logFile \
    -batchmode \
    -username "$UNITY_USERNAME" -password "$UNITY_PASSWORD"
    ```

4. Wait for output that looks like this:

    ```
    LICENSE SYSTEM [2017723 8:6:38] Posting <?xml version="1.0" encoding="UTF-8"?><root><SystemInfo><IsoCode>en</IsoCode><UserName>[...]
    ```

5. Copy xml content and save as `unity3d.alf`
6. Open https://license.unity3d.com/manual
7. Upload `unity3d.alf` for manual activation
8. Download `Unity_v2017.x.ulf`
9. Copy the content of `Unity_v2017.x.ulf` license file to your CI's environment variable `UNITY_LICENSE_CONTENT`.

[`.gitlab-ci.yml`](.gitlab-ci.yml) will then place the `UNITY_LICENSE_CONTENT` to the right place before running tests or creating the builds.

## How to add build targets

Update [`.gitlab-ci.yml`](.gitlab-ci.yml) by adding a build section like this:

```yaml
build-StandaloneWindows64:
  <<: *build
  variables:
    BUILD_TARGET: StandaloneWindows64
```

Supported build targets can be found [here](https://docs.unity3d.com/ScriptReference/BuildTarget.html)

## License

[MIT](LICENSE.md) © [Gabriel Le Breton](https://gableroux.com)
