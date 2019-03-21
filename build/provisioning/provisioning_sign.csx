using System;
using System.IO;
using System.Linq;
using Serilog;

if (!IsMac)
  return;

Console.WriteLine(Env("APPLECODESIGNIDENTITYURL"));
Console.WriteLine(Env("APPLECODESIGNIDENTITY"));
Console.WriteLine(Env("APPLECODESIGNPROFILEURL"));
  
AppleCodesignIdentity(Env("APPLECODESIGNIDENTITY"),Env("APPLECODESIGNIDENTITYURL"));
AppleCodesignProfile(Env("APPLECODESIGNPROFILEURL"));
