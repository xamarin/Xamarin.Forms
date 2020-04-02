string monoMajorVersion = "6.8.0";
string monoPatchVersion = "123";
string monoVersion = $"{monoMajorVersion}.{monoPatchVersion}";

string monoSDK_windows = "";//$"https://download.mono-project.com/archive/{monoMajorVersion}/windows-installer/mono-{monoVersion}-x64-0.msi";
string androidSDK_windows = "";//"https://download.visualstudio.microsoft.com/download/pr/1131a8f5-99f5-4326-93b1-f5827b54ecd5/e7bd0f680004131157a22982c389b05f2d3698cc04fab3901ce2d7ded47ad8e0/Xamarin.Android.Sdk-10.0.0.43.vsix";
string iOSSDK_windows = "";
string macSDK_windows = "";

string androidSDK_macos = "https://download.visualstudio.microsoft.com/download/pr/8f94ca38-039a-4c9f-a51a-a6cb33c76a8c/aa46188c5f7a2e0c6f2d4bd4dc261604/xamarin.android-10.2.0.100.pkg";
string monoSDK_macos = $"https://download.visualstudio.microsoft.com/download/pr/8f94ca38-039a-4c9f-a51a-a6cb33c76a8c/3a376d8c817ec4d720ecca2d95ceb4c1/monoframework-mdk-6.8.0.123.macos10.xamarin.universal.pkg";
string iOSSDK_macos = $"https://download.visualstudio.microsoft.com/download/pr/6e56949e-1beb-4550-abf9-ff404868de82/cf7090bee19401076987a57cd12f11e5/xamarin.ios-13.16.0.11.pkg";
string macSDK_macos = $"https://download.visualstudio.microsoft.com/download/pr/6e56949e-1beb-4550-abf9-ff404868de82/547895e66c0543faccb25933d8691371/xamarin.mac-6.16.0.11.pkg";


if (IsMac)
{
	// Item (XreItem.Xcode_11_4_0).XcodeSelect ();

  	if(!String.IsNullOrEmpty(monoSDK_macos))
    	Item ("Mono", monoVersion)
      	.Source (_ => monoSDK_macos);

	if(!String.IsNullOrEmpty(androidSDK_macos))
		Item ("Xamarin.Android", "10.2.0.100")
      .Source (_ => androidSDK_macos);

	if(!String.IsNullOrEmpty(iOSSDK_macos))
		Item ("Xamarin.iOS", "13.16.0.11")
      .Source (_ => iOSSDK_macos);

	if(!String.IsNullOrEmpty(macSDK_macos))
		Item ("Xamarin.Mac", "6.16.0.11")
      .Source (_ => macSDK_macos);
    
	ForceJavaCleanup();

    var dotnetVersion = System.Environment.GetEnvironmentVariable("DOTNET_VERSION");
    if (!string.IsNullOrEmpty(dotnetVersion))
	  {
		// VSTS installs into a non-default location. Let's hardcode it here because why not.
		var vstsBaseInstallPath = Path.Combine (Environment.GetEnvironmentVariable ("HOME"), ".dotnet", "sdk");
		var vstsInstallPath = Path.Combine (vstsBaseInstallPath, dotnetVersion);
		var defaultInstallLocation = Path.Combine ("/usr/local/share/dotnet/sdk/", dotnetVersion);
		if (Directory.Exists (vstsBaseInstallPath) && !Directory.Exists (vstsInstallPath))
			ln (defaultInstallLocation, vstsInstallPath);
	  }
}
else
{
	if(!String.IsNullOrEmpty(androidSDK_windows))
		Item ("Xamarin.Android", "10.0.0.43")
      .Source (_ => androidSDK_windows);

	if(!String.IsNullOrEmpty(iOSSDK_windows))
		Item ("Xamarin.iOS", "13.2.0.42")
      .Source (_ => iOSSDK_windows);

	if(!String.IsNullOrEmpty(macSDK_windows))
		Item ("Xamarin.Mac", "6.2.0.42")
      .Source (_ => macSDK_windows);

	if(!String.IsNullOrEmpty(monoSDK_windows))
    Item ("Mono", monoVersion)
      .Source (_ => monoSDK_windows);

}

// Item(XreItem.Java_OpenJDK_1_8_0_25);
AndroidSdk ().ApiLevel((AndroidApiLevel)24);
AndroidSdk ().ApiLevel((AndroidApiLevel)28);
AndroidSdk ().ApiLevel((AndroidApiLevel)29);

void ln (string source, string destination)
{
	Console.WriteLine ($"ln -sf {source} {destination}");
	if (!Config.DryRun)
		Exec ("/bin/ln", "-sf", source, destination);
}
