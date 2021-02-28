using Android.App;
using Android.Content.PM;
using Android.OS;
using Microsoft.Maui.Controls.Compatibility.Platform.Android;

namespace Compatibility.Sample.Droid
{
	[Activity(
		Label = "Compatibility.Sample",
		Icon = "@mipmap/icon",
		Theme = "@style/MainTheme",
		MainLauncher = true,
		ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize)]
	public class MainActivity : FormsAppCompatActivity
	{
		protected override void OnCreate(Bundle savedInstanceState)
		{
			base.OnCreate(savedInstanceState);

			Microsoft.Maui.Essentials.Platform.Init(this, savedInstanceState);
			Microsoft.Maui.Controls.Compatibility.Forms.Init(this, savedInstanceState);

			LoadApplication(new App());
		}

		public override void OnRequestPermissionsResult(int requestCode, string[] permissions, Permission[] grantResults)
		{
			Microsoft.Maui.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

			base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
		}
	}
}