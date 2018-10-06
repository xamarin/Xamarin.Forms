using Xamarin.Forms.CustomAttributes;
using Xamarin.Forms.Internals;
using Xamarin.Forms.PlatformConfiguration;
using Xamarin.Forms.PlatformConfiguration.WindowsSpecific;

#if UITEST
using Xamarin.Forms.Core.UITests;
using Xamarin.UITest;
using NUnit.Framework;
#endif

namespace Xamarin.Forms.Controls.Issues
{
#if UITEST
	[Category(UITestCategories.ManualReview)]
#endif
	[Preserve(AllMembers = true)]
	[Issue(IssueTracker.Github, 3988, "Windows Platform Specific - IsDynamicOverflowEnabled", PlatformAffected.UWP)]
	public class Issue3988 : TestNavigationPage // or TestMasterDetailPage, etc ...
	{
		readonly StackLayout _stackLayout = new StackLayout();

		protected override void Init()
		{			
			var button = new Button() { Text = "Toggle IsDynamicOverflowEnabled" };
			button.Clicked += Button_Clicked;
			var text = new Label()
			{
				Text =
					"Shrink the app window size so that not all toolbar items are visible. By default, items should overflow to secondary menu, but this can be disabled by clicking the toggle button",
				LineBreakMode = LineBreakMode.WordWrap
			};
			_stackLayout.Children.Add(text);
			_stackLayout.Children.Add(button);			
			Navigation.PushAsync(new ContentPage
			{
				Content = _stackLayout
			});

			for (int i = 0; i < 10; i++)
			{
				ToolbarItems.Add(new ToolbarItem("Test", "coffee.png", () => { }));
			}
			
		}

		void Button_Clicked(object sender, System.EventArgs e)
		{
			var overflowEnabled = On<Windows>().GetToolbarDynamicOverflowEnabled();
			On<Windows>().SetToolbarDynamicOverflowEnabled(!overflowEnabled);
		}
	}
}