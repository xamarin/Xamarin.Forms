using System.Diagnostics;
using Xamarin.Forms.CustomAttributes;
using Xamarin.Forms.Internals;

#if UITEST
using Xamarin.UITest;
using NUnit.Framework;
#endif

// Apply the default category of "Issues" to all of the tests in this assembly
// We use this as a catch-all for tests which haven't been individually categorized
#if UITEST
[assembly: NUnit.Framework.Category("Issues")]
#endif

namespace Xamarin.Forms.Controls.Issues
{
	[Preserve(AllMembers = true)]
	[Issue(IssueTracker.Bugzilla, 44074, "OnDetachingFrom for Behavior is never called, therefore memory can leak due to event handlers not being unbound", PlatformAffected.All)]
	public class Bugzilla44074 : TestNavigationPage // or TestMasterDetailPage, etc ...
	{
		protected override void Init()
		{
			var cp = new ContentPage();
			var sl = new StackLayout { Padding = 20 };

			var label = new Label
			{
				Text = "This is the first page with no behaviors",
				HorizontalTextAlignment = TextAlignment.Center,
				VerticalTextAlignment = TextAlignment.Center
			};
			sl.Children.Add(label);

			var button = new Button
			{
				Text = "Open Behavior View",
				Command = new Command(() => { Navigation.PushAsync(new TestSecondPage()); })
			};
			sl.Children.Add(button);

			cp.Content = sl;
			PushAsync(cp);
		}
	}

	public class TestSecondPage : ContentPage
	{
		public TestSecondPage()
		{
			var sl = new StackLayout { Padding = 20 };

			var label = new Label
			{
				Text = "This is the Second Page with a Behavior Attached",
				HorizontalTextAlignment = TextAlignment.Center,
				VerticalTextAlignment = TextAlignment.Center
			};
			label.Behaviors.Add(new BehaviorTestEvents());
			sl.Children.Add(label);

			var button = new Button
			{
				Text = "Open initial view",
				Command = new Command(() => { Navigation.PopAsync(); })
			};
			sl.Children.Add(button);

			Content = sl;
		}
	}

	public class BehaviorTestEvents : Behavior<View>
	{
		protected override void OnAttachedTo(View bindable)
		{
			base.OnAttachedTo(bindable);

			Debug.WriteLine("OnAttached");
		}

		protected override void OnDetachingFrom(View bindable)
		{
			base.OnDetachingFrom(bindable);

			Debug.WriteLine("OnDetachingFrom");
		}
	}
}