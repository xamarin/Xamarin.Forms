using System.Threading;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Runtime;
using Android.Views;
using AndroidX.AppCompat.App;
using Microsoft.Maui.Controls.Compatibility;
using Microsoft.Maui.Controls.Compatibility.Platform.Android.UnitTests;
using AToolbar = AndroidX.AppCompat.Widget.Toolbar;

[assembly: ExportRenderer(typeof(TestShell), typeof(TestShellRenderer))]
namespace Microsoft.Maui.Controls.Compatibility.Platform.Android.UnitTests
{
	[Activity(Label = "TestActivity", Icon = "@drawable/icon", Theme = "@style/MyTheme",
		MainLauncher = false, HardwareAccelerated = true,
		ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize | ConfigChanges.UiMode)]
	public class TestActivity : AppCompatActivity
	{
		public static SemaphoreSlim semaphore = new SemaphoreSlim(1, 1);
		public static TaskCompletionSource<TestActivity> Surface { get; set; }

		protected override void OnCreate(Bundle savedInstanceState)
		{
			base.OnCreate(savedInstanceState);
			var bar = LayoutInflater.Inflate(FormsAppCompatActivity.ToolbarResource, null).JavaCast<AToolbar>();
			SetSupportActionBar(bar);
		}

		protected override void OnResume()
		{
			base.OnResume();
			Surface.SetResult(this);
		}

		public static async Task<TestActivity> GetTestSurface(Context context, VisualElement visualElement)
		{
			await semaphore.WaitAsync();
			Surface = new TaskCompletionSource<TestActivity>();
			Intent intent = new Intent(context, typeof(TestActivity));
			context.StartActivity(intent);
			var result = await Surface.Task;

			if (visualElement != null)
			{
				var renderer = AppCompat.Platform.CreateRendererWithContext(visualElement, result);
				AppCompat.Platform.SetRenderer(visualElement, renderer);
				result.SetContentView(renderer.View);
			}

			return result;
		}

		protected override void OnDestroy()
		{
			base.OnDestroy();
			semaphore.Release();
		}
	}
}
