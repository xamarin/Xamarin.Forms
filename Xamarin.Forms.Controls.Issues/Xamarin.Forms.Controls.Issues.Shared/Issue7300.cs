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
			// To test FastRenderer.ButtonRenderer, change the folowing to VisualMarker.Default
			// and comment out setting the LegacyRenderers flag in FormsAppCompatActivity in Xamarin.Forms.ControlGallery.Android
			Visual = VisualMarker.Material;
			// Test with Grid with overlap
			Grid theGrid = new Grid();
			SetupLayout(theGrid, true);

			// Test with AbsoluteLayout with overlap
			AbsoluteLayout absLayout = new AbsoluteLayout();
			SetupLayout(absLayout, true);

			// Test with RelativeLayout with overlap
			RelativeLayout relLayout = new RelativeLayout() { HeightRequest = 50 };
			SetupLayout(relLayout, true);

			// Test with Grid without overlap
			Grid theGrid2 = new Grid();
			SetupLayout(theGrid2, false);

			// Test with AbsoluteLayout without overlap
			AbsoluteLayout absLayout2 = new AbsoluteLayout();
			SetupLayout(absLayout2, false);

			// Test with RelativeLayout without overlap
			RelativeLayout relLayout2 = new RelativeLayout() { HeightRequest = 50 };
			SetupLayout(relLayout2, false);

			StackLayout stackLayout = new StackLayout() { BackgroundColor = Color.Red };
			stackLayout.Children.Add(new Label { Text = "Grid: Image covers button. Should never see button text" });
			stackLayout.Children.Add(theGrid);
			stackLayout.Children.Add(new Label { Text = "AbsoluteLayout: Image covers button. Should never see button text" });
			stackLayout.Children.Add(absLayout);
			stackLayout.Children.Add(new Label { Text = "RelativeLayout: Image covers button. Should never see button text" });
			stackLayout.Children.Add(relLayout);
			stackLayout.Children.Add(new Label { Text = "Grid: Image does not cover button. " });
			stackLayout.Children.Add(theGrid2);
			stackLayout.Children.Add(new Label { Text = "AbsoluteLayout: Image does not cover button. " });
			stackLayout.Children.Add(absLayout2);
			stackLayout.Children.Add(new Label { Text = "RelativeLayout: Image does not cover button. " });
			stackLayout.Children.Add(relLayout2);

			Content = stackLayout;
		}

		void SetupLayout(Layout layout, bool overlap)
		{

			Button button = new Button()
			{
				HorizontalOptions = LayoutOptions.Fill,
				VerticalOptions = LayoutOptions.Fill,
				Text = overlap ? "If you can see this, even briefly, the test has failed" : "Button",
			};
			button.Clicked += Button_Clicked;

			Image image = new Image()
			{
				Source = "coffee.png",
				HorizontalOptions = LayoutOptions.Fill,
				VerticalOptions = LayoutOptions.Fill,
				BackgroundColor = Color.Green,
				InputTransparent = true
			};

			if (layout is Grid)
			{
				var grid = layout as Grid;
				grid.Children.Add(button);
				grid.Children.Add(image);
				if (!overlap)
				{
					grid.ColumnDefinitions.Add(new ColumnDefinition() { Width = GridLength.Star });
					grid.ColumnDefinitions.Add(new ColumnDefinition() { Width = GridLength.Star });
					Grid.SetColumn(button, 0);
					Grid.SetColumn(image, 1);
				}
			}
			else if (layout is AbsoluteLayout)
			{
				var absLayout = layout as AbsoluteLayout;

				if(overlap)
				{
					AbsoluteLayout.SetLayoutFlags(button, AbsoluteLayoutFlags.All);
					AbsoluteLayout.SetLayoutBounds(button, new Rectangle(0, 0, 1, 1));

					AbsoluteLayout.SetLayoutFlags(image, AbsoluteLayoutFlags.All);
					AbsoluteLayout.SetLayoutBounds(image, new Rectangle(0, 0, 1, 1));
				}
				else
				{
					AbsoluteLayout.SetLayoutFlags(button, AbsoluteLayoutFlags.None);
					AbsoluteLayout.SetLayoutBounds(button, new Rectangle(0, 0, 195, 50));

					AbsoluteLayout.SetLayoutFlags(image, AbsoluteLayoutFlags.None);
					AbsoluteLayout.SetLayoutBounds(image, new Rectangle(205, 0, 195, 50));
				}

				absLayout.Children.Add(button);
				absLayout.Children.Add(image);
			}
			else if (layout is RelativeLayout)
			{
				var relLayout = layout as RelativeLayout;

				if (overlap)
				{
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
				else
				{
					relLayout.Children.Add(button,
						Forms.Constraint.Constant(0),
						Forms.Constraint.Constant(0),
						Forms.Constraint.Constant(195),
						Forms.Constraint.Constant(50)
						);
					relLayout.Children.Add(image,
						Forms.Constraint.Constant(205),
						Forms.Constraint.Constant(0),
						Forms.Constraint.Constant(195),
						Forms.Constraint.Constant(50)
						);
				}
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
