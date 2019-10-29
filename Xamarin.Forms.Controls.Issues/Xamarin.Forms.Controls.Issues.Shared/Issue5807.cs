using Xamarin.Forms.CustomAttributes;
using Xamarin.Forms.Internals;

namespace Xamarin.Forms.Controls.Issues
{
	[Preserve(AllMembers = true)]
	[Issue(IssueTracker.Github, 5807, "[Visual][iOS] Material entry text caret miss aligned", PlatformAffected.iOS)]
	public class Issue5807 : TestContentPage
	{
		protected override void Init()
		{
			Content = new StackLayout()
			{
				Children =
				{
					new Label
					{
						Text = "Tap on the Entry and verify that the Caret is in the correct (center) position.",
						Margin = new Thickness(30)
					},
					new Entry
					{
						HorizontalTextAlignment = TextAlignment.Center,
						Visual = VisualMarker.Material,
						Placeholder = "loremipsumplaceholer"
					}
				}
			};
		}
	}
}
