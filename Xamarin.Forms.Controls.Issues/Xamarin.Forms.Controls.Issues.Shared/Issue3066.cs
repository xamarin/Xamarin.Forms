using Xamarin.Forms.CustomAttributes;
using Xamarin.Forms.Internals;
#if UITEST
using Xamarin.UITest;
using NUnit.Framework;
using Xamarin.Forms.Core.UITests;
#endif
namespace Xamarin.Forms.Controls.Issues
{
#if UITEST
	[Category(UITestCategories.Layout)]
#endif
	[Preserve(AllMembers = true)]
	[Issue(IssueTracker.Github, 3066, "RTL does't switch the margin", PlatformAffected.All)]
	public class Issue3066 : TestContentPage
	{
		protected override void Init()
		{
			var pageStack = new StackLayout();

			var ltrContentView = new ContentView { VerticalOptions = LayoutOptions.Start };

			var ltrBox = new BoxView
			             {
				             WidthRequest = 50,
				             HeightRequest = 50,
				             HorizontalOptions = LayoutOptions.Start,
				             BackgroundColor = Color.Red,
				             Margin = new Thickness(50, 0, 0, 0)
			             };
			var rtlContentView = new ContentView
			                     {
				                     VerticalOptions = LayoutOptions.Start,
				                     FlowDirection = FlowDirection.RightToLeft
			                     };

			var rtlBox = new BoxView
			             {
				             WidthRequest = 50,
				             HeightRequest = 50,
				             HorizontalOptions = LayoutOptions.Start,
				             BackgroundColor = Color.Red,
				             Margin = new Thickness(50, 0, 0, 0)
			             };

			ltrContentView.Content = ltrBox;
			pageStack.Children.Add(ltrContentView);
			rtlContentView.Content = rtlBox;
			pageStack.Children.Add(rtlContentView);
			Content = pageStack;
		}
	}
}