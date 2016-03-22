#! /bin/bash
# IPAD_IP=10.0.1.106

IPOD_IP=10.0.3.253

DEVICE_IDIOM=PHONE DEVICE_ID=$IPOD_ID DEVICE_IP=$IPOD_IP mono packages/NUnit.Runners.2.6.3/tools/nunit-console-x86.exe -result=iphone-results.xml Xamarin.Forms.iOS.UITests/bin/Debug/Xamarin.Forms.iOS.UITests.dll;
# DEVICE_IDIOM=TABLET DEVICE_IP=$IPAD_IP mono packages/NUnit.Runners.2.6.3/tools/nunit-console-x86.exe -result=ipad-results.xml Xamarin.Forms.iOS.UITests/bin/Debug/Xamarin.Forms.iOS.UITests.dll;
