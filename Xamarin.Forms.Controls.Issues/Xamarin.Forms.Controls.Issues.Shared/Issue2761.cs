using System;

using Xamarin.Forms.CustomAttributes;
using System.Collections.Generic;
using Xamarin.Forms.Internals;

namespace Xamarin.Forms.Controls.Issues
{
	[Preserve(AllMembers = true)]
	[Issue(IssueTracker.Github, 2761, "[Core] Slider unable to update Maximum , Minimum by binding")]
	public class Issue2761 : TestContentPage
	{
		class RangeModel
		{
			public double Max { get; set; }
			public double Min { get; set; }
		}
		protected override void Init()
		{
			Slider slider = new Slider
			{
				HorizontalOptions = LayoutOptions.FillAndExpand,
			};

			var minLabel = new Label { };
			var maxLabel = new Label { };
			var logLabel = new Label { Text = "Log : \n" };
			minLabel.SetBinding(Label.TextProperty, new Binding("Minimum") { StringFormat = "Min : {0:F2}" });
			maxLabel.SetBinding(Label.TextProperty, new Binding("Maximum") { StringFormat = "Max : {0:F2}" });
			minLabel.BindingContext = slider;
			maxLabel.BindingContext = slider;

			slider.SetBinding(Slider.MaximumProperty, new Binding("Max"));
			slider.SetBinding(Slider.MinimumProperty, new Binding("Min"));

			System.Console.WriteLine($"Slider.MaximumProperty.DefaultBindingMode = {Slider.MaximumProperty.DefaultBindingMode}");

			RangeModel range1 = new RangeModel { Min = -100, Max = -1 };
			RangeModel range2 = new RangeModel { Min = 0, Max = 49 };
			RangeModel range3 = new RangeModel { Min = 50, Max = 100 };

			Padding = new Thickness(20);
			Content = new StackLayout
			{
				Children =
				{
					new StackLayout
					{
						Orientation = StackOrientation.Horizontal,
						Children =
						{
							new Button() { Text = "-100 ~ -1", Command = new Command(() => {
								try
								{
									slider.BindingContext = range1;
								}
								catch (Exception e)
								{
									logLabel.Text += e.Message + "\n";
								}
							})},
							new Button() { Text = "0 ~ 49", Command = new Command(() => {
								try
								{
									slider.BindingContext = range2;
								}
								catch (Exception e)
								{
									logLabel.Text += e.Message + "\n";
								}
							})},
							new Button() { Text = "50 ~ 100", Command = new Command(() => {
								try
								{
									slider.BindingContext = range3;
								}
								catch (Exception e)
								{
									logLabel.Text += e.Message  + "\n";
								}
							})},
						}
					},
					new StackLayout
					{
						Orientation = StackOrientation.Horizontal,
						Children = { minLabel, slider, maxLabel },
					},
					logLabel,
				}
			};
		}
	}
}
