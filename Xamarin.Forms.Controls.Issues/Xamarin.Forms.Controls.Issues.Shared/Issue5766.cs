using System;
using System.Collections;
using System.Linq;
using System.Collections.ObjectModel;

using Xamarin.Forms.CustomAttributes;
using Xamarin.Forms.Internals;
using System.Collections.Generic;

#if UITEST
using Xamarin.UITest;
using NUnit.Framework;
using Xamarin.Forms.Core.UITests;
#endif

namespace Xamarin.Forms.Controls.Issues
{
	[Preserve (AllMembers=true)]
	[Issue (IssueTracker.Github, 5766, "Frame size gets corrupted when ListView is scrolled", PlatformAffected.Android)]
#if UITEST
	[NUnit.Framework.Category(UITestCategories.Layout)]
#endif
	public class Issue5766 : TestContentPage
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
				Text = "Scroll up and down several times and make sure Frame size is accurate when using Fast Renderers.",
				VerticalTextAlignment = TextAlignment.Center
			}, 0, 0);
			grid.AddChild(new ListView
			{
				AutomationId = List,
				HasUnevenRows = true,
				ItemsSource = (new[] { StartText }).Concat(Enumerable.Range(0, 99).Select(i => i % 2 != 0 ? SmallText : BigText)).Concat(new[] { EndText }),
				ItemTemplate = new DataTemplate(() =>
				{
					var text = new Label
					{
						VerticalOptions = LayoutOptions.Fill,
						TextColor = Color.White
					};
					text.SetBinding(Label.TextProperty, ".");
					var view = new Grid
					{
						HeightRequest = 200,
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
		UITest.Queries.AppRect[] GetLabels(IApp RunningApp, string label)
		{
			return RunningApp
				.Query(q => q.Class("FormsTextView"))
				.Where(x => x.Text == label)
				.Select(x => x.Rect)
				.ToArray();
		}

		bool RectIsEquals(UITest.Queries.AppRect[] left, UITest.Queries.AppRect[] right)
		{
			if (left.Length != right.Length)
				return false;

			for (int i = 0; i < left.Length; i++)
			{
				if (left[i].X != right[i].X || 
					left[i].Y != right[i].Y || 
					left[i].Width != right[i].Width || 
					left[i].Height != right[i].Height)
					return false;
			}

			return true;
		}

		[Test]
		public void Issue5766Test()
		{
			RunningApp.WaitForElement(StartText);
			var start = GetLabels(RunningApp, StartText);
			var smalls = GetLabels(RunningApp, SmallText);
			var bigs = GetLabels(RunningApp, BigText);

			RunningApp.ScrollDownTo(EndText, List, ScrollStrategy.Gesture, 0.9, 15000, timeout: TimeSpan.FromMinutes(1));
			RunningApp.ScrollUpTo(StartText, List, ScrollStrategy.Gesture, 0.9, 15000, timeout: TimeSpan.FromMinutes(1));

			var startAfter = GetLabels(RunningApp, StartText);
			Assert.IsTrue(RectIsEquals(start, startAfter));
			var smallAfter = GetLabels(RunningApp, SmallText);
			Assert.IsTrue(RectIsEquals(smalls, smallAfter));
			var bigAfter = GetLabels(RunningApp, BigText);
			Assert.IsTrue(RectIsEquals(bigs, bigAfter));
		}
#endif
	}
}
