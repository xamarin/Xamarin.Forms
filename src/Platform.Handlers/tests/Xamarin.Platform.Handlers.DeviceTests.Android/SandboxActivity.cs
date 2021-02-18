using System.Drawing;
using Android.App;
using Android.Content.PM;
using Android.OS;
using Android.Views;
using Android.Widget;
using AndroidX.AppCompat.App;
using Xamarin.Platform.Handlers.DeviceTests.Stubs;
using RegistrarHandlers = Xamarin.Platform.Registrar;

namespace Xamarin.Platform.Handlers.DeviceTests
{
	// This can be used as a quick scratch pad to sanity test how a view is rendering
	// Just set it as the MainLauncher.
	[Activity(
		Name = "com.xamarin.handlers.devicetests.SandboxActivity",
		Label = "@string/app_name",
		Theme = "@style/MainTheme",
		MainLauncher = true,
		ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
	public class SandboxActivity : AppCompatActivity
	{
		FrameLayout _rootLayout;
		protected override void OnCreate(Bundle bundle)
		{
			//Essentials.Platform.Init(this, bundle);
			Platform.Init(this);

			base.OnCreate(bundle);

			SetupRootLayout();

			var stuub = new SwitchStub()
			{
				IsToggled = true,
				TrackColor = Color.Red,
				ThumbColor = Color.Red,
				IsEnabled = true,
			};

			AddStubView<SwitchHandler, SwitchStub>(new SwitchStub()
			{
				IsToggled = true,
				TrackColor = Color.Red,
				ThumbColor = Color.Red,
				IsEnabled = true,
			});
		}


		void AddStubView<THandler, TView>(IView view)
			where TView : IView
			where THandler : IViewHandler
		{
			RegistrarHandlers.Handlers.Register<TView, THandler>();

			var nativeView = view.ToNative(this);
			nativeView.LayoutParameters = new FrameLayout.LayoutParams(FrameLayout.LayoutParams.WrapContent, FrameLayout.LayoutParams.WrapContent)
			{
				Gravity = GravityFlags.Center
			};

			_rootLayout.AddView(nativeView);
		}

		void SetupRootLayout()
		{
			_rootLayout = new FrameLayout(this);
			SetContentView(_rootLayout);
		}
	}
}