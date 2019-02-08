# unity3d ci example

[![CircleCI](https://circleci.com/gh/MizoTake/unity3d-ci-example.svg?style=svg)](https://circleci.com/gh/MizoTake/unity3d-ci-example)

[GabLeRoux/unity3d-ci-example](https://github.com/GabLeRoux/unity3d-ci-example)
をforkしています。CircleCIでUnityのTestとBuildをするまでの流れをまとめています。

今回参考にしたRepositoryは２つです
- [GabLeRoux/unity3d-ci-example](https://github.com/GabLeRoux/unity3d-ci-example)
- [wtanaka/docker-unity3d](https://github.com/wtanaka/docker-unity3d)

## セットアップ

CircleCIやDockerの登録やらなんやらの説明は省きます。
https://circleci.com/
基本的には公式サイトに行ってGitHubやBitBucketの連携登録をすれば簡単にできるかと思います。

https://www.docker.com/
MacやLinuxの方はすんなりセットアップできると思います、Windowsの方はセットアップして起動できてなさそうであればhyper-vという項目をONにしないといけないので[こちら](https://docs.microsoft.com/ja-jp/virtualization/hyper-v-on-windows/quick-start/enable-hyper-v)を参考にしてみてください。

unity3d-ci-exampleで最初わからなかったのがREADMEにある`How to activate`という項目…
要するにUnityのLicense認証なのですが少し手間でした。

またこの段階からDockerが必要となります。Docker初心者ですが調べつつ何とかなったのでコマンドも覚えているものは残しておきます。
今回使用するContainerは[gableroux/unity3d](https://hub.docker.com/r/gableroux/unity3d/)です。

DockerHubからimageを落として来ないといけないので落としてきます

```sh
docker pull gableroux/unity3d
```

落とせたかどうかは

```sh
docker images
```

で確認できます。

落とせていたら

```sh
UNITY_VERSION=2018.2.3f1
docker run -it --rm \
-e "UNITY_USERNAME=username@example.com" \
-e "UNITY_PASSWORD=example_password" \
-e "TEST_PLATFORM=linux" \
-e "WORKDIR=/root/project" \
-v "$(pwd):/root/project" \
gableroux/unity3d:$UNITY_VERSION \
bash
```

UNITY_VERSIONやUNITY_USERNAME、UNITY_PASSWORDは適宜変えましょう。
何をしているかというとgableroux/unity3dを起動させるときに環境変数として値を渡しているみたいです。
コマンドの処理が終わるとDockerが起動してDocker上のbashが操作できるようになります。
一回Dockerから出たい時は

```sh
exit
```

だけ打てば出れます。再度入る時は入った時と同じコマンドでいいと思います。
次にDocker上で

```sh
xvfb-run --auto-servernum --server-args='-screen 0 640x480x24' \
/opt/Unity/Editor/Unity \
-logFile \
-batchmode \
-username "$UNITY_USERNAME" -password "$UNITY_PASSWORD"
```

Unityの起動を行いLicense認証をしようと動きます。
うまくいくと

```sh
LICENSE SYSTEM [2017723 8:6:38] Posting <?xml version="1.0" encoding="UTF-8"?><root><SystemInfo><IsoCode>en</IsoCode><UserName>[...]
```

というxmlがログに現れるので`unity3d.alf`という名前でホストのOSに保存しましょう。
次に https://license.unity3d.com/manual をホストのOSで開いて`unity3d.alf`を選択して送信します。
すると`Unity_v2018.x.ulf`というファイルがダウンロードされます。
`How to activate`的には、ここまでで区切りですがCircleCIの場合はもう少しセットアップが必要です。

`Unity_v2018.x.ulf`というのがUnityのLicenseあるよ！という証明するファイルになるのですが、そのままgit管理してしまうのは危険かもしれません。PlusライセンスやProライセンスだと他の人がアクティベートできてしまうからです。(たぶん)
そこで自分でKEYを決めて暗号化をかけてCircleCI上で復号化して使おうじゃないか！という流れです。

次もホストOSでの処理になります。

```sh
export KEY=insert-your-strong-generated-key-here
openssl aes-256-cbc -e -in ci/Unity_v2018.x.ulf -out ci/Unity_v2018.x.ulf-cipher -k $KEY
git add ci/Unity_v2018.x.ulf-cipher
git commit -m "Add encrypted Unity_v2018.x.ulf using openssl aes-256-cbc KEY"
```

`insert-your-strong-generated-key-here`を好きな文字列に置き換えて覚えておいてください。後々CircleCIの環境変数として保存します。
そして暗号化されたファイルが`Unity_v2018.x.ulf-cipher`となります。間違っても`Unity_v2018.x.ulf`を追加しないように気をつけてください。
そのためリポジトリとは別のディレクトリで作業して`Unity_v2018.x.ulf-cipher`のみリポジトリに追加する方が安全かもしれません。

### CircleCIに環境変数を登録する
先ほど暗号化に使ったKEYをCircleCIに登録します。
CircleCIとリポジトリを連携するとリポジトリに対して設定が行えるようになります。設定画面にある
<img width="657" alt="名称未設定.png" src="https://qiita-image-store.s3.amazonaws.com/0/127761/732bf623-c40f-fbf0-fbbf-cfe3f5c8170c.png">

`Enviroment Variables`の項目があり選択すると画像にはないですが右上に`Add Variables`というボタンがあるのでKEYという名前で環境変数を登録しましょう。

セットアップは終わりです。

## config.ymlを書くぞ
CicleCIで処理して欲しい項目をyml形式で書くことになります。
保存場所はリポジトリのルートに`.circlci`フォルダを作って配下に`config.yml`を追加してください。
今回使用したのは以下のymlです。(unity3d-ci-sampleのものをベースにしました)

```yml:config.yml
version: 2
references:
  docker_image: &docker_image
    docker:
      - image: gableroux/unity3d:2018.2.6f1
  setup_unity_license_env_var: &setup_unity_license_env_var
    command: |
      mkdir -p /root/.cache/unity3d
      mkdir -p /root/.local/share/unity3d/Unity/
      openssl version
      openssl aes-256-cbc -md md5 -d -in ./ci/Unity_v2018.ulf-cipher -out /Unity_v2018.ulf -k $KEY
      export UNITY_LICENSE_CONTENT=`cat /Unity_v2018.ulf`
      echo "$UNITY_LICENSE_CONTENT" | tr -d '\r' > "/root/.local/share/unity3d/Unity/Unity_lic.ulf"
  remove_license_file: &remove_license_file
    command: |
      rm /Unity_v2018.ulf
      rm /root/.local/share/unity3d/Unity/Unity_lic.ulf
jobs:
  test_editmode:
    <<: *docker_image
    steps:
      # TODO: Add git to unity image so this is not required anymore
      # this will prevent following error on 'checkout' step:
      # Either git or ssh (required by git to clone through SSH) is not installed in the image. Falling back to CircleCI's native git client but the behavior may be different from official git. If this is an issue, please use an image that has official git and ssh installed.
      - run:
          command: apt-get update && apt-get install -y git && git --version
      - checkout
      - run:
          <<: *setup_unity_license_env_var
      - run:
          environment:
            TEST_PLATFORM: editmode
          command: |
            chmod -R 755 ./ci/test.sh
            ./ci/test.sh
      - run:
          <<: *remove_license_file
      - store_artifacts:
          path: '$(pwd)/$TEST_PLATFORM-results.xml'
          destination: '$TEST_PLATFORM-results.xml'
  test_playmode:
    <<: *docker_image
    steps:
      - run:
          command: apt-get update && apt-get install -y git && git --version
      - checkout
      - run:
          <<: *setup_unity_license_env_var
      - run:
          environment:
            TEST_PLATFORM: playmode
          command: |
            chmod -R 755 ./ci/test.sh
            ./ci/test.sh
      - run:
          <<: *remove_license_file
      - store_artifacts:
          path: '$(pwd)/$TEST_PLATFORM-results.xml'
          destination: '$TEST_PLATFORM-results.xml'
  build_StandaloneWindows:
    <<: *docker_image
    steps:
      - run:
          command: |
            apt-get update && apt-get install -y git zip && git --version
      - checkout
      - run:
          <<: *setup_unity_license_env_var
      - run:
          environment:
            BUILD_TARGET: StandaloneWindows
          command: |
            chmod -R 755 ./ci/build.sh
            ./ci/build.sh
      - run:
          <<: *remove_license_file
      - store_artifacts:
          path: './Builds/'
  build_StandaloneOSX:
    <<: *docker_image
    steps:
      - run:
          command: |
            apt-get update && apt-get install -y git zip && git --version
      - checkout
      - run:
          <<: *setup_unity_license_env_var
      - run:
          environment:
            BUILD_TARGET: StandaloneOSX
          command: |
            chmod -R 755 ./ci/build.sh
            ./ci/build.sh
      - run:
          <<: *remove_license_file
      - store_artifacts:
          path: './Builds/'
workflows:
  version: 2
  test_and_build:
    jobs:
      - build_StandaloneWindows
      - build_StandaloneOSX
      - test_editmode

```

これが
![コメント 2019-02-07 233248.jpg](https://qiita-image-store.s3.amazonaws.com/0/127761/a65c9b56-8a46-6fe3-543b-7e79d2f2f8bd.jpeg)
こうなります。

少し分解して解説を入れます。
`setup_unity_license_env_var`だけを見ます。

```sh
# なんでこれ追加したんだったか…無くて怒られたような…
mkdir -p /root/.cache/unity3d
mkdir -p /root/.local/share/unity3d/Unity/
openssl version
# 暗号化したファイルを復号化する処理
openssl aes-256-cbc -md md5 -d -in ./ci/Unity_v2018.ulf-cipher -out /Unity_v2018.ulf -k $KEY
export UNITY_LICENSE_CONTENT=`cat /Unity_v2018.ulf`
# 復号化した中身をファイルとしてUnity配下に保存
echo "$UNITY_LICENSE_CONTENT" | tr -d '\r' > "/root/.local/share/unity3d/Unity/Unity_lic.ulf"
```

復号化した中身をファイルとしてUnity配下に保存をしないと自分の場合動きませんでした。`unity3d-ci-example`にはなかったのですが`wtanaka/docker-unity3d`にはその記述があり書いてみたらライセンスが通りました。。。先駆者たちに感謝っ・・・・！圧倒的感謝っ・・・・！

あとは`unity3d-ci-example`のciフォルダ配下の.shに権限与えて動かしているだけです。
.shの中身はunityをコマンドで動かしているだけなので色々といじれるとは思います。
編集したり調べる際には[コマンド引数のリファレンス](https://docs.unity3d.com/ja/2018.1/Manual/CommandLineArguments.html)にだいたい乗っているので確認してみてください。

Testは基本的に成功知れていればいいと思いますがBuildは成果物がダウンロードできないと意味がないのでダウンロードする用意をします。
CircleCIには[Artifacts](https://circleci.com/docs/2.0/artifacts/)という機能がありArtifactsに保存したい成果物を登録しておけば長期的に保存ができるものになります。

`unity3d-ci-sample`もBuildフォルダというものを作っておりフォルダごと保存するようになっています。
jobの詳細画面？でArtifactsが確認できます。
![コメント 2019-02-07 231417.jpg](https://qiita-image-store.s3.amazonaws.com/0/127761/1dbe4e69-1fe2-ce99-c8f6-221ba091c3b7.jpeg)

フォルダを成果物としているのでかなりバラバラになっています。(-customBuildPathの環境変数登録してなくてそのままなのはスルー)
１つ１つダウンロードしてられないのでzipにまとめます。

```yml:config.yml
build_StandaloneWindows:
    <<: *docker_image
    steps:
      - run:
          command: |
            apt-get update && apt-get install -y git zip && git --version
      - checkout
      - run:
          <<: *setup_unity_license_env_var
      - run:
          environment:
            BUILD_TARGET: StandaloneWindows
          command: |
            chmod -R 755 ./ci/build.sh
            ./ci/build.sh
      - run:
          <<: *remove_license_file
      - store_artifacts:
          path: './Builds/'
```

一部抜粋ですが、apt-getでgitのついでにzipも一緒にインストールしておきます。最後の方にartifactsに登録しているのがわかると思います。
そしてbuild.shの一部を編集します。

```sh:build.sh
if [ $UNITY_EXIT_CODE -eq 0 ]; then
  echo "Run succeeded, no failures occurred";
  cd $BUILD_PATH
  zip archive -r .
  cd /root/project
elif [ $UNITY_EXIT_CODE -eq 2 ]; then
  echo "Run succeeded, some tests failed";
elif [ $UNITY_EXIT_CODE -eq 3 ]; then
  echo "Run failure (other failure)";
else
  echo "Unexpected exit code $UNITY_EXIT_CODE";
fi
```

成功したときにartifactsに登録するディレクトリに移動してzip処理をします。そして後処理があるのでディレクトリをもとの位置に戻ります。
上記の編集を加えると

![コメント 2019-02-07 231229.jpg](https://qiita-image-store.s3.amazonaws.com/0/127761/b8549553-c3fa-a12a-4763-4ab8ee72aa01.jpeg)

archive.zipになっているのがわかります。archive.zipを落として解答すればビルド結果が確認できるかと思います。
