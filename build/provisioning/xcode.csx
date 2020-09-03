using System;
using System.Linq;

var desiredXcode = Environment.GetEnvironmentVariable ("REQUIRED_XCODE");
if (string.IsNullOrEmpty (desiredXcode)) {
	Console.WriteLine ("The environment variable 'REQUIRED_XCODE' must be exported and the value must be a valid value from the 'XreItem' enumeration.");
	return;
}

desiredXcode = desiredXcode.Replace("Xcode_", "").Replace("_", ".");

Item item;

if(desiredXcode == "Latest")
	item = XcodeBeta();
else if (desiredXcode == "Stable")
	item = XcodeStable();
else
	item = Xcode(desiredXcode);

Console.WriteLine ("InstallPath: {0}", item.Version);
item.XcodeSelect ();
