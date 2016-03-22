# Clean all bin/ and obj/
rm -rf Xamarin.Forms.ControlGallery.Android/bin/Debug/*;
rm -rf Xamarin.Forms.ControlGallery.Android/obj/Debug/*;

rm -rf Xamarin.Forms.ControlGallery.iOS/bin/iPhone/Debug/*;
rm -rf Xamarin.Forms.ControlGallery.iOS/obj/iPhone/Debug/*;

rm -rf Xamarin.Forms.Core.Android.UITests/bin/Debug/*;
rm -rf Xamarin.Forms.Core.Android.UITests/obj/Debug/*;

rm -rf Xamarin.Forms.Core.iOS.UITests/bin/Debug/*;
rm -rf Xamarin.Forms.Core.iOS.UITests/obj/Debug/*;

# Build .apk
/Applications/Xamarin\ Studio.app/Contents/MacOS/mdtool build Xamarin.Forms.sln -c:"Debug";

# Build .ipa
/Applications/Xamarin\ Studio.app/Contents/MacOS/mdtool build Xamarin.Forms.sln -c:"Debug|iPhone";

echo "################################"
echo "######    GOOD TO GO!     ######"
echo "################################"

