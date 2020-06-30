using Xamarin.Forms.CustomAttributes;
using Xamarin.Forms.Internals;

#if UITEST
using Xamarin.Forms.Core.UITests;
using Xamarin.UITest;
using NUnit.Framework;
#endif

namespace Xamarin.Forms.Controls.Issues
{
#if UITEST
	[NUnit.Framework.Category(Core.UITests.UITestCategories.Github5000)]
	[Category(Core.UITests.UITestCategories.Gestures)]
#endif
	[Preserve(AllMembers = true)]
	[Issue(IssueTracker.Github, 4714, "SingleTapGesture called once on DoubleTap", PlatformAffected.UWP)]
	public class Issue4714 : TestContentPage
	{
		const string InitialText = "Click Me To Increment";

		public Command TapCommand { get; set; }

		protected override void Init()
		{
			int i = 0;

			var tapGesture = new TapGestureRecognizer { NumberOfTapsRequired = 1 };
			tapGesture.SetBinding(TapGestureRecognizer.CommandProperty, "TapCommand");

			var label = new Label()
			{
				AutomationId = InitialText,
				HorizontalOptions = LayoutOptions.Center,
				Text = InitialText,
				GestureRecognizers =
				{
					tapGesture
				}
			};

			TapCommand = new Command(() =>
			{
				i++;
				label.Text = $"{InitialText}: {i}";
			});

			Content = new ContentView()
			{
				Content = new StackLayout()
				{
					Children =
					{
						label
					}
				}
			};
			BindingContext = this;
		}

#if UITEST
		[Test]
		public void Issue4714Test()
		{
			RunningApp.WaitForElement(InitialText);
			RunningApp.DoubleTap(InitialText);
			RunningApp.Tap(InitialText);
			RunningApp.Tap(InitialText);
			RunningApp.WaitForElement($"{InitialText}: 4");
		}
#endif
	}
}
