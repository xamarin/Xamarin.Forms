
string monoMajorVersion = "6.4.0";
string monoPatchVersion = "198";
string monoVersion = $"{monoMajorVersion}.{monoPatchVersion}";

string monoSDK_windows = $"https://download.mono-project.com/archive/{monoMajorVersion}/windows-installer/mono-{monoVersion}-x64-0.msi";
string androidSDK_windows = "https://aka.ms/xamarin-android-commercial-d16-3-windows";
string iOSSDK_windows = "";
string macSDK_windows = "";

string androidSDK_macos = "https://aka.ms/xamarin-android-commercial-d16-3-macos";
string monoSDK_macos = $"https://download.mono-project.com/archive/{monoMajorVersion}/macos-10-universal/MonoFramework-MDK-{monoVersion}.macos10.xamarin.universal.pkg";
string iOSSDK_macos = $"https://bosstoragemirror.blob.core.windows.net/wrench/jenkins/d16-3/5e8a208b5f44c4885060d95e3c3ad68d6a5e95e8/40/package/xamarin.ios-13.2.0.42.pkg";
string macSDK_macos = $"https://bosstoragemirror.blob.core.windows.net/wrench/jenkins/d16-3/5e8a208b5f44c4885060d95e3c3ad68d6a5e95e8/40/package/xamarin.mac-6.2.0.42.pkg";


if (IsMac)
{
	Item (XreItem.Xcode_11_1_0_rc).XcodeSelect ();

	if(!String.IsNullOrEmpty(androidSDK_macos))
		Item (androidSDK_macos);
	if(!String.IsNullOrEmpty(monoSDK_macos))
		Item (monoSDK_macos);
	if(!String.IsNullOrEmpty(iOSSDK_macos))
		Item (iOSSDK_macos);
	if(!String.IsNullOrEmpty(macSDK_macos))
		Item (macSDK_macos);
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
		Item (androidSDK_windows);
	if(!String.IsNullOrEmpty(monoSDK_windows))
		Item (monoSDK_windows);
	if(!String.IsNullOrEmpty(iOSSDK_windows))
		Item (iOSSDK_windows);
	if(!String.IsNullOrEmpty(macSDK_windows))
		Item (macSDK_windows);
}

Item(XreItem.Java_OpenJDK_1_8_0_25);
AndroidSdk ().ApiLevel((AndroidApiLevel)29);

void ln (string source, string destination)
{
	Console.WriteLine ($"ln -sf {source} {destination}");
	if (!Config.DryRun)
		Exec ("/bin/ln", "-sf", source, destination);
}