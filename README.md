# unity3d gitlab-ci example

[![pipeline status](https://gitlab.com/gableroux/unity3d-gitlab-ci-example/badges/master/pipeline.svg)](https://gitlab.com/gableroux/unity3d-gitlab-ci-example/commits/master)

This project is a PoC to run unity3d tests and builds inside gitlab-ci using [gableroux/unity3d docker image](https://hub.docker.com/r/gableroux/unity3d/).

## Point of interest

### gitlab-ci

This is probably what you're looking for, have a look to [`.gitlab-ci.yml`](.gitlab-ci.yml)

### build script

File passed to the unity3d command line as argument to create builds

todo

### test files

Very basic `editmode` and `playmode` tests (all passing) can be found in [Assets/Scripts/Editor/EditModeTests](Assets/Scripts/Editor/EditModeTests) and [Assets/Scripts/Editor/PlayModeTests](Assets/Scripts/Editor/PlayModeTests)

### Execute the tests

```bash
path/to/unity -runTests -projectPath $(pwd) -testResults $(pwd)/results.xml -testPlatform editmode
path/to/unity -runTests -projectPath $(pwd) -testResults $(pwd)/results.xml -testPlatform playmode
```

## License

[MIT](LICENSE.md) © [Gabriel Le Breton](https://gableroux.com)
