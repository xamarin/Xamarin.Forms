using Xamarin.Forms.CustomAttributes;
using Xamarin.Forms.Internals;
using System.Linq;

#if UITEST
using Xamarin.UITest;
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
	[Issue(IssueTracker.Bugzilla, 48090, "Java.Lang.StackOverflowError when using FormattedText inside ListView inside CarouselView", PlatformAffected.Android)]
	public class Bugzilla48090 : TestContentPage
	{
		const string Success = "Success";

		protected override void Init()
		{
			Content = new CarouselView
			{
				ItemsSource = Enumerable.Range(0, 10).ToList(),
				ItemTemplate = new DataTemplate(() =>
				{
					var output = new FormattedString();
					output.Spans.Add(new Span() { Text = Success, FontSize = 24, ForegroundColor = Color.FromHex("#120A8F") });

					return new ListView
					{
						HasUnevenRows = true,
						ItemsSource = Enumerable.Range(0, 10),
						ItemTemplate = new DataTemplate(() =>
						{
							return new ViewCell()
							{
								View = new StackLayout
								{
									Orientation = StackOrientation.Horizontal,
									HorizontalOptions = LayoutOptions.Fill,
									Children = { new StackLayout { Orientation = StackOrientation.Vertical, HorizontalOptions = LayoutOptions.Start, Children = { new Label { FormattedText = output } } } }
								}
							};
						})
					};
				})
			};
		}

#if UITEST
		[Test]
		public void Bugzilla48090Test()
		{
			RunningApp.WaitForElement(q => q.Marked(Success));
		}
#endif
	}
}