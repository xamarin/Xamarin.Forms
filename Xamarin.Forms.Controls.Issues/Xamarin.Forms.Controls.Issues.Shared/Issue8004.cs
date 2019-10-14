using System;
using Xamarin.Forms.CustomAttributes;
using Xamarin.Forms.Internals;
using System.Threading.Tasks;

#if UITEST
using Xamarin.UITest;
using NUnit.Framework;
using Xamarin.Forms.Core.UITests;
#endif

namespace Xamarin.Forms.Controls.Issues
{
#if UITEST
	[Category(UITestCategories.Animation)]
#endif
	[Preserve(AllMembers = true)]
	[Issue(IssueTracker.Github, 8004, "Add a ScaleXTo and ScaleYTo animation extension method", PlatformAffected.All)]
	public class Issue8004 : TestContentPage
	{
		BoxView _boxView;
		const string AnimateBoxView = "AnimateBoxView";

		protected override void Init()
		{
			var button = new Button
			{
				AutomationId = AnimateBoxView,
				Text = "Animate BoxView",
				BackgroundColor = Color.Black,
				TextColor = Color.White,
				VerticalOptions = LayoutOptions.EndAndExpand
			};

			button.Clicked += AnimateButton_Clicked;

			_boxView = new BoxView
			{
				BackgroundColor = Color.Blue,
				WidthRequest = 200,
				HeightRequest = 100,
				HorizontalOptions = LayoutOptions.Center
			};

			var anotherBoxView = new BoxView
			{
				BackgroundColor = Color.Red,
				WidthRequest = 200,
				HeightRequest = 100,
				Scale = 1,
				Opacity = 0.5,
				HorizontalOptions = LayoutOptions.Center
			};

			var grid = new Grid();

			Grid.SetRow(anotherBoxView, 1);
			Grid.SetRow(button, 2);
					   
			grid.Children.Add(_boxView);
			grid.Children.Add(anotherBoxView);
			grid.Children.Add(button);

			Content = grid;
		}

		void AnimateButton_Clicked(object sender, EventArgs e)
		{
			_boxView.ScaleYTo(2, 250, Easing.CubicInOut);
			_boxView.ScaleXTo(1.5, 400, Easing.BounceOut);
		}

#if UITEST
		[Test]
		public async Task AnimateScaleOfBoxView()
		{
			RunningApp.WaitForElement(AnimateBoxView);
			RunningApp.Tap(AnimateBoxView);

			await Task.Delay(500);

			Assert.AreEqual(_boxView.ScaleX, 1.5);
			Assert.AreEqual(_boxView.ScaleY, 2);
		}
#endif
	}
}
