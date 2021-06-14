using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android.UnitTests;
using System.Threading;
using System;
using AndroidX.AppCompat.App;
using AToolbar = AndroidX.AppCompat.Widget.Toolbar;
using AndroidX.AppCompat.Widget;

namespace Xamarin.Forms.Platform.Android.UnitTests
{
	[Preserve(AllMembers = true)]
	[Activity(Label = "TestActivity", Icon = "@drawable/icon", Theme = "@style/MyTheme",
		MainLauncher = false, HardwareAccelerated = true,
		ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize | ConfigChanges.UiMode)]
	public class TestActivity : AppCompatActivity
	{
		public static SemaphoreSlim semaphore = new SemaphoreSlim(1, 1);
		public static TaskCompletionSource<TestActivity> Surface { get; set; }

		TestLinearLayoutCompat _linearLayoutCompat;
		TaskCompletionSource<bool> _windowAttached = new TaskCompletionSource<bool>();
		TaskCompletionSource<bool> _windowDetached = new TaskCompletionSource<bool>();
		public Task<bool> WindowAttachedTask => _windowAttached.Task;
		public Task<bool> WindowDetachedTask => _windowDetached.Task;
		VisualElement Element { get; set; }
		protected override void OnCreate(Bundle savedInstanceState)
		{
			base.OnCreate(savedInstanceState);

			_linearLayoutCompat = new TestLinearLayoutCompat(this);

			SetContentView(_linearLayoutCompat,
				new LinearLayoutCompat.LayoutParams(LinearLayoutCompat.LayoutParams.MatchParent, LinearLayoutCompat.LayoutParams.MatchParent));

			var bar = LayoutInflater.Inflate(FormsAppCompatActivity.ToolbarResource, null).JavaCast<AToolbar>();
			SetSupportActionBar(bar);
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
				result.Element = visualElement;
				var maintainApp = Xamarin.Forms.Application.Current;
				visualElement.Parent = new Application();
				Xamarin.Forms.Application.Current = maintainApp;
				var renderer = Platform.CreateRendererWithContext(visualElement, result);

				renderer.View.ViewAttachedToWindow += result.OnViewAttachedToWindow;
				renderer.View.ViewDetachedFromWindow += result.OnViewDetachedToWindow;

				Platform.SetRenderer(visualElement, renderer);
				result._linearLayoutCompat.AddView(renderer.View, new LinearLayoutCompat.LayoutParams(LinearLayoutCompat.LayoutParams.MatchParent, LinearLayoutCompat.LayoutParams.MatchParent));
			}

			return result;
		}

		void OnViewDetachedToWindow(object sender, global::Android.Views.View.ViewDetachedFromWindowEventArgs e)
		{
			if (sender is global::Android.Views.View view)
				view.ViewDetachedFromWindow -= OnViewDetachedToWindow;

			_windowDetached.SetResult(true);
		}

		void OnViewAttachedToWindow(object sender, global::Android.Views.View.ViewAttachedToWindowEventArgs e)
		{
			if (sender is global::Android.Views.View view)
				view.ViewAttachedToWindow -= OnViewAttachedToWindow;

			_windowAttached.SetResult(true);
		}

		public override void Finish()
		{
			if (Element != null)
			{
				Element.Parent = null;
				Platform.SetRenderer(Element, null);
			}

			var view = _linearLayoutCompat.GetChildAt(0);
			if (view.IsAlive())
			{
				view.ViewAttachedToWindow -= OnViewAttachedToWindow;
				view.ViewDetachedFromWindow -= OnViewDetachedToWindow;
			}

			_linearLayoutCompat.RemoveAllViews();
			base.Finish();
		}

		protected override void OnDestroy()
		{
			base.OnDestroy();
			semaphore.Release();
		}

		protected override void OnResume()
		{
			base.OnResume();
			Surface.SetResult(this);
		}

		[Preserve(AllMembers = true)]
		class TestLinearLayoutCompat : LinearLayoutCompat
		{
			public TestLinearLayoutCompat(Context context) : base(context)
			{
			}
		}
	}
}
