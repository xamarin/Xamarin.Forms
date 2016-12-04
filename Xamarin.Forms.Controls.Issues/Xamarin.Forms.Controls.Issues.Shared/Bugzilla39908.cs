using System;

using Xamarin.Forms.CustomAttributes;
using Xamarin.Forms.Internals;

#if UITEST
using Xamarin.UITest;
using NUnit.Framework;
#endif

namespace Xamarin.Forms.Controls
{
	[Preserve(AllMembers = true)]
	[Issue(IssueTracker.Bugzilla, 39908, " Back button hit quickly results in jumbled pages", PlatformAffected.iOS)]
	public class Bugzilla39908 : TestNavigationPage // or TestMasterDetailPage, etc ...
	{
		protected override void Init()
		{
			PushAsync(new ContentPage39908());
		}
	}

	[Preserve(AllMembers = true)]
	public class ContentPage39908 : ContentPage
	{
		public ContentPage39908()
		{
			string label = "Page " + Navigation.NavigationStack.Count;

			var button = new Button
			{
				Text = "Another one",
				AutomationId = "Another one"
			};
			button.Clicked += AddNewPage;

			Title = label;
			Content = new StackLayout
			{
				VerticalOptions = LayoutOptions.Center,
				Children =
				{
					new Label
					{
						HorizontalTextAlignment = TextAlignment.Center,
						Text = label
					},
					button
				}
			};
		}

		void AddNewPage(object sender, EventArgs e)
		{
			Navigation.PushAsync(new ContentPage39908());
		}
	}
}