# unity3d ci example

[![pipeline status](https://gitlab.com/gableroux/unity3d-gitlab-ci-example/badges/master/pipeline.svg)](https://gitlab.com/gableroux/unity3d-gitlab-ci-example/commits/master)
[![Build Status](https://travis-ci.com/GabLeRoux/unity3d-ci-example.svg?branch=master)](https://travis-ci.com/GabLeRoux/unity3d-ci-example)

This project is a PoC to **run unity3d tests and builds inside a CI** using **[gableroux/unity3d docker image](https://hub.docker.com/r/gableroux/unity3d/)**. It currently creates builds for Windows, Linux, MacOS and webgl. The webgl build is published by the CI to [gitlab-pages](https://about.gitlab.com/features/pages/) and [github-pages](https://pages.github.com/)! **You can try the built project on [the published gitlab-pages](https://gableroux.gitlab.io/unity3d-gitlab-ci-example/)**. 

_github-pages integration will be done in [GabLeRoux/unity3d-ci-example#4](https://github.com/GabLeRoux/unity3d-ci-example/issues/4)._

## Git remotes

This repository is hosted on multiple remotes to provide examples for [Gitlab-CI](https://about.gitlab.com/product/continuous-integration/), [Travis](https://travis-ci.org/) and [CircleCI](https://circleci.com/)

* [github](https://github.com/gableroux/unity3d-ci-example)
* [gitlab](https://gitlab.com/gableroux/unity3d-gitlab-ci-example)

<!-- START doctoc generated TOC please keep comment here to allow auto update -->
<!-- DON'T EDIT THIS SECTION, INSTEAD RE-RUN doctoc TO UPDATE -->
## Table of Contents  *generated with [DocToc](https://github.com/thlorenz/doctoc)*

- [Getting started](#getting-started)
- [Points of interest](#points-of-interest)
  - [Build script](#build-script)
  - [CI Configuration](#ci-configuration)
    - [gitlab-ci](#gitlab-ci)
    - [WIP: CircleCI](#wip-circleci)
    - [Travis](#travis)
  - [Test files](#test-files)
- [How to activate](#how-to-activate)
  - [Unity Personal](#unity-personal)
  - [Unity Plus/Pro](#unity-pluspro)
  - [Travis](#travis-1)
  - [Unity license per target](#unity-license-per-target)
- [How to add build targets](#how-to-add-build-targets)
  - [gitlab-ci](#gitlab-ci-1)
  - [iOS support](#ios-support)
  - [Android support](#android-support)
- [How to run scripts manually](#how-to-run-scripts-manually)
  - [Test](#test)
  - [Build](#build)
- [About the example project](#about-the-example-project)
- [Get involved](#get-involved)
- [Shameless plug](#shameless-plug)
- [License](#license)

<!-- END doctoc generated TOC please keep comment here to allow auto update -->

## Getting started

If you don't have a Unity project yet:

1. Fork this project from github or gitlab
2. Update the readme and remove undesired CI configurations
3. Follow How to activate instructions
4. Configure your CI

If you already have your own project:

1. Copy desired CI file
2. Update the Unity version according to your project version in the CI file. All versions are available at [gableroux/unity3d docker image](https://hub.docker.com/r/gableroux/unity3d/)
3. Copy build script (make sure you use the same path as original project, it must be in an `Editor` folder)
4. Follow How to activate instructions
5. Configure your CI

## Points of interest

This is probably what you're looking for.

### Build script

Script passed to the unity3d command line as argument to create builds

* See [`BuildScript.cs`](Assets/Scripts/Editor/BuildCommand.cs)

### CI Configuration

Pick one, if you're on gitlab, use gitlab-ci as Travis and CircleCI don't support Gitlab as of september 2018, if you're on github, Travis is more popular but CircleCI and [gitlab-ci will also work](https://about.gitlab.com/features/github/). If you can't decide, see [CircleCI vs. GitLab CI/CD](https://about.gitlab.com/comparison/gitlab-vs-circleci.html) and [Travis CI vs GitLab](https://about.gitlab.com/comparison/travis-ci-vs-gitlab.html).

#### gitlab-ci

* [`.gitlab-ci.yml`](.gitlab-ci.yml)

Note: you can add BuildOptions per target by adding environment variable `BuildOptions`.

```
build-ios:
  <<: *build
  image: gableroux/unity3d:2019.1.14f1-android
  variables:
    BUILD_TARGET: iOS
	BuildOptions: AcceptExternalModificationsToPlayer
```

If you would like to use several BuildOptions, you have to separate all values by `,` :  
```	
	BuildOptions: AcceptExternalModificationsToPlayer,CompressTextures,ConnectToHost
```

See [HERE](https://docs.unity3d.com/ScriptReference/BuildOptions.html) for BuildOptions values.

#### WIP: CircleCI

**TODO**

* [`.circleci/config.yml`](.circleci/config.yml)

#### Travis

* [`.travis.yml`](.travis.yml)

### Test files

* [`editmode` tests in `Assets/Scripts/Editor/EditModeTests`](Assets/Scripts/Editor/EditModeTests)
* [`playmode` tests in `Assets/Tests/`](Assets/Tests/)

## How to activate

You'll first need to run this locally. All you need is [docker](https://www.docker.com/) installed on your machine.

### Unity Personal

1. Clone this project
2. Pull the docker image and run bash inside, passing unity username and password to env

    _hint: you should write this to a shell script and execute the shell script so you don't have your credentials stored in your bash history_. Also make sure you use your Unity3d _email address_ for `UNITY_USERNAME` env var.

    ```bash
    UNITY_VERSION=2019.1.14f1
    docker run -it --rm \
    -e "UNITY_USERNAME=username@example.com" \
    -e "UNITY_PASSWORD=example_password" \
    -e "TEST_PLATFORM=linux" \
    -e "WORKDIR=/root/project" \
    -v "$(pwd):/root/project" \
    gableroux/unity3d:$UNITY_VERSION \
    bash
    ```
3. In Unity docker container's bash, run once like this, it will try to activate

    ```bash
    xvfb-run --auto-servernum --server-args='-screen 0 640x480x24' \
    /opt/Unity/Editor/Unity \
    -logFile /dev/stdout \
    -batchmode \
    -username "$UNITY_USERNAME" -password "$UNITY_PASSWORD"
    ```

4. Wait for output that looks like this:

    ```
    LICENSE SYSTEM [2017723 8:6:38] Posting <?xml version="1.0" encoding="UTF-8"?><root><SystemInfo><IsoCode>en</IsoCode><UserName>[...]
    ```
    If you get the following error:
    
    > Can't activate unity: No sufficient permissions while processing request HTTP error code 401
    
    Make sure your credentials are valid. You may try to disable 2FA in your account and try again. Once done, you should enable 2FA again for security reasons. See [#11](https://gitlab.com/gableroux/unity3d-gitlab-ci-example/issues/11) for more details.

5. Copy xml content and save as `unity3d.alf`
6. Open https://license.unity3d.com/manual and answer questions
7. Upload `unity3d.alf` for manual activation
8. Download `Unity_v2018.x.ulf` # TODO: confirm new file name for 2019
9. Copy the content of `Unity_v2018.x.ulf` license file to your CI's environment variable `UNITY_LICENSE_CONTENT`.
   _Note: if you are doing this on windows, chances are the [line endings will be wrong as explained here](https://gitlab.com/gableroux/unity3d-gitlab-ci-example/issues/5#note_95831816). Luckily for you, [`.gitlab-ci.yml`](.gitlab-ci.yml) solves this by removing `\r` character from the env variable so you'll be alright_
[`.gitlab-ci.yml`](.gitlab-ci.yml) will then place the `UNITY_LICENSE_CONTENT` to the right place before running tests or creating the builds.

### Unity Plus/Pro

1. Clone this project
2. Pull the docker image and run bash inside, passing unity username and password to env

    _hint: you should write this to a shell script and execute the shell script so you don't have your credentials stored in your bash history_. Also make sure you use your Unity3d _email address_ for `UNITY_USERNAME` env var.

    ```bash
    UNITY_VERSION=2018.2.3f1
    docker run -it --rm \
    -e "UNITY_USERNAME=username@example.com" \
    -e "UNITY_PASSWORD=example_password" \
    -e "UNITY_SERIAL=AN-EXAM-PLE-SERIA-LKEY-1234" \
    -e "TEST_PLATFORM=linux" \
    -e "WORKDIR=/root/project" \
    -v "$(pwd):/root/project" \
    gableroux/unity3d:$UNITY_VERSION \
    bash
    ```
3. In Unity docker container's bash, run once like this, it will try to activate

    ```bash
    xvfb-run --auto-servernum --server-args='-screen 0 640x480x24' \
    /opt/Unity/Editor/Unity \
    -logFile /dev/stdout \
    -batchmode \
    -username "$UNITY_USERNAME" -password "$UNITY_PASSWORD" -serial "$UNITY_SERIAL"
    ```
4. Wait for the command to finish without errors
5. Obtain the contents of the license file by running `cat /root/.local/share/unity3d/Unity/Unity_lic.ulf`
5. Copy the content to your CI's environment variable `UNITY_LICENSE_CONTENT`.
   _Note: if you are doing this on windows, chances are the [line endings will be wrong as explained here](https://gitlab.com/gableroux/unity3d-gitlab-ci-example/issues/5#note_95831816). Luckily for you, [`.gitlab-ci.yml`](.gitlab-ci.yml) solves this by removing `\r` character from the env variable so you'll be alright_
[`.gitlab-ci.yml`](.gitlab-ci.yml) will then place the `UNITY_LICENSE_CONTENT` to the right place before running tests or creating the builds.

### Travis

Travis doesn't support multiple-lines env variable out of the box and I had troubles with escaping so I recommend encrypting the license file. `.travis.yml` will decrypt the file and add its content to `UNITY_LICENSE_CONTENT` env var itself afterward.

```bash
travis encrypt-file --pro -r YOUR_TRAVIS_USERNAME/YOUR_TRAVIS_REPO_NAME ./Unity_v2018.x.ulf # TODO confirm new file name for 2019
```

For the record, the message I was getting:

> The previous command failed, possibly due to a malformed secure environment variable.
>  Please be sure to escape special characters such as ' ' and '$'.
>  For more information, see https://docs.travis-ci.com/user/encryption-keys.

### Unity license per target

If you need a specific Unity license for a build target, you can add environment var `UNITY_LICENSE_CONTENT_{BUILD_TARGET}`. (`UNITY_LICENSE_CONTENT_ANDROID`, `UNITY_LICENSE_CONTENT_IOS`, ...)

## How to add build targets

Supported build targets can be found [here](https://docs.unity3d.com/ScriptReference/BuildTarget.html)

### gitlab-ci

Update [`.gitlab-ci.yml`](.gitlab-ci.yml) by adding a build section like this:

```yaml
build-StandaloneWindows64:
  <<: *build
  variables:
    BUILD_TARGET: StandaloneWindows64
```

### iOS support

**Help wanted!** See [#16](https://gitlab.com/gableroux/unity3d-gitlab-ci-example/issues/16)

### Android support

To make build working with Android, you will need a specific Unity license (because that is not the same docker image).  
Add the content of your specific Unity license in your CI's environment variable : `UNITY_LICENSE_CONTENT_ANDROID`

By default the apk is not signed and the build will use the Unity's default debug key.  
For security reason, you must not add your keystore to git.  
Encode your keystore file as base64 using openssl:  
`openssl base64 -A -in yourKeystore.keystore`

Copy the result to your CI's environment variable `ANDROID_KEYSTORE_BASE64`

Add following environment variables:  
- `KEYSTORE_PASS` : Keystore pass  
- `KEY_ALIAS_NAME` : Keystore alias name to use (if not set, the program will use the alias name set in Project's PlayerSettings)  
- `KEY_ALIAS_PASS` : Keystore alias pass to use

Note about keystore security, if you would like to use another solution, see [HERE](https://android.jlelse.eu/where-to-store-android-keystore-file-in-ci-cd-cycle-2365f4e02e57)

## How to run scripts manually

You can execute the local scripts and specify the path of your Unity executable using `UNITY_EXECUTABLE`. You may try this in your project before you setup the whole CI so you confirm it works with your current unity version :+1:

### Test

```bash
UNITY_EXECUTABLE="/Applications/Unity/Hub/Editor/2019.1.14f1/Unity.app/Contents/MacOS/Unity" \
  ./local_test.sh
```

### Build

```bash
UNITY_EXECUTABLE="/Applications/Unity/Hub/Editor/2019.1.14f1/Unity.app/Contents/MacOS/Unity" \
  ./local_build.sh
```

## About the example project

This is an updated version of the [Unity's Creator Kit: RPG free asset](https://assetstore.unity.com/packages/templates/tutorials/creator-kit-rpg-149309) which is not affiliated with this project at all. Feel free to explore it, dialogs are updated ;)

## Get involved

There is a discord `#technical-english` channel at [totema.studio/discord](https://totema.studio/discord). Feel free to join in! I will be looking for maintainers as this project is getting more and more attention :tada:.

## Shameless plug

I made this for free as a gift to the video game community so if this tool helped you and you would like to support me, send your love to [Totema Studio](https://totemastudio.com) on Patreon: :beers:

[![Totema Studio Logo](./doc/totema-studio-logo-217.png)](https://patreon.com/totemastudio)

[![Become a Patron](./doc/become_a_patron_button.png)](https://www.patreon.com/bePatron?c=1073078)

## License

[MIT](LICENSE.md) © [Gabriel Le Breton](https://gableroux.com)

