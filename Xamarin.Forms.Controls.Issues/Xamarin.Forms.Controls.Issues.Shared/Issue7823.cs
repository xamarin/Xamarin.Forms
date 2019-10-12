using Xamarin.Forms.CustomAttributes;
using Xamarin.Forms.Internals;

namespace Xamarin.Forms.Controls.Issues
{
	[Preserve(AllMembers = true)]
	[Issue(IssueTracker.Github, 7823, "[Bug] Frame corner radius.", PlatformAffected.Android)]
	public sealed class Issue7823 : TestContentPage
	{
		protected override void Init()
		{
			Content = new Frame
			{
				CornerRadius = 5,
				BackgroundColor = Color.Red,
				Padding = 10,
				Content = new Frame
				{
					CornerRadius = 10,
					BackgroundColor = Color.Blue,
					Padding = 0,
					Content = new BoxView
					{
						BackgroundColor = Color.Green,
						WidthRequest = 100,
						HeightRequest = 100
					}
				}
			};
		}
	}
}
