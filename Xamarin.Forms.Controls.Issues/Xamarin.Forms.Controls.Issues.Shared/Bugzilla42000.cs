using Xamarin.Forms.CustomAttributes;
using Xamarin.Forms.Internals;

namespace Xamarin.Forms.Controls.TestCasesPages
{
	[Preserve(AllMembers = true)]
	[Issue(IssueTracker.Bugzilla, 42000, "Unable to use comma (\", \") as decimal point", PlatformAffected.Android)]
	public class Bugzilla42000 : ContentPage
	{
		public Bugzilla42000()
		{
			var flag = false;

			var entry = new Entry { Keyboard = Keyboard.Numeric };

			var button = new Button
			{
				Text = "Change keyboard type"
			};
			button.Clicked += (sender, args) =>
			{
				entry.Keyboard = !flag ? Keyboard.Chat : Keyboard.Numeric;

				flag = !flag;
			};

			Content = new StackLayout
			{
				VerticalOptions = LayoutOptions.Center,
				Children =
				{
					entry,
					button
				}
			};
		}
	}
}