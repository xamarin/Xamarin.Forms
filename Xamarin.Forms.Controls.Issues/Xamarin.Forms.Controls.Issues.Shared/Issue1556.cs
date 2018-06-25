using Xamarin.Forms.CustomAttributes;
using Xamarin.Forms.Internals;

#if UITEST
using Xamarin.UITest;
using NUnit.Framework;
#endif

namespace Xamarin.Forms.Controls.Issues
{
#if UITEST
	[System.ComponentModel.Category(Xamarin.Forms.Core.UITests.UITestCategories.Animation)]
#endif

	[Preserve(AllMembers = true)]
	[Issue(IssueTracker.Github, 1556, "Animation tasks do not complete when Battery Saver enabled", PlatformAffected.Android)]
	public class Issue1556 : TestContentPage
	{
		const string FirstLabel = "Label 1";
		const string SecondLabel = "Label 2";

		protected override void Init()
		{
			var instructions = new Label {Text = $"The label with the text '{SecondLabel}' should be visible." 
												+ $" If the label with '{SecondLabel}' does not become visible, the test has failed." };

			var label1 = new Label { Text = FirstLabel, Opacity = 0 };
			var label2 = new Label { Text = SecondLabel, IsVisible = false };

			var layout = new StackLayout();

			layout.Children.Add(instructions);
			layout.Children.Add(label1);
			layout.Children.Add(label2);

			Content = layout;

			Appearing += async (sender, args) =>
			{
				await label1.FadeTo(1);
				label2.IsVisible = true;
			};
		}

#if UITEST
		[Test, Explicit("This only makes sense to run if animator duration scale (in dev options) is set to 0 or low battery mode is active")]
		public void LowBatteryAnimationTest ()
		{
			RunningApp.WaitForElement(SecondLabel);
		}
#endif
	}
}