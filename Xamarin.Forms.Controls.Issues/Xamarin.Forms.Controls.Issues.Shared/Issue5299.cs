using Xamarin.Forms.CustomAttributes;
using Xamarin.Forms.Internals;

namespace Xamarin.Forms.Controls.Issues
{
	[Preserve(AllMembers = true)]
	[Issue(IssueTracker.Github, 5299, "Disabled Button Appearance", PlatformAffected.iOS | PlatformAffected.Android)]
	public class Issue5299 : TestContentPage
	{
		protected override void Init()
		{
			Content = new StackLayout
			{
				Children =
				{
					 new Button
					 {
						Text = "This text should be disabled and have a custom disabled text color.",
						TextColor = Color.Black,
						DisabledTextColor = Color.Gray,
						IsEnabled = false
					 }
				}
			};
		}
	}
}
