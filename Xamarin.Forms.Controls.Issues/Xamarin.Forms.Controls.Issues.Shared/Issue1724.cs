using Xamarin.Forms.CustomAttributes;
using Xamarin.Forms.Internals;
using System;
using System.Linq;
using System.Threading;
using System.Collections.Generic;

namespace Xamarin.Forms.Controls.Issues
{
	[Preserve(AllMembers = true)]
	[Issue(IssueTracker.Github, 1724, "[Enhancement] ImageButton ", PlatformAffected.All)]
	public class Issue1724 : TestContentPage
	{
		bool animation = false;

		protected override void Init()
		{
			StackLayout layout = new StackLayout()
			{
				Children =
				{
					new Label(){ Text = "No padding?" },
					new ImageButton()
					{
						Source = "coffee.png",
						BackgroundColor = Color.GreenYellow
					},
					new Label(){ Text = "Do I have left padding? I should have left padding." },
					new ImageButton()
					{
						Source = "coffee.png",
						BackgroundColor = Color.Green,
						Padding = new Thickness(100, 0, 0, 0)
					},
					new Label(){ Text = "Do I have top padding? I should have top padding." },
					new ImageButton()
					{
						Source = "coffee.png",
						BackgroundColor = Color.LawnGreen,
						Padding = new Thickness(0, 30, 0, 0)
					},
					new Label(){ Text = "Do I have right padding? I should have right padding."},
					new ImageButton()
					{
						Source = "coffee.png",
						BackgroundColor = Color.LightGreen,
						Padding = new Thickness(0, 0, 100, 0)
					},
					new Label(){ Text = "Do I have bottom padding? I should have bottom padding." },
					new ImageButton()
					{
						Source = "coffee.png",
						BackgroundColor = Color.ForestGreen,
						Padding = new Thickness(0, 0, 0, 30)
					},
					new Label(){ Text = "Do you see image from a Uri?" },
					new ImageButton()
					{
						Source = "https://raw.githubusercontent.com/xamarin/Xamarin.Forms/master/Xamarin.Forms.Controls/coffee.png",
						BackgroundColor = Color.ForestGreen
					},
					new Label(){ Text = "Invalid Image Uri just to test it doesn't crash" },
					new ImageButton()
					{
						Source = "http://xamarin.com/imginvalidf@#$R(P&fb.png",
						BackgroundColor = Color.ForestGreen
					}
				}
			};

			var buttons = layout.Children.OfType<ImageButton>();
			layout.Children.Insert(0, ActionGrid(buttons.ToList()));
			PaddingAnimation(buttons).Start();

			Content = new ScrollView() { Content = layout };
		}

		Grid ActionGrid(List<ImageButton> buttons)
		{
			ImageButton firstButton = buttons.FirstOrDefault();
			Grid actionGrid = new Grid();
			actionGrid.AddChild(new Button()
			{
				Text = "Add Right",
				Command = new Command(() =>
				{
					var button = firstButton;
					button.Padding = new Thickness(button.Padding.Left, 0, button.Padding.Right + 10, 0);
				})
			}, 0, 0);
			actionGrid.AddChild(new Button()
			{
				Text = "Add Left",
				Command = new Command(() =>
				{
					var button = firstButton;
					button.Padding = new Thickness(button.Padding.Left + 10, 0, button.Padding.Right, 0);
				})
			}, 0, 1);

			actionGrid.AddChild(new Button()
			{
				Text = "Animation",
				Command = new Command(() => animation = !animation)
			}, 1, 1);
			actionGrid.AddChild(new Button()
			{
				Text = "Add Top",
				Command = new Command(() =>
				{
					var button = firstButton;
					button.Padding = new Thickness(0, button.Padding.Top + 10, 0, button.Padding.Bottom);
				})
			}, 2, 0);
			actionGrid.AddChild(new Button()
			{
				Text = "Add Bottom",
				Command = new Command(() =>
				{
					var button = firstButton;
					button.Padding = new Thickness(0, button.Padding.Top, 0, button.Padding.Bottom + 10);
				})
			}, 2, 1);
			return actionGrid;
		}

		Thread PaddingAnimation(IEnumerable<ImageButton> buttons)
		{
			return new Thread(() =>
			{
				int increment = 1;
				int current = 0;
				int max = 15;
				int FPS = 30;
				int sleep = 1000 / FPS;

				while (true)
				{
					Thread.Sleep(sleep);

					if (!animation)
						continue;

					current += increment;
					if (current > max || current < 0)
					{
						increment *= -1;
						current += increment * 2;
					}

					Device.BeginInvokeOnMainThread(() =>
					{
						foreach (var button in buttons)
						{
							var padding = button.Padding;
							button.Padding = padding = new Thickness(
								padding.Left + increment,
								padding.Top + increment,
								padding.Right + increment,
								padding.Bottom + increment);
						}
					});
				}
			});
		}
	}
}