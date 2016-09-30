using Xamarin.Forms.CustomAttributes;
using Xamarin.Forms.Internals;

#if UITEST
using Xamarin.UITest;
using NUnit.Framework;
#endif

namespace Xamarin.Forms.Controls
{
	[Preserve(AllMembers = true)]
	[Issue(IssueTracker.Bugzilla, 44944, "iOS: Text goes outside the bounds of Entry if it can't fit inside", PlatformAffected.iOS)]
	public class Bugzilla44944 : TestContentPage
	{
		protected override void Init()
		{
			Content = new Grid
			{
				Children =
				{
					new Entry
					{
						FontSize = 200,
						HorizontalOptions = LayoutOptions.Center,
						VerticalOptions = LayoutOptions.Center
					}
				}
			};
		}
	}
}