if (IsMac)
{
	Item (XreItem.Xcode_10_3_0).XcodeSelect ();
	Item ("https://download.visualstudio.microsoft.com/download/pr/edd7782e-dc4f-4be5-9b55-8a51eeb7a718/ed8c238ec095b5d1051e2539cea38113/xamarin.android-10.0.0.40.pkg");
	Item ("https://download.mono-project.com/archive/6.4.0/macos-10-universal/MonoFramework-MDK-6.4.0.189.macos10.xamarin.universal.pkg");
	Item ("https://bosstoragemirror.blob.core.windows.net/wrench/jenkins/xcode11/dc5fc120e090317bbd78049f25dacd84e4f3dc48/254/package/xamarin.ios-12.99.4.5.pkg");
	Item ("https://bosstoragemirror.blob.core.windows.net/wrench/jenkins/xcode11/dc5fc120e090317bbd78049f25dacd84e4f3dc48/254/package/xamarin.mac-5.99.4.5.pkg");
	ForceJavaCleanup();

    var dotnetVersion = System.Environment.GetEnvironmentVariable("DOTNET_VERSION");
    if (!string.IsNullOrEmpty(dotnetVersion))
        DotNetCoreSdk(dotnetVersion);

  // VSTS installs into a non-default location. Let's hardcode it here because why not.
	var vstsBaseInstallPath = Path.Combine (Environment.GetEnvironmentVariable ("HOME"), ".dotnet", "sdk");
	var vstsInstallPath = Path.Combine (vstsBaseInstallPath, dotnetVersion);
	var defaultInstallLocation = Path.Combine ("/usr/local/share/dotnet/sdk/", dotnetVersion);
	if (Directory.Exists (vstsBaseInstallPath) && !Directory.Exists (vstsInstallPath))
		ln (defaultInstallLocation, vstsInstallPath);
}
else
{
	Item("https://download.visualstudio.microsoft.com/download/pr/68e5a599-5d9b-4d59-b41a-8348d23faa73/abd0a8f9a6b6253facfaaa97e5e138509b004263134d310cc6d49c2f1ac734b9/Xamarin.Android.Sdk-10.0.0.40.vsix");	
}

Item(XreItem.Java_OpenJDK_1_8_0_25);
AndroidSdk ().ApiLevel((AndroidApiLevel)29);

void ln (string source, string destination)
{
	Console.WriteLine ($"ln -sf {source} {destination}");
	if (!Config.DryRun)
		Exec ("/bin/ln", "-sf", source, destination);
}