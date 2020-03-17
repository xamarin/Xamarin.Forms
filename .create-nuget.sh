 #!/bin/bash 

# This is not our official nuget build script.
# This is used as a quick and dirty way create nuget packages used to test user issue reproductions.
# This is updated as XF developers use it to test reproductions. As such, it may not always work.
# This is not ideal, but it's better than nothing, and it usually works fine.

CONSOLE_PREFIX="-- SCRIPT MESSAGE:"
CONFIG=Debug
NUGET_EXE=nuget.exe

# Download NuGet if it does not exist.
if [ ! -f "$NUGET_EXE" ]; then
    echo "${CONSOLE_PREFIX} Downloading NuGet..."
    curl -Lsfo "$NUGET_EXE" https://dist.nuget.org/win-x86-commandline/latest/nuget.exe
    if [ $? -ne 0 ]; then
        echo "${CONSOLE_PREFIX} An error occurred while downloading nuget.exe."
        exit 1
    fi
fi

case $1 in
  "droid")
    sh .create-stubs.sh $CONFIG
    msbuild /t:restore restore .xamarin.forms.android.nuget.sln
    msbuild /v:m /p:platform="any cpu" /p:WarningLevel=0 /p:CreateAllAndroidTargets=true .xamarin.forms.android.nuget.sln
    msbuild /v:m /p:platform="any cpu" /p:WarningLevel=0 /p:CreateAllAndroidTargets=true .xamarin.forms.android.nuget.sln /p:AndroidTargetFrameworkVersion=v8.1 /t:Restore
    msbuild /v:m /p:platform="any cpu" /p:WarningLevel=0 /p:CreateAllAndroidTargets=true .xamarin.forms.android.nuget.sln /p:AndroidTargetFrameworkVersion=v8.1
    ;;
  "rdroid")
    CONFIG=Release
    msbuild /t:restore .xamarin.forms.android.nuget.sln
    msbuild /v:m /p:configuration=release /p:platform="any cpu" /p:WarningLevel=0 .xamarin.forms.android.nuget.sln
    msbuild /v:m /p:configuration=release /p:platform="any cpu" /p:WarningLevel=0 .xamarin.forms.android.nuget.sln /p:AndroidTargetFrameworkVersion=v8.1 /t:Restore
    msbuild /v:m /p:configuration=release /p:platform="any cpu" /p:WarningLevel=0 .xamarin.forms.android.nuget.sln /p:AndroidTargetFrameworkVersion=v8.1
    ;;
  "adroid")
    ./create-nuget.sh droid
    ./create-nuget.sh rdroid
    exit 1
    ;;
  "pdroid")
    CONFIG=Release
    msbuild /v:m /p:configuration=release /p:platform="anyCpu" /p:WarningLevel=0 Xamarin.Forms.Platform.Android/Xamarin.Forms.Platform.Android.csproj
    msbuild /v:m /p:configuration=release /p:platform="anyCpu" /p:WarningLevel=0 Xamarin.Forms.Platform.Android/Xamarin.Forms.Platform.Android.csproj /p:AndroidTargetFrameworkVersion=v8.1 /t:Restore
    msbuild /v:m /p:configuration=release /p:platform="anyCpu" /p:WarningLevel=0 Xamarin.Forms.Platform.Android/Xamarin.Forms.Platform.Android.csproj /p:AndroidTargetFrameworkVersion=v8.1
    ;;
  "pddroid")
    CONFIG=Debug
    msbuild /v:m /p:configuration=debug /p:platform="anyCpu" /p:WarningLevel=0 Xamarin.Forms.Platform.Android/Xamarin.Forms.Platform.Android.csproj
    msbuild /v:m /p:configuration=debug /p:platform="anyCpu" /p:WarningLevel=0 Xamarin.Forms.Platform.Android/Xamarin.Forms.Platform.Android.csproj /p:AndroidTargetFrameworkVersion=v8.1 /t:Restore
    msbuild /v:m /p:configuration=debug /p:platform="anyCpu" /p:WarningLevel=0 Xamarin.Forms.Platform.Android/Xamarin.Forms.Platform.Android.csproj /p:AndroidTargetFrameworkVersion=v8.1
    ;;
  "ios")
    CONFIG=Debug
    sh .create-stubs.sh $CONFIG
    msbuild /t:restore .xamarin.forms.ios.nuget.sln
    msbuild /v:m /p:platform="any cpu" .xamarin.forms.ios.nuget.sln
    ;;
  "droidios")
    CONFIG=Debug
    sh .create-stubs.sh $CONFIG
    msbuild /t:restore .xamarin.forms.android.nuget.sln
    msbuild /t:restore .xamarin.forms.ios.nuget.sln
    msbuild /v:m /p:platform="any cpu" /p:WarningLevel=0 .xamarin.forms.android.nuget.sln
    msbuild /v:m /p:platform="any cpu" .xamarin.forms.ios.nuget.sln
    ;;
  "all")
    CONFIG=Debug
    sh .create-stubs.sh $CONFIG
    msbuild /t:restore .xamarin.forms.nuget.sln
    msbuild /v:m /p:platform="any cpu" /p:WarningLevel=0 .xamarin.forms.nuget.sln
    ;;
  "rall")
    CONFIG=Release
    sh .create-stubs.sh $CONFIG
    msbuild /t:restore .xamarin.forms.nuget.sln
    msbuild /v:m /p:platform="any cpu" /p:WarningLevel=0 .xamarin.forms.nuget.sln /p:configuration=release
    msbuild /v:m /p:platform="any cpu" /p:WarningLevel=0 .xamarin.forms.nuget.sln /p:configuration=release /p:AndroidTargetFrameworkVersion=v8.1 /t:Restore
    msbuild /v:m /p:platform="any cpu" /p:WarningLevel=0 .xamarin.forms.nuget.sln /p:configuration=release /p:AndroidTargetFrameworkVersion=v8.1
    ;;
  *)
    CONFIG=Debug
    sh .create-stubs.sh $CONFIG
    msbuild /t:restore .xamarin.forms.nuget.sln
    msbuild /v:m /p:platform="any cpu" /p:WarningLevel=0 .xamarin.forms.nuget.sln
    ;;
esac

DEBUG_VERSION=0
mono $NUGET_EXE pack .nuspec/Xamarin.Forms.nuspec -properties configuration=$CONFIG\;platform=anycpu -Version 9.9.$DEBUG_VERSION
if [ ! -n $CREATE_MAP_NUGET ]; then
  # Requires building x86, x64, AMD
  mono $NUGET_EXE pack .nuspec/Xamarin.Forms.Maps.nuspec -properties configuration=$CONFIG\;platform=anycpu -Version 9.9.$DEBUG_VERSION
fi