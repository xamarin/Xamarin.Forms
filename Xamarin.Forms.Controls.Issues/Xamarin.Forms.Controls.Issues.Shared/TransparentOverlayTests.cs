using System;
using System.Collections.Generic;
using System.Diagnostics;
using Xamarin.Forms.CustomAttributes;
using Xamarin.Forms.Internals;
using System.Linq;

#if UITEST
using Xamarin.Forms.Core.UITests;
using Xamarin.UITest;
using NUnit.Framework;
#endif

namespace Xamarin.Forms.Controls.Issues
{
#if UITEST
	[Category(UITestCategories.InputTransparent)]
#endif
	[Preserve(AllMembers = true)]
	[Issue(IssueTracker.None, 618, "Transparent Overlays", PlatformAffected.All)]
	public class TransparentOverlayTests : TestNavigationPage
	{
		readonly Color _transparentColor = Color.Transparent;
		readonly Color _nontransparentColor = Color.Blue;

		double _transparentOpacity = 0;
		double _nonTransparentOpacity = 0.2;

		const string Success = "Success";
		const string Failure = "Failure";
		const string DefaultButtonText = "Button";

		protected override void Init()
		{
			PushAsync(Menu());
		}

		ContentPage Menu()
		{
			var layout = new StackLayout();

			layout.Children.Add(new Label {Text = "Select a test below"});

			foreach (var test in GenerateTests)
			{
				layout.Children.Add(MenuButton(test));
			}

			return new ContentPage { Content = layout };
		}

		Button MenuButton(TestPoint test)
		{
			var button = new Button { Text = test.ToString(), AutomationId = test.AutomationId };

			button.Clicked += (sender, args) => PushAsync(CreateTestPage(test));

			return button;
		}

		[Preserve(AllMembers = true)]
		public struct TestPoint
		{
			public TestPoint(int i) : this()
			{
				AutomationId = $"transparenttest{i}";

				// TODO hartez 2017/05/22 15:45:12 Update this with real values	
				ShouldBeTransparent = true;

				Opacity = (i & (1 << 0)) == 0;
				InputTransparent = (i & (1 << 1)) == 0;
				BackgroundColor = (i & (1 << 2)) == 0;
			}

			internal string AutomationId { get; set; }
			internal bool ShouldBeTransparent { get; set; }

			internal bool Opacity { get; set; }
			internal bool InputTransparent { get; set; }
			internal bool BackgroundColor { get; set; }
			
			public override string ToString()
			{
				return $"O{(Opacity ? "1" : "0")}, B{(BackgroundColor ? "1" : "0")}, I{(InputTransparent ? "1" : "0")}";
			}
		}

		static IEnumerable<TestPoint> GenerateTests
		{
			get { return Enumerable.Range(0, 8).Select(i => new TestPoint(i)); }
		}

		ContentPage CreateTestPage(TestPoint test)
		{
			Color backgroundColor = test.BackgroundColor? _transparentColor : _nontransparentColor;
			double opacity = test.Opacity ? _transparentOpacity : _nonTransparentOpacity;
			bool inputTransparent = test.InputTransparent;

			var grid = new Grid() {
				HorizontalOptions = LayoutOptions.Fill,
				VerticalOptions = LayoutOptions.Fill
			};

			grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
			grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Star });

			var instructions = new Label
			{
				HorizontalOptions = LayoutOptions.Fill,
				HorizontalTextAlignment = TextAlignment.Center,
				Text = $"Tap the button below."
				       + (test.ShouldBeTransparent
					       ? $" If the button's text changes to {Success} the test has passed."
					       : " If the button's text remains unchanged, the test has passed.")
			};

			grid.Children.Add(instructions);

			var button = new Button
			{
				Text = DefaultButtonText,
				HorizontalOptions = LayoutOptions.Center,
				VerticalOptions = LayoutOptions.Center
			};

			button.Clicked += (sender, args) =>
			{
				button.Text = test.ShouldBeTransparent ? Success : Failure;
			};

			var layout = new StackLayout
			{
				HorizontalOptions = LayoutOptions.Fill,
				VerticalOptions = LayoutOptions.Fill,
				BackgroundColor = backgroundColor,
				InputTransparent = inputTransparent,
				Opacity = opacity
			};

			grid.Children.Add(button);
			Grid.SetRow(button, 1);
			
			grid.Children.Add(layout);
			Grid.SetRow(layout, 1);

			return new ContentPage { Content = grid, Title = test.ToString()};
		}

#if UITEST
		[Test, TestCaseSource(nameof(GenerateTests))]
		public void VerifyInputTransparent(TestPoint test)
		{
			RunningApp.WaitForElement(q => q.Marked(test.AutomationId));
			RunningApp.Tap(test.AutomationId);

			RunningApp.WaitForElement(DefaultButtonText);
			RunningApp.Tap(DefaultButtonText);

			if (test.ShouldBeTransparent)
			{
				RunningApp.WaitForElement(Success);
			}
			else
			{
				RunningApp.WaitForElement(DefaultButtonText);
			}
		}
#endif

	}
}