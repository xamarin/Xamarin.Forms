using System;

using Xamarin.Forms.Controls;
using Xamarin.Forms.CustomAttributes;
using Xamarin.Forms.Internals;
using Xamarin.Forms.Controls.Issues;

#if UITEST
using Xamarin.UITest;
using NUnit.Framework;
#endif

namespace Xamarin.Forms.Controls.Issues
{
	[Preserve(AllMembers = true)]
	[Issue(IssueTracker.Bugzilla, 53516, "ZoomableScrollView for Hitcents")]
	public class Bugzill53516 : TestContentPage
	{
		protected override void Init()
		{
			Content = new ZoomableScrollView
			{
				MaxZoom = 3,
				Orientation = ScrollOrientation.Both,
				Content = new Grid
				{
					WidthRequest = 1000,
					HeightRequest = 1000,
					ColumnSpacing = 10,
					RowSpacing = 10,
					Children =
					{
							{ new BoxView {Color = Color.Purple}, 0, 0 },
							{ new BoxView {Color = Color.Orange}, 0, 1 },
							{ new BoxView {Color = Color.Blue}, 0, 2 },
							{ new BoxView {Color = Color.Purple}, 1, 0 },
							{ new BoxView {Color = Color.Orange}, 1, 1 },
							{ new BoxView {Color = Color.Blue}, 1, 2 },
							{ new BoxView {Color = Color.Purple}, 2, 0 },
							{ new BoxView {Color = Color.Orange}, 2, 1 },
							{ new BoxView {Color = Color.Blue}, 2, 2 },
						}
				}
			};
		}
	}
}
