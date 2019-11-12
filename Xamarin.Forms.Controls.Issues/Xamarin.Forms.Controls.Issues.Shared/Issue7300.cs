using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Text;
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
	[Issue(IssueTracker.Github, 7300, "[Bug] [Android] Content rendered above a Material button is not visible when button is pressed",
		PlatformAffected.Android)]
#if UITEST
	[NUnit.Framework.Category(UITestCategories.Layout)]
#endif
	public class Issue7300 : TestContentPage
	{
		protected override void Init()
		{
			Visual = VisualMarker.Material;
			// Test with Grid
			Grid theGrid = null;
			theGrid = new Grid();
			SetupLayout(theGrid);

			// Test with AbsoluteLayout
			AbsoluteLayout absLayout = new AbsoluteLayout();
			SetupLayout(absLayout);

			// Test with RelativeLayout
			RelativeLayout relLayout = new RelativeLayout() { HeightRequest = 50 };
			SetupLayout(relLayout);

			StackLayout stackLayout = new StackLayout();
			stackLayout.Children.Add(new Label { Text = "Grid" });
			stackLayout.Children.Add(theGrid);
			stackLayout.Children.Add(new Label { Text = "AbsoluteLayout" });
			stackLayout.Children.Add(absLayout);
			stackLayout.Children.Add(new Label { Text = "RelativeLayout" });
			stackLayout.Children.Add(relLayout);

			Content = stackLayout;
		}

		void SetupLayout(Layout layout)
		{

			Button button = new Button()
			{
				HorizontalOptions = LayoutOptions.Fill,
				VerticalOptions = LayoutOptions.Fill,
				Text = "If you can see this, even briefly, the test has failed",
				AutomationId = "ClickMe",

			};
			button.Clicked += Button_Clicked;

			Image image = new Image()
			{
				Source = "coffee.png",
				HorizontalOptions = LayoutOptions.Fill,
				VerticalOptions = LayoutOptions.Fill,
				AutomationId = "ClickMe",
				BackgroundColor = Color.Green,
				InputTransparent = true
			};

			if (layout is Grid)
			{
				var grid = layout as Grid;
				grid.Children.Add(button);
				grid.Children.Add(image);
			}
			else if (layout is AbsoluteLayout)
			{
				var absLayout = layout as AbsoluteLayout;

				AbsoluteLayout.SetLayoutFlags(button, AbsoluteLayoutFlags.All);
				AbsoluteLayout.SetLayoutBounds(button, new Rectangle(0, 0, 1, 1));
				absLayout.Children.Add(button);

				AbsoluteLayout.SetLayoutFlags(image, AbsoluteLayoutFlags.All);
				AbsoluteLayout.SetLayoutBounds(image, new Rectangle(0, 0, 1, 1));
				absLayout.Children.Add(image);

			}
			else if (layout is RelativeLayout)
			{
				var relLayout = layout as RelativeLayout;

				relLayout.Children.Add(button,
					Forms.Constraint.Constant(0),
					Forms.Constraint.Constant(0),
					Forms.Constraint.Constant(320),
					Forms.Constraint.Constant(50)
					);
				relLayout.Children.Add(image,
					Forms.Constraint.Constant(0),
					Forms.Constraint.Constant(0),
					Forms.Constraint.Constant(320),
					Forms.Constraint.Constant(50)
					);
			}

		}

		private void Button_Clicked(object sender, EventArgs e)
		{
			System.Diagnostics.Debug.WriteLine($"{sender} Clicked");
		}

#if UITEST
		[Test]
		public void ImageShouldLayoutOnTopOfButton()
		{
			// Not sure how to make an automated UI test for this since the issue only occurs
			// during the time that the android button is animating for the state change, so even getting a 
			// screenshot after the button click likely will not capture the issue if it occurs. 
			// To manually test, run the 7300 issue test page and click all 3 buttons more than once 
			// (as issue does not occur every button click, but most times does) and make sure you never see
			// the button text "If you can see this, even briefly, the test has failed"
		}
#endif
	}
}
