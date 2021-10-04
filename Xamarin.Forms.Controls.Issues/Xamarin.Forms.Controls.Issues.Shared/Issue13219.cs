using System;
using Xamarin.Forms.CustomAttributes;
using Xamarin.Forms.Internals;

namespace Xamarin.Forms.Controls.Issues
{
	[Preserve(AllMembers = true)]
	[Issue(IssueTracker.Github, 13219, "Flyout item navigation behaves different on previous navigating", PlatformAffected.All)]
	public partial class Issue13219 : TestShell
	{
		public Issue13219()
		{

		}

		protected override void Init()
		{
			var aboutPage = AddContentPage<FlyoutItem, Tab>(new AboutPage_Issue13219(), "About Page");
			var browsePage = AddContentPage<FlyoutItem, Tab>(new BrowsePage_Issue13219(), "Browse Page");
			browsePage.Route = "browse";
			CurrentItem = aboutPage;
		}

		[Preserve(AllMembers = true)]
		class AboutPage_Issue13219 : ContentPage
		{
			public AboutPage_Issue13219()
			{
				var button = new Button
				{
					Text = "Navigate GoToAsync Browse Page"

				};
				button.Clicked += (_, __) => Current.GoToAsync("//browse?pageId=1");

				var label = new Label
				{
					Text = "Tap on button below and you should navigate to the Browse page. \n\n You should see the number 1 when you navigate. \n\n If you navigate using the Flyout you should the message \"value is null\" ",
					FontSize = 20,
					HorizontalTextAlignment = TextAlignment.Center,
					TextColor = Color.Black
				};

				var stack = new StackLayout();
				stack.Children.Add(label);
				stack.Children.Add(button);
				Content = stack;
			}
		}
	}


	[Preserve(AllMembers = true)]
	[QueryProperty("PageId", "pageId")]
	class BrowsePage_Issue13219 : ContentPage
	{
		int holder = 0;
		Label lblValue;

		string id = "";
		public string PageId
		{
			get { return id; }
			set
			{
				if (string.IsNullOrEmpty(value))
				{
					SetLabelValue(ref value);
					return;
				}

				int.TryParse(Uri.UnescapeDataString(value), out holder);
				SetLabelValue(ref value);
			}
		}

		void SetLabelValue(ref string value)
		{
			lblValue.Text = "";
			lblValue.Text = value ?? "value is null";
		}

		public BrowsePage_Issue13219()
		{
			var description = new Label
			{
				Text = "This is the Browse page. \n If you open the FlyoutMenu and tap again on the Browse FlyoutItem the set method shouldn't be called and the value of Id property should be the same",
				TextColor = Color.Black
			};

			lblValue = new Label
			{
				Text = PageId,
				VerticalOptions = LayoutOptions.CenterAndExpand,
				TextColor = Color.Black,
				HorizontalTextAlignment = TextAlignment.Center,
				FontAttributes = FontAttributes.Bold,
				FontSize = 50
			};

			var stck = new StackLayout();
			stck.Children.Add(description);
			stck.Children.Add(lblValue);
			Content = stck;
			string x = null;
			SetLabelValue(ref x);
		}
	}
}