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
	[Issue(IssueTracker.Github, 13066, "[Bug] Shell.Background not displaying gradient Brush",
		PlatformAffected.All)]
#if UITEST
	[Category(UITestCategories.Brush)]
#endif
	public class Issue13066 : TestShell
	{
		public Issue13066()
		{
			SetBackgroundColor(this, Color.Red);

			SetBackground(this, new LinearGradientBrush
			{
				StartPoint = new Point(0, 0),
				EndPoint = new Point(1, 0),
				GradientStops = new GradientStopCollection
				{
					new GradientStop { Color = Color.Violet, Offset = 0.1f },
					new GradientStop { Color = Color.Red, Offset = 1.0f }
				}
			});
		}

		protected override void Init()
		{
			var page = CreateContentPage("Issue 13066");

			var clearButton = new Button
			{
				Text = "Clear Colors"
			};

			var colorButton = new Button
			{
				Text = "Set Shell.BackgroundColor"
			};

			var brushButton = new Button
			{
				Text = "Set Shell.Background"
			};

			var instructions = new StackLayout()
			{
				Children =
				{
					new Label()
					{
						Text = "If the background is a gradient, the test has passed."
					},
					clearButton,
					colorButton,
					brushButton
				}
			};

			page.Content = new StackLayout()
			{
				Children =
				{
					instructions
				}
			};

			clearButton.Clicked += (sender, args) =>
			{
				SetBackgroundColor(this, Color.Default);
				SetBackground(this, Brush.Default);
			};

			colorButton.Clicked += (sender, args) =>
			{
				SetBackgroundColor(this, Color.BlueViolet);
				SetBackground(this, Brush.Default);
			};

			brushButton.Clicked += (sender, args) =>
			{
				SetBackgroundColor(this, Color.Default);
				SetBackground(this, new LinearGradientBrush
				{
					StartPoint = new Point(0, 0),
					EndPoint = new Point(1, 0),
					GradientStops = new GradientStopCollection
					{
						new GradientStop { Color = Color.Green, Offset = 0.1f },
						new GradientStop { Color = Color.GreenYellow, Offset = 1.0f }
					}
				});
			};
		}
	}
}
