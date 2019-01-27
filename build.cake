// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


// examples
/*

./build.ps1 -Target NugetPack -ScriptArgs '-version="9.9.9-custom"'
./build.sh --target NugetPack --version="9.9.9-custom"



 */
//////////////////////////////////////////////////////////////////////
// ADDINS
//////////////////////////////////////////////////////////////////////
#addin "nuget:?package=Cake.Xamarin&version=3.0.0"
#addin "nuget:?package=Cake.Android.Adb&version=3.0.0"
#addin "nuget:?package=Cake.Git&version=0.19.0"

//////////////////////////////////////////////////////////////////////
// TOOLS
//////////////////////////////////////////////////////////////////////
#tool nuget:?package=NUnit.ConsoleRunner&version=3.4.0

//////////////////////////////////////////////////////////////////////
// ARGUMENTS
//////////////////////////////////////////////////////////////////////

var target = Argument("target", "Default");
var configuration = Argument("configuration", "Debug");
var version = Argument("version", "9.9.9-beta");


//////////////////////////////////////////////////////////////////////
// TASKS
//////////////////////////////////////////////////////////////////////

Task("Clean")
    .Does(() =>
{
    CleanDirectories("./**/obj", (fsi)=> !fsi.Path.FullPath.Contains("XFCorePostProcessor"));
    CleanDirectories("./**/bin", (fsi)=> !fsi.Path.FullPath.Contains("XFCorePostProcessor"));
    Information(MakeAbsolute(Directory("./")));

    // this doesn't work
    GitClean(MakeAbsolute(Directory("./")));
});

Task("NuGetPack")
    .IsDependentOn("Build")
    .IsDependentOn("Android81")
    .IsDependentOn("_NuGetPack");


Task("_NuGetPack")
    .Does(() =>
    {
        var nugetPackageDir = Directory("./artifacts");
        var nuGetPackSettings = new NuGetPackSettings
        {   
            OutputDirectory = nugetPackageDir,
            Version = version
        };

        var nugetFilePaths = 
            GetFiles("./.nuspec/*.nuspec");

        nuGetPackSettings.Properties.Add("configuration", configuration);
        nuGetPackSettings.Properties.Add("platform", "anycpu");
        // nuGetPackSettings.Verbosity = NuGetVerbosity.Detailed;
        NuGetPack(nugetFilePaths, nuGetPackSettings);
    });

Task("BuildHack")
    .Does(() =>
    {
        if(!IsRunningOnWindows())
            MSBuild("./Xamarin.Forms.Build.Tasks/Xamarin.Forms.Build.Tasks.csproj", GetMSBuildSettings().WithRestore());
        
    });

Task("Build")
    .IsDependentOn("BuildHack")
    .Does(() =>
{ 
    try
    {
        MSBuild("./Xamarin.Forms.sln", GetMSBuildSettings()
                .WithTarget("restore"));
    }
    catch(Exception)
    {
        if(!IsRunningOnWindows())
            throw;
    }


    try{
        MSBuild("./Xamarin.Forms.sln", GetMSBuildSettings());
    }
    catch(Exception)
    {
        if(!IsRunningOnWindows())
            throw;
    }
});

Task("Android81")
    .IsDependentOn("BuildHack")
    .Does(() =>
    {
        string[] androidProjects = 
            new []
            {
                "./Xamarin.Forms.Platform.Android/Xamarin.Forms.Platform.Android.csproj",
                "./Xamarin.Forms.Platform.Android.AppLinks/Xamarin.Forms.Platform.Android.AppLinks.csproj",
                "./Xamarin.Forms.Maps.Android/Xamarin.Forms.Maps.Android.csproj",
                "./Stubs/Xamarin.Forms.Platform.Android/Xamarin.Forms.Platform.Android (Forwarders).csproj"
            };

        foreach(var project in androidProjects)
            MSBuild(project, 
                    GetMSBuildSettings()
                        .WithRestore()
                        .WithProperty("AndroidTargetFrameworkVersion", "v8.1"));
    });

Task("Deploy")
    .IsDependentOn("DeployiOS")
    .IsDependentOn("DeployAndroid")
    .Does(() =>
    { 
        // not sure how to get this to deploy to iOS
        BuildiOSIpa("./Xamarin.Forms.sln", platform:"iPhoneSimulator", configuration:"Debug");

    });

Task("VSMAC")
    .IsDependentOn("BuildHack")
    .Does(() =>
    {
        StartProcess("open", new ProcessSettings{ Arguments = "Xamarin.Forms.sln" });
        
    });

// TODO? Not sure how to make this work
Task("DeployiOS")
    .Does(() =>
    { 
        // not sure how to get this to deploy to iOS
        BuildiOSIpa("./Xamarin.Forms.sln", platform:"iPhoneSimulator", configuration:"Debug");

    });

Task("DeployAndroid")
    .Does(() =>
    { 
        BuildAndroidApk("./Xamarin.Forms.ControlGallery.Android/Xamarin.Forms.ControlGallery.Android.csproj", sign:true, configuration:"Debug");
        AdbUninstall("AndroidControlGallery.AndroidControlGallery");
        AdbInstall("./Xamarin.Forms.ControlGallery.Android/bin/Debug/AndroidControlGallery.AndroidControlGallery-Signed.apk");

        // how to grab this dynamically?
        AmStartActivity("AndroidControlGallery.AndroidControlGallery/md546303760447087909496d02dc7b17ae8.Activity1");

    });

//////////////////////////////////////////////////////////////////////
// TASK TARGETS
//////////////////////////////////////////////////////////////////////

Task("Default")
    .IsDependentOn("Build")
    ;

//////////////////////////////////////////////////////////////////////
// EXECUTION
//////////////////////////////////////////////////////////////////////

RunTarget(target);


MSBuildSettings GetMSBuildSettings()
{
    var msbuildSettings =  new MSBuildSettings();

    msbuildSettings.ToolVersion = MSBuildToolVersion.VS2017;
    msbuildSettings.PlatformTarget = PlatformTarget.MSIL;
    msbuildSettings.MSBuildPlatform = (Cake.Common.Tools.MSBuild.MSBuildPlatform)1;
    msbuildSettings.Configuration = configuration;
    return msbuildSettings;

}