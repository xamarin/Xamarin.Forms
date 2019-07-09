using System;
using System.Linq;

using Xamarin.Forms.CustomAttributes;
using Xamarin.Forms.Internals;

#if UITEST
using Xamarin.UITest;
using NUnit.Framework;
using Xamarin.Forms.Core.UITests;
#endif

namespace Xamarin.Forms.Controls.Issues
{
	[Preserve(AllMembers = true)]
	[Issue(IssueTracker.Github, 6297, "Frame size gets corrupted when ListView is scrolled (RecycleElement CachingStrategy)", PlatformAffected.Android)]
#if UITEST
	[NUnit.Framework.Category(UITestCategories.Layout)]
#endif
	public class Issue6297 : TestContentPage
	{
		const string SmallText = "small";
		const string BigText = "big string > big frame";
		const string StartText = "start";
		const string EndText = "end";
		const string List = "lstMain";

		protected override void Init()
		{
			var grid = new Grid
			{
				RowDefinitions = new RowDefinitionCollection
				{
					new RowDefinition { Height = GridLength.Auto},
					new RowDefinition(),
				}
			};
			grid.AddChild(new Label
			{
				Text = "Scroll up and down several times and make sure Frame size is accurate.",
				VerticalTextAlignment = TextAlignment.Center
			}, 0, 0);
			grid.AddChild(new ListView(ListViewCachingStrategy.RecycleElement)
			{
				AutomationId = List,
				HasUnevenRows = true,
				ItemsSource = (new[] { StartText }).Concat(Enumerable.Range(0, 99).Select(i => i % 2 != 0 ? SmallText : BigText)).Concat(new[] { EndText }),
				ItemTemplate = new DataTemplate(() =>
				{
					var text = new Label
					{
						VerticalOptions = LayoutOptions.Fill,
						TextColor = Color.White,
					};

					text.SetBinding(Label.TextProperty, ".");
					var view = new Grid
					{
						HeightRequest = 80,
						Margin = new Thickness(0, 10, 0, 0),
						BackgroundColor = Color.FromHex("#F1F1F1")
					};
					view.AddChild(new Frame
					{
						Padding = new Thickness(5),
						Margin = new Thickness(0, 0, 10, 0),
						BorderColor = Color.Blue,
						BackgroundColor = Color.Gray,
						VerticalOptions = LayoutOptions.Center,
						HorizontalOptions = LayoutOptions.End,
						CornerRadius = 3,
						Content = text
					}, 0, 0);
					return new ViewCell
					{
						View = view
					};
				})
			}, 0, 1);

			Content = grid;
		}

#if UITEST && __ANDROID__
		[Test]
		public void Issue6297Test()
		{
			RunningApp.WaitForElement(StartText);
			var start = Issue5766.GetLabels(RunningApp, StartText);
			var smalls = Issue5766.GetLabels(RunningApp, SmallText);
			var bigs = Issue5766.GetLabels(RunningApp, BigText);

			RunningApp.ScrollDownTo(EndText, List, ScrollStrategy.Gesture, 0.9, 15000, timeout: TimeSpan.FromMinutes(1));
			RunningApp.ScrollUpTo(StartText, List, ScrollStrategy.Gesture, 0.9, 15000, timeout: TimeSpan.FromMinutes(1));

			var startAfter = Issue5766.GetLabels(RunningApp, StartText);
			Assert.IsTrue(Issue5766.RectIsEquals(start, startAfter));
			var smallAfter = Issue5766.GetLabels(RunningApp, SmallText);
			Assert.IsTrue(Issue5766.RectIsEquals(smalls, smallAfter));
			var bigAfter = Issue5766.GetLabels(RunningApp, BigText);
			Assert.IsTrue(Issue5766.RectIsEquals(bigs, bigAfter));
		}
#endif
	}
}