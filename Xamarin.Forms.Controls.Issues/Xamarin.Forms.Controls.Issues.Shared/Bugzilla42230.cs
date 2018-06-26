using Xamarin.Forms.CustomAttributes;
using Xamarin.Forms.Internals;
using System.Linq;

namespace Xamarin.Forms.Controls.Issues
{
	[Preserve(AllMembers = true)]
	[Issue(IssueTracker.Bugzilla, 42230, "CarouselView Moves to First item on Rotation (iOS)", PlatformAffected.iOS)]
	public class Bugzilla42230 : TestContentPage
	{
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
				})
			};

			Content = new StackLayout
			{
				Children = {
					new Label { Text = "The CarouselView should display \"1\". Rotate the device. If the CarouselView does not still display \"1\", this test has failed." },
					cv
				}
			};

			cv.Position = 1;
		}
	}
}