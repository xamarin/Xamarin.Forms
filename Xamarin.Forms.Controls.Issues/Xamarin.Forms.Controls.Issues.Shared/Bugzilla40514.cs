using Xamarin.Forms.CustomAttributes;
using Xamarin.Forms.Internals;
using System;

#if UITEST
using Xamarin.UITest;
using NUnit.Framework;
#endif

namespace Xamarin.Forms.Controls.Issues
{
	[Preserve(AllMembers = true)]
	[Issue(IssueTracker.Bugzilla, 40514, "[WinRT] InputTransparent does not work if the background color not explicitly set")]
	public class Bugzilla40514 : TestContentPage
	{
		protected override void Init()
		{
			var parentGrid = new Grid
			{
				VerticalOptions = LayoutOptions.FillAndExpand
			};

			//Child at back of the UI
			var btnBack = new Button();
			btnBack.Text = "Should Not Be Clickable";
			btnBack.Clicked += BtnBack_Clicked;
			btnBack.TranslationX = -75;
			btnBack.HeightRequest = 50;
			btnBack.VerticalOptions = LayoutOptions.Center;
			btnBack.HorizontalOptions = LayoutOptions.Center;
			parentGrid.Children.Add(btnBack);

			//Visual element
			var rectangle = new Grid();
			rectangle.Opacity = 0.5;
			rectangle.InputTransparent = false;
			parentGrid.Children.Add(rectangle);

			//Child at back of the UI
			var btnFront = new Button();
			btnFront.Text = "Toggle Background";
			btnFront.Command = new Command(() => rectangle.BackgroundColor = rectangle.BackgroundColor == Color.Blue ? Color.Default : Color.Blue);
			btnFront.TranslationX = 75;
			btnFront.HeightRequest = 50;
			btnFront.VerticalOptions = LayoutOptions.Center;
			btnFront.HorizontalOptions = LayoutOptions.Center;
			parentGrid.Children.Add(btnFront);
			
			Content = new StackLayout
			{
				Children =
				{
					parentGrid,
					new Button
					{
						Text = "Toggle InputTransparent",
						Command = new Command(() => rectangle.InputTransparent = !rectangle.InputTransparent)
					}
				}
			};
		}

		void BtnBack_Clicked(object sender, EventArgs e)
		{
			throw new NotImplementedException();
		}
	}
}