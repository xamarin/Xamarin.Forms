 #!/bin/bash 

# This is not our official nuget build script.
# This is used as a quick and dirty way create nuget packages used to test user issue reproductions.
# This is updated as XF developers use it to test reproductions. As such, it may not always work.
# This is not ideal, but it's better than nothing, and it usually works fine.

CONSOLE_PREFIX="-- SCRIPT MESSAGE:"
CONFIG=Debug
NUGET_EXE=caketools/nuget.exe

# Check if Homebrew is installed
if [[ $(command -v brew) == "" ]]; then
    echo "${CONSOLE_PREFIX} Installing Homebrew"
    /usr/bin/ruby -e "$(curl -fsSL https://raw.githubusercontent.com/Homebrew/install/master/install)"
else
    echo "${CONSOLE_PREFIX} Updating Homebrew"
    brew update
fi

# Check if Wine is installed
if brew cask ls --versions wine-stable > /dev/null; then
  echo "${CONSOLE_PREFIX} Wine package is installed."
else
  echo "${CONSOLE_PREFIX} Wine package is not installed."
  echo "${CONSOLE_PREFIX} In order to install Wine we're also need to install Xquartz."
  echo "${CONSOLE_PREFIX} The installing proccess may require to ask your password"
  brew cask install xquartz
  brew cask install wine-stable
fi

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
    wine $NUGET_EXE restore .xamarin.forms.android.nuget.sln
    msbuild /v:m /p:platform="any cpu" /p:WarningLevel=0 /p:CreateAllAndroidTargets=true .xamarin.forms.android.nuget.sln
    msbuild /v:m /p:platform="any cpu" /p:WarningLevel=0 /p:CreateAllAndroidTargets=true .xamarin.forms.android.nuget.sln /p:AndroidTargetFrameworkVersion=v8.1 /t:Restore
    msbuild /v:m /p:platform="any cpu" /p:WarningLevel=0 /p:CreateAllAndroidTargets=true .xamarin.forms.android.nuget.sln /p:AndroidTargetFrameworkVersion=v8.1
    ;;
  "rdroid")
    CONFIG=Release
    wine $NUGET_EXE restore .xamarin.forms.android.nuget.sln
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
    wine $NUGET_EXE restore .xamarin.forms.ios.nuget.sln
    msbuild /v:m /p:platform="any cpu" .xamarin.forms.ios.nuget.sln
    ;;
  "droidios")
    CONFIG=Debug
    sh .create-stubs.sh $CONFIG
    wine $NUGET_EXE restore .xamarin.forms.android.nuget.sln
    wine $NUGET_EXE restore .xamarin.forms.ios.nuget.sln
    msbuild /v:m /p:platform="any cpu" /p:WarningLevel=0 .xamarin.forms.android.nuget.sln
    msbuild /v:m /p:platform="any cpu" .xamarin.forms.ios.nuget.sln
    ;;
  "all")
    CONFIG=Debug
    sh .create-stubs.sh $CONFIG
    wine $NUGET_EXE restore .xamarin.forms.nuget.sln
    msbuild /v:m /p:platform="any cpu" /p:WarningLevel=0 .xamarin.forms.nuget.sln
    ;;
  "rall")
    CONFIG=Release
    sh .create-stubs.sh $CONFIG
    wine $NUGET_EXE restore .xamarin.forms.nuget.sln
    msbuild /v:m /p:platform="any cpu" /p:WarningLevel=0 .xamarin.forms.nuget.sln /p:configuration=release
    msbuild /v:m /p:platform="any cpu" /p:WarningLevel=0 .xamarin.forms.nuget.sln /p:configuration=release /p:AndroidTargetFrameworkVersion=v8.1 /t:Restore
    msbuild /v:m /p:platform="any cpu" /p:WarningLevel=0 .xamarin.forms.nuget.sln /p:configuration=release /p:AndroidTargetFrameworkVersion=v8.1
    ;;
  *)
    CONFIG=Debug
    sh .create-stubs.sh $CONFIG
    wine $NUGET_EXE restore .xamarin.forms.nuget.sln
    msbuild /v:m /p:platform="any cpu" /p:WarningLevel=0 .xamarin.forms.nuget.sln
    ;;
esac

DEBUG_VERSION=0
wine $NUGET_EXE pack .nuspec/Xamarin.Forms.nuspec -properties configuration=$CONFIG\;platform=anycpu -Version 9.9.$DEBUG_VERSION
if [ ! -n $CREATE_MAP_NUGET ]; then
  # Requires building x86, x64, AMD
  wine $NUGET_EXE pack .nuspec/Xamarin.Forms.Maps.nuspec -properties configuration=$CONFIG\;platform=anycpu -Version 9.9.$DEBUG_VERSION
fi