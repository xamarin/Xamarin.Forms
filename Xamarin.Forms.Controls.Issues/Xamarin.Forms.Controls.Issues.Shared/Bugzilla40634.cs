using System.Linq;
using Xamarin.Forms.CustomAttributes;
using Xamarin.Forms.Internals;

#if UITEST
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
	[Issue(IssueTracker.Bugzilla, 40634, "CarouselView fires (or not) \"ItemSelected\" or \"PositionSelected\" in an buggie way (v2.2.0.18-pre3)", PlatformAffected.iOS)]
	public class Bugzilla40634 : TestContentPage
	{
		const int Position = 3;
		protected override void Init()
		{
			var cv = new CarouselView
			{
				ItemsSource = Enumerable.Range(0, 10).ToList(),
				ItemTemplate = new DataTemplate(() =>
				{
					var label = new Label();
					label.SetBinding(Label.TextProperty, ".");
					return label;
				}),
				Position = Position,
				HeightRequest = 100,
				BackgroundColor = Color.AliceBlue
			};

			var positionLabel = new Label { BackgroundColor = Color.Beige };
			var itemSelectedLabel = new Label { BackgroundColor = Color.Beige };

			cv.PositionSelected += (s, e) => { positionLabel.Text = ((SelectedPositionChangedEventArgs)e).SelectedPosition.ToString(); };
			cv.ItemSelected += (s, e) => { itemSelectedLabel.Text = ((SelectedItemChangedEventArgs)e).SelectedItem.ToString(); };

			Content = new StackLayout
			{
				Children = {
						new Label { Text = $"Swipe the CarouselView partway, but do not complete the swipe. The text of the Labels (beige) should match the text of the CarouselView (blue). If they do not, this test has failed." },
						itemSelectedLabel,
						positionLabel,
						cv
					}
			};
		}

	}
}