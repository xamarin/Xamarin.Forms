#! /bin/bash
# NEXUS_IP=10.0.1.42

GALAXY_IP=10.0.1.161

# Genymotion
# GALAXY_IP=192.168.56.101

# NEXUS_IP=192.168.56.103

# DEVICE_IDIOM=TABLET DEVICE_ID=$NEXUS_ID DEVICE_IP=$NEXUS_IP mono packages/NUnit.Runners.2.6.3/tools/nunit-console-x86.exe -result=nexus-results.xml Xamarin.Forms.Android.UITests/bin/Debug/Xamarin.Forms.Android.UITests.dll; 
DEVICE_IDIOM=PHONE DEVICE_IP=$GALAXY_IP mono packages/NUnit.Runners.2.6.3/tools/nunit-console-x86.exe -result=galaxy-results.xml Xamarin.Forms.Android.UITests/bin/Debug/Xamarin.Forms.Android.UITests.dll;
