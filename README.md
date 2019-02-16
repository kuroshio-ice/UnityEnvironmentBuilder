# UnityEnvironmentBuilder

このプロジェクトは
develop環境, staging環境など
ビルドの環境分けを行うエディタ拡張です。

This project is
develop environment, staging environment, etc.
It is an editor extension that separates the build environment.

## Setup.

```
Assets/Editor/EnvironmentBuilder
```

配下をインポートします。

I will import subordinates.

## Json Setting.

develop.json
```
{
    "PlatformOptions": [   # <- platform build setting.
        {
            "Platform": "iOS",
            "ProductName": "AppName",
            "ApplicationIdentifier": "com.appname"
        },
        {
            "Platform": "Android",
            "ProductName": "AppName",
            "ApplicationIdentifier": "com.appname"
        }
    ],
    "DefineSymbols": [    # <- build preprocessor.
        "SERVER_DEVELOP",
        "DEBUG_LOG",
        "TUTORIAL_SKIP"
    ],
    "BuildOptions": [     # <- valid BuildOptions.
        "Development"
    ]
}
```

他の環境を追加する場合は
別名のjsonを追加してください。

When adding other environment
Please add alias json.

## BUILD

```
$unityPath ¥
 -quit ¥
 -batchmode ¥
 -buildTarget {platform name} ¥
 -projectPath $projectPath ¥
 -executeMethod AppBuilder.Perform ¥
 -path {output file name} ¥
 -env {json file name (no extension)}
```

