# Capibara

[![Build status](https://ci.appveyor.com/api/projects/status/m7us4abx4c53mwb5/branch/develop?svg=true)](https://ci.appveyor.com/project/cheesecomer/capibara-app/branch/develop)
[![codecov](https://codecov.io/gh/cheesecomer/capibara-app/branch/develop/graph/badge.svg)](https://codecov.io/gh/cheesecomer/capibara-app)

## プロジェクト構成


## Topic

`git update-index --skip-worktree Capibara.Droid/Resources/Resource.designer.cs`

## sharpie
```
mkdir PhotoTweaks
cd PhotoTweaks
sharpie pod init ios PhotoTweaks
sharpie pod bind
cd Pods
xcodebuild -project Pods.xcodeproj -target PhotoTweaks -sdk iphonesimulator11.2 -configuration Release build
cd ..
lipo -create Binding/PhotoTweaks.framework/PhotoTweaks build/Release-iphonesimulator/PhotoTweaks/PhotoTweaks.framework/PhotoTweaks -output PhotoTweaks
mv PhotoTweaks Binding/PhotoTweaks.framework/PhotoTweaks
```