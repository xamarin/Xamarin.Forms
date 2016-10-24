﻿using Xamarin.Forms.CustomAttributes;
using Xamarin.Forms.Internals;

#if UITEST
using Xamarin.UITest;
using NUnit.Framework;
#endif

namespace Xamarin.Forms.Controls
{
	[Preserve(AllMembers = true)]
	[Issue(IssueTracker.Bugzilla, 44096, "Grid, StackLayout, and ContentView still participate in hit testing on Android after IsEnabled is set to false", PlatformAffected.Android)]
	public class Bugzilla44096 : TestContentPage
	{
		bool _flag;

		protected override void Init()
		{
			var result = new Label
			{
				Text = "Success"
			};

			var grid = new Grid
			{
				IsEnabled = false,
				WidthRequest = 250,
				HeightRequest = 50,
				AutomationId = "grid"
			};
			AddTapGesture(result, grid);

			var contentView = new ContentView
			{
				IsEnabled = false,
				WidthRequest = 250,
				HeightRequest = 50,
				AutomationId = "contentView"
			};
			AddTapGesture(result, contentView);

			var stackLayout = new StackLayout
			{
				IsEnabled = false,
				WidthRequest = 250,
				HeightRequest = 50,
				AutomationId = "stackLayout"
			};
			AddTapGesture(result, stackLayout);

			var color = new Button
			{
				Text = "Toggle colors",
				Command = new Command(() =>
				{
					if (!_flag)
					{
						grid.BackgroundColor = Color.Red;
						contentView.BackgroundColor = Color.Blue;
						stackLayout.BackgroundColor = Color.Yellow;
					}
					else
					{
						grid.BackgroundColor = Color.Default;
						contentView.BackgroundColor = Color.Default;
						stackLayout.BackgroundColor = Color.Default;
					}

					_flag = !_flag;
				}),
				AutomationId = "color"
			};

			var enabled = new Button
			{
				Text = "Enabled",
				Command = new Command(() =>
				{
					grid.IsEnabled = true;
					contentView.IsEnabled = true;
					stackLayout.IsEnabled = true;
				}),
				AutomationId = "enabled"
			};

			var parent = new StackLayout
			{
				Spacing = 10,
				Orientation = StackOrientation.Vertical,
				HorizontalOptions = LayoutOptions.Center,
				VerticalOptions = LayoutOptions.Center,
				Children =
				{
					color,
					enabled,
					result,
					grid,
					contentView,
					stackLayout
				}
			};
			AddTapGesture(result, parent, true);

			Content = parent;
		}

		void AddTapGesture(Label result, View view, bool isParent = false)
		{
			var tapGestureRecognizer = new TapGestureRecognizer
			{
				Command = new Command(() => { result.Text = !isParent ? "Child" : "Parent"; })
			};
			view.GestureRecognizers.Add(tapGestureRecognizer);
		}

#if UITEST
		[Test]
		public void Test()
		{
			RunningApp.WaitForElement(q => q.Marked("grid"));
			RunningApp.Tap(q => q.Marked("grid"));
			RunningApp.WaitForElement(q => q.Marked("Success"));

			RunningApp.WaitForElement(q => q.Marked("contentView"));
			RunningApp.Tap(q => q.Marked("contentView"));
			RunningApp.WaitForElement(q => q.Marked("Success"));

			RunningApp.WaitForElement(q => q.Marked("stackLayout"));
			RunningApp.Tap(q => q.Marked("stackLayout"));
			RunningApp.WaitForElement(q => q.Marked("Success"));

			RunningApp.WaitForElement(q => q.Marked("color"));
			RunningApp.Tap(q => q.Marked("color"));

			RunningApp.WaitForElement(q => q.Marked("grid"));
			RunningApp.Tap(q => q.Marked("grid"));
			RunningApp.WaitForElement(q => q.Marked("Success"));

			RunningApp.WaitForElement(q => q.Marked("contentView"));
			RunningApp.Tap(q => q.Marked("contentView"));
			RunningApp.WaitForElement(q => q.Marked("Success"));

			RunningApp.WaitForElement(q => q.Marked("stackLayout"));
			RunningApp.Tap(q => q.Marked("stackLayout"));
			RunningApp.WaitForElement(q => q.Marked("Success"));

			RunningApp.WaitForElement(q => q.Marked("enabled"));
			RunningApp.Tap(q => q.Marked("enabled"));

			RunningApp.WaitForElement(q => q.Marked("grid"));
			RunningApp.Tap(q => q.Marked("grid"));
			RunningApp.WaitForElement(q => q.Marked("Child"));

			RunningApp.WaitForElement(q => q.Marked("contentView"));
			RunningApp.Tap(q => q.Marked("contentView"));
			RunningApp.WaitForElement(q => q.Marked("Child"));

			RunningApp.WaitForElement(q => q.Marked("stackLayout"));
			RunningApp.Tap(q => q.Marked("stackLayout"));
			RunningApp.WaitForElement(q => q.Marked("Child"));

			RunningApp.WaitForElement(q => q.Marked("color"));
			RunningApp.Tap(q => q.Marked("color"));

			RunningApp.WaitForElement(q => q.Marked("grid"));
			RunningApp.Tap(q => q.Marked("grid"));
			RunningApp.WaitForElement(q => q.Marked("Child"));

			RunningApp.WaitForElement(q => q.Marked("contentView"));
			RunningApp.Tap(q => q.Marked("contentView"));
			RunningApp.WaitForElement(q => q.Marked("Child"));

			RunningApp.WaitForElement(q => q.Marked("stackLayout"));
			RunningApp.Tap(q => q.Marked("stackLayout"));
			RunningApp.WaitForElement(q => q.Marked("Child"));
		}
#endif
	}
}