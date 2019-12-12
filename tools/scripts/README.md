# Scripts

## profile-android.ps1

A script for profiling Xamarin.Forms startup on Android.

You can get rich help text via:

    PS> .\tools\scripts\profile-android.ps1 -?

An example of usage:

    PS> profile-android.ps1 -iterations 3
    Launching: com.xamarin.forms.helloforms
    Launching: com.xamarin.forms.helloforms
    Launching: com.xamarin.forms.helloforms
    12-12 09:13:15.593 1876 1898 I ActivityManager: Displayed com.xamarin.forms.helloforms/crc6450e568c951913723.MainActivity: +1s501ms
    12-12 09:13:19.696 1876 1898 I ActivityManager: Displayed com.xamarin.forms.helloforms/crc6450e568c951913723.MainActivity: +1s475ms
    12-12 09:13:23.863 1876 1898 I ActivityManager: Displayed com.xamarin.forms.helloforms/crc6450e568c951913723.MainActivity: +1s564ms
    Average(ms): 1513.33333333333

For further reading, see the [Xamarin.Android documentation][profiling]
on other ways of profiling app startup.

[profiling]: https://github.com/xamarin/xamarin-android/blob/master/Documentation/guides/profiling.md
