using System.Diagnostics;
using Xamarin.Forms.CustomAttributes;
using Xamarin.Forms.Internals;

#if UITEST
using Xamarin.UITest;
using NUnit.Framework;
#endif

namespace Xamarin.Forms.Controls
{
	[Preserve(AllMembers = true)]
	[Issue(IssueTracker.Bugzilla, 41415, "ScrollX and ScrollY values are not consistent with iOS", PlatformAffected.Android)]
	public class Bugzilla41415 : TestContentPage
	{
		protected override void Init()
		{
			var scrollView = new ScrollView
			{
				Orientation = ScrollOrientation.Both,
				HorizontalOptions = LayoutOptions.FillAndExpand,
				VerticalOptions = LayoutOptions.FillAndExpand
			};

			var grid = new Grid
			{
				BackgroundColor = Color.Yellow,
				WidthRequest = 1000,
				HeightRequest = 1000,
				Children =
				{
					new BoxView
					{
						WidthRequest =  200,
						HeightRequest = 200,
						BackgroundColor = Color.Red,
						HorizontalOptions = LayoutOptions.Center,
						VerticalOptions = LayoutOptions.Center
					}
				}
			};

			scrollView.Content = grid;
			scrollView.Scrolled += (sender, args) => { Debug.WriteLine("{0:0.0} {1:0.0}", args.ScrollX, args.ScrollY); };

			Content = scrollView;
		}
	}
}