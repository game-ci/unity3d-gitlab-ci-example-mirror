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
        - [a. Using gitlab-ci](#a-using-gitlab-ci)
        - [b. Locally](#b-locally)
    - [Unity Plus/Pro](#unity-pluspro)
    - [Unity license per target](#unity-license-per-target)
        - [Note about components in recent images](#note-about-components-in-recent-images)
    - [Travis](#travis-1)
- [How to add build targets](#how-to-add-build-targets)
    - [gitlab-ci](#gitlab-ci-1)
    - [iOS support](#ios-support)
        - [Setup (only one time per mac)](#setup-only-one-time-per-mac)
        - [Unity Settings](#unity-settings)
        - [XCode project](#xcode-project)
        - [App on portail](#app-on-portail)
        - [Fastlane initialization](#fastlane-initialization)
        - [Provisioning profile](#provisioning-profile)
        - [Make lane](#make-lane)
        - [Run tests locally](#run-tests-locally)
        - [Gitlab-runner - register your mac](#gitlab-runner-register-your-mac)
    - [Android support](#android-support)
        - [Android app bundle](#android-app-bundle)
        - [Bundle version code](#bundle-version-code)
        - [Fastlane supply (deployement)](#fastlane-supply-deployement)
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
1. Update the readme and remove undesired CI configurations
1. Follow How to activate instructions
1. Configure your CI environment variables (once everything is set, you should only need `UNITY_LICENSE_CONTENT` in your project variables)

If you already have your own project:

1. Copy desired ci file (`.gitlab-ci.yml`, or `.travis.yml`, etc.)
1. Copy [`BuildScript.cs`](Assets/Scripts/Editor/BuildCommand.cs) (make sure you use the same path as original project, it must be in an `Editor` folder)
1. Copy [`ci` folder](ci)
1. Update the Unity version according to your project version in the CI file. All versions are available at [gableroux/unity3d docker image](https://hub.docker.com/r/gableroux/unity3d/)
1. Follow How to activate instructions
1. Configure your CI environment variables (once everything is set, you should only need `UNITY_LICENSE_CONTENT` in your project variables)

## Points of interest

This is probably what you're looking for.

### Build script

Script passed to the unity3d command line as argument to create builds

* See [`BuildScript.cs`](Assets/Scripts/Editor/BuildCommand.cs)

You need to have this file in your project in order to build your project in the CI.

### CI Configuration

Pick one, if you're on gitlab, use gitlab-ci as Travis and CircleCI don't support Gitlab as of september 2018, if you're on github, Travis is more popular but CircleCI and [gitlab-ci will also work](https://about.gitlab.com/features/github/). If you can't decide, see [CircleCI vs. GitLab CI/CD](https://about.gitlab.com/comparison/gitlab-vs-circleci.html) and [Travis CI vs GitLab](https://about.gitlab.com/comparison/travis-ci-vs-gitlab.html).

You need to have one of these files in your project in order to build your project to actually use your CI.

#### gitlab-ci

* [`.gitlab-ci.yml`](.gitlab-ci.yml)

Note: you can add BuildOptions per target by adding environment variable `BuildOptions`.

```yaml
build-ios:
  <<: *build
  image: gableroux/unity3d:2019.3.7f1-android
  variables:
    BUILD_TARGET: iOS
	BuildOptions: AcceptExternalModificationsToPlayer
```

If you would like to use several BuildOptions, you have to separate all values by `,` :  

```	yaml
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

There are a few methods available, if you're using gitlab-ci, the easiest method in the current documentation is using gitlab-ci.

### Unity Personal

#### a. Using gitlab-ci

Once you've added all required files to your project (mainly `.gitlab-ci.yml`), there should be a manual step that can be triggered for activation. 

1. Visit your project's settings > CI/CD > Variables and add `UNITY_USERNAME` and `UNITY_PASSWORD` with your credentials
1. Push your first commit to your project and visit CI/CD Pipelines.
1. Locate your latest job, there should be a `play` button, click on it and click `get-activation-file`
1. Wait for the job to run and follow instructions in the console 

#### b. Locally

All you need is [docker](https://www.docker.com/) installed on your machine.

1. Clone this project
2. Pull the docker image and run bash inside, passing unity username and password to env

    _hint: you should write this to a shell script and execute the shell script so you don't have your credentials stored in your bash history_. Also make sure you use your Unity3d _email address_ for `UNITY_USERNAME` env var.

    ```bash
    UNITY_VERSION=2019.3.7f1
    docker run -it --rm \
    -e "UNITY_USERNAME=username@example.com" \
    -e "UNITY_PASSWORD=example_password" \
    -e "TEST_PLATFORM=linux" \
    -e "WORKDIR=/root/project" \
    -v "$(pwd):/root/project" \
    gableroux/unity3d:$UNITY_VERSION \
    bash
    ```
    
    If your password contains a `!`, you can escape it like this (`example_pass!word`):
    
     ```bash
     ...
     -e "UNITY_PASSWORD=example_pass"'!'"word" \
     ...
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
8. Download `Unity_v2018.x.ulf` (`Unity_v2019.x.ulf` for 2019 versions)
9. Copy the content of `Unity_v2018.x.ulf` license file to your CI's environment variable `UNITY_LICENSE_CONTENT`.
   _Note: if you are doing this on windows, chances are the [line endings will be wrong as explained here](https://gitlab.com/gableroux/unity3d-gitlab-ci-example/issues/5#note_95831816). Luckily for you, [`.gitlab-ci.yml`](.gitlab-ci.yml) solves this by removing `\r` character from the env variable so you'll be alright_
[`.gitlab-ci.yml`](.gitlab-ci.yml) will then place the `UNITY_LICENSE_CONTENT` to the right place before running tests or creating the builds.

### Unity Plus/Pro

1. Clone this project
2. Pull the docker image and run bash inside, passing unity username and password to env

    _hint: you should write this to a shell script and execute the shell script so you don't have your credentials stored in your bash history_. Also make sure you use your Unity3d _email address_ for `UNITY_USERNAME` env var.

    ```bash
    UNITY_VERSION=2019.3.7f1
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

### Unity license per target

Before `2018.4.8f1` for 2018 versions and before `2019.2.4f1` for 2019 versions, if you need a specific Unity license for a build target, you can add environment var `UNITY_LICENSE_CONTENT_{BUILD_TARGET}`. (`UNITY_LICENSE_CONTENT_ANDROID`, `UNITY_LICENSE_CONTENT_IOS`, ...). _This is not required anymore now that images share a base image [See related change](https://gitlab.com/gableroux/unity3d/merge_requests/63)**

#### Note about components in recent images

Starting from these versions, base image doesn't include windows, mac and webgl components anymore. This means you must use `-mac`, `-windows` or `-webgl` images. [See related change](https://gitlab.com/gableroux/unity3d/merge_requests/63)

### Travis

Travis doesn't support multiple-lines env variable out of the box and I had troubles with escaping so I recommend encrypting the license file. `.travis.yml` will decrypt the file and add its content to `UNITY_LICENSE_CONTENT` env var itself afterward.

```bash
travis encrypt-file --pro -r YOUR_TRAVIS_USERNAME/YOUR_TRAVIS_REPO_NAME ./Unity_v2018.x.ulf # TODO confirm new file name for 2019
```

For the record, the message I was getting:

> The previous command failed, possibly due to a malformed secure environment variable.
>  Please be sure to escape special characters such as ' ' and '$'.
>  For more information, see https://docs.travis-ci.com/user/encryption-keys.

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

#### Setup (only one time per mac)

Install the latest Xcode command line tools :

```bash
xcode-select --install
```

Install fastlane using:

```bash
# Using RubyGems
sudo gem install fastlane -NV

# Alternatively using Homebrew
brew install fastlane
```

#### Unity Settings

1. Switch target to iOS
1. In `PlayerSettings -> Other Settings`
    1. Fill the field `Signing Team ID` 
    1. Ensure `Automatically Sign` is unchecked
    1. iOS Provisioning Profile  
        1. `ProfileID`: `match AppStore your_bundle_identifier` _Replace `your_bundle_identifier` by yours_
        1. `ProfileType`: `Distribution`

#### XCode project

Make a first iOS build using your mac from Unity, that will create an xcode project.  
Ensure your target the same path than the ci.  
Ex: if you let `BUILD_NAME: ExampleProjectName` in `.gitlab-ci.yml`, your xcode project must be at the root of the following path: `.\Builds\iOS\ExampleProjectName\`

#### App on portail

Make sure that you have setup your app on the Apple Developer Portal and the App Store Connect or use [fastlane produce](https://docs.fastlane.tools/actions/produce/) to create it.

#### Fastlane initialization

Open the terminal at the same path then run `fastlane init`, follow instructions to generate Appfile and default Fastfile.

#### Provisioning profile

Run `fastlane match init`, follow instructions, select `appstore` provisioning profile type. ([Documentation](https://docs.fastlane.tools/actions/match/))

#### Make lane

Copy the following instructions on your `fastlane/Fastfile`:

```ruby
default_platform(:ios)

platform :ios do
  desc "Push a new beta build to TestFlight"
  lane :beta do
    sync_code_signing(type:"appstore", readonly: is_ci)
    increment_build_number({
        build_number: latest_testflight_build_number + 1
    })
    build_app(scheme:"Unity-iPhone")
    upload_to_testflight(groups:["Team"]) 
  end
end
```

Note about `upload_to_testflight`: Change "Team" to your internal tester or remove `(groups:["Team"])` if you want set manually who can test the build  

##### Related documentation

* [sync_code_signing (alias for match)](https://docs.fastlane.tools/actions/sync_code_signing/)
* [increment_build_number](https://docs.fastlane.tools/actions/increment_build_number/)
* [build_app (alias for gym)](https://docs.fastlane.tools/actions/build_app/)
* [upload_to_testflight (alias for pilot)](https://docs.fastlane.tools/actions/testflight/)


#### Run tests locally

Run the following command to test the build and the deployement localy:

```bash
fastlane ios beta
```

If the build and upload are ok, you have to force add some file to your git using command below   

```bash
git add -f pathToTheFileToAdd
```

you have to add the following files:

* `Gemfile`
* `Gemfile.lock` (if here)
* `fastlane/Appfile`
* `fastlane/Fastfile`
* `fastlane/Matchfile`

#### Gitlab-runner - register your mac

To automate your build with gitlab, you need to setup your mac as a gitlab runner.  
Installation:

```bash
sudo curl --output /usr/local/bin/gitlab-runner https://gitlab-runner-downloads.s3.amazonaws.com/latest/binaries/gitlab-runner-darwin-amd64
```

Give permission to execute : 

```bash
sudo chmod +x /usr/local/bin/gitlab-runner
```

* [Source (if you would like to check)](https://docs.gitlab.com/runner/install/osx.html)

Go to your project gitlab page, then go to `settings` -> `CI/CD` -> `Runners` -> `Specitic Runners` -> `Set up a specific Runner manually` -> take note of the token 

[Follow these instructions](https://docs.gitlab.com/runner/register/index.html) to register your mac as a gitlab-runner for your specific project.  
Follow **macOS** instructions **without** sudo command for registration.

* Tags: set `mac,ios`
* Executor: set `shell`

Then, to install/launch the runner:

```bash
cd ~ 
gitlab-runner install
gitlab-runner start
```

Runner is installed and will be run after a system reboot.

Now, you can uncomment the job `build-and-deploy-ios` in `.gitlab-ci.yml` to make the app build and deployement work.

### Android support

To make build working with Android, you will need a specific Unity license (because that is not the same docker image).  
Add the content of your specific Unity license in your CI's environment variable : `UNITY_LICENSE_CONTENT_ANDROID`

By default the apk is not signed and the build will use the Unity's default debug key.  
For security reason, you must not add your keystore to git.  
Encode your keystore file as base64 using openssl:  
`openssl base64 -A -in yourKeystore.keystore`

Copy the result to your CI's environment variable `ANDROID_KEYSTORE_BASE64`

Add following environment variables:

* `KEYSTORE_PASS`: Keystore pass  
* `KEY_ALIAS_NAME`: Keystore alias name to use (if not set, the program will use the alias name set in Project's PlayerSettings)  
* `KEY_ALIAS_PASS`: Keystore alias pass to use

Note about _keystore security_, if you would like to use another solution for storage, see [HERE](https://android.jlelse.eu/where-to-store-android-keystore-file-in-ci-cd-cycle-2365f4e02e57).

#### Android app bundle

`BUILD_APP_BUNDLE` env var is defined in `gitlab-ci.yml`. Set it to `true` to build an `.aab` file.  
Note: to build an android app bundle, you need an image with **Android NDK**. See [related issue gableroux/unity3d#61](https://gitlab.com/gableroux/unity3d/issues/61)

#### Bundle version code

The bundle version code must be increment for each deployed build.  
To simplify the process, the `BUNDLE_VERSION_CODE` env var is used and set as bundle version code.  
Currently, for gitlab, `BUNDLE_VERSION_CODE = $CI_PIPELINE_IID`. [Documentation](https://docs.gitlab.com/ee/ci/variables/predefined_variables.html)  
If you use another CI solution, set a CI env var incrementing for each pipeline.  

#### Fastlane supply (deployement)

Follow [setup instructions](https://docs.fastlane.tools/actions/supply/) to get a google play console token, then, add the content to env var `GPC_TOKEN`.  
Uncomment the `#deploy-android` job in gitlab-ci.yml and replace `com.youcompany.yourgame` by your package name.  
You can change the track `internal` to `alpha`, `beta` or `production`.  

That is the simplest way with command line but you can also make `fastlane/Fastfile` and `fastlane/Appfile`, with the following command after building a temporary gradle project (export gradle project option in Unity build settings):

```bash
fastlane init
```

Then run the following command:

```bash
fastlane supply init
```

and update all metadata, images, changelogs, etc... These will be uploaded to the store everytime. Refer to [fastlane supply documentation](https://docs.fastlane.tools/actions/supply/) for more details.

## How to run scripts manually

You can execute the local scripts and specify the path of your Unity executable using `UNITY_EXECUTABLE`. You may try this in your project before you setup the whole CI so you confirm it works with your current unity version :+1:

### Test

```bash
UNITY_EXECUTABLE="/Applications/Unity/Hub/Editor/2019.3.7f1/Unity.app/Contents/MacOS/Unity" \
  ./local_test.sh
```

### Build

```bash
UNITY_EXECUTABLE="/Applications/Unity/Hub/Editor/2019.3.7f1/Unity.app/Contents/MacOS/Unity" \
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

