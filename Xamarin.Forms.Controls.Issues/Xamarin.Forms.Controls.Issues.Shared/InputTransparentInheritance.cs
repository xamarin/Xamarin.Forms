using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using Xamarin.Forms.CustomAttributes;
using Xamarin.Forms.Internals;
using Xamarin.Forms.PlatformConfiguration;
using Xamarin.Forms.PlatformConfiguration.AndroidSpecific;
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

	// TODO hartez 2018/01/17 15:58:40 Somewhere we need tests for when InputTransparentInherited changes	

	[Preserve(AllMembers = true)]
	[Issue(IssueTracker.None, 5552368, "Transparency Inheritance", PlatformAffected.All)]
    public class InputTransparentInheritance : TestNavigationPage
    {
		const string Running = "Running...";
		const string Success = "Success";
		const string Failure = "Failure";
		const string UnderButtonText = "Button";
		const string OverButtonText = "+";
		const string Overlay = "overlay";

		protected override void Init()
		{
			PushAsync(Menu());
		}

		ContentPage Menu()
		{
			var layout = new StackLayout();

			layout.Children.Add(new Label {Text = "Select a test below"});

			layout.Children.Add(MenuButton(true));
			layout.Children.Add(MenuButton(false));

			return new ContentPage { Content = layout };
		}

		Button MenuButton(bool inherited)
		{
			var text = inherited ? "Inherited" : "Not Inherited";
			var button = new Button { Text = text, AutomationId = text };

			button.Clicked += (sender, args) => PushAsync(CreateTestPage(inherited));

			return button;
		}

		ContentPage CreateTestPage(bool inherited)
		{
            var grid = new Grid
            {
                AutomationId = "testgrid",
				HorizontalOptions = LayoutOptions.Fill,
				VerticalOptions = LayoutOptions.Fill
			};

			grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
			grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Star });

			var instructions = new Label
			{
				HorizontalOptions = LayoutOptions.Fill,
				HorizontalTextAlignment = TextAlignment.Center,
				Text = $"Tap the button labeled '{UnderButtonText}'. Then tap the button labeled '{OverButtonText}'."
				       + $" If the label below's text changes to '{Success}' the test has passed."
			};

			grid.Children.Add(instructions);

            var results = new Label 
            { 
                HorizontalOptions = LayoutOptions.Fill,
                HorizontalTextAlignment = TextAlignment.Center, 
                Text = Running 
            };

            grid.Children.Add(results);
            Grid.SetRow(results, 1);

			var underButton = new Button
			{
				Text = UnderButtonText,
				HorizontalOptions = LayoutOptions.Center,
				VerticalOptions = LayoutOptions.Center
			};

			bool overPressed = false;
			bool underPressed = false;
			bool layoutTapped = false;

			underButton.Clicked += (sender, args) =>
			{
				underPressed = true;
				EvaluateTest(results, inherited, overPressed, underPressed, layoutTapped);
			};

			var overButton = new Button
			{
				Text = OverButtonText,
				HorizontalOptions = LayoutOptions.End
			};

			overButton.Clicked += (sender, args) =>
			{
				overPressed = true;
				EvaluateTest(results, inherited, overPressed, underPressed, layoutTapped);
			};

			var layout = new StackLayout
			{
                AutomationId = Overlay,
				HorizontalOptions = LayoutOptions.Fill,
				VerticalOptions = LayoutOptions.Fill,
				InputTransparent = true,
				InputTransparentInherited = inherited,
				BackgroundColor = Color.Blue,
				Opacity = 0.2
			};

			layout.GestureRecognizers.Add(new TapGestureRecognizer()
			{
				Command = new Command(() =>
				{
					layoutTapped = true;
					EvaluateTest(results, inherited, overPressed, underPressed, layoutTapped);
				})
			});

			layout.Children.Add(overButton);

			// Bump up the elevation to cover FastRenderer buttons
			layout.On<Android>().SetElevation(10f);

			grid.Children.Add(underButton);
			Grid.SetRow(underButton, 2);

			grid.Children.Add(layout);
			Grid.SetRow(layout, 2);

			return new ContentPage { Content = grid, Title = inherited.ToString()};
		}

		static void EvaluateTest(Label results, bool inherited, bool overPressed, bool underPressed, bool layoutTapped)
		{
			if (layoutTapped)
			{
				results.Text = Failure;
				return;
			}

			if (inherited)
			{
				if (overPressed)
				{
					results.Text = Failure;
					return;
				}

				if (underPressed)
				{
					results.Text = Success;
					return;
				}
			}
			else
			{
				if (overPressed && underPressed)
				{
					results.Text = Success;
					return;
				}
			}

			results.Text = Running;
		}
	}
}
