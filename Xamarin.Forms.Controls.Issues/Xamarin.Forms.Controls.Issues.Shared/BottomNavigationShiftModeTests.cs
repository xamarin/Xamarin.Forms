using Xamarin.Forms.CustomAttributes;
using Xamarin.Forms.Internals;
using Xamarin.Forms.PlatformConfiguration;
using Xamarin.Forms.PlatformConfiguration.AndroidSpecific;

#if UITEST
using NUnit.Framework;
using Xamarin.UITest;
#endif

namespace Xamarin.Forms.Controls.Issues
{
	[Preserve(AllMembers = true)]
	[Issue(IssueTracker.Github, 3083, "Implement fixed mode for Bottom Navigation Bar Android", PlatformAffected.Android)]
	public class BottomNavigationShiftModeTests : TestTabbedPage
	{
		protected override void Init()
		{
			On<Android>().SetToolbarPlacement(ToolbarPlacement.Bottom);
			var toolbarShiftmode = ToolbarShiftMode.Fixed;
			var toolbarItemShiftmode = ToolbarShiftMode.Fixed;
			On<Android>().SetBottomToolbarShiftMode(toolbarShiftmode, toolbarItemShiftmode);

			Children.Add(new ContentPage() { Title = "Page 1", Content = new Label(), Icon = "coffee.png" });
			Children.Add(new ContentPage() { Title = "Page 2", Content = new Label(), Icon = "bank.png" });

			var managedContentPage = new ContentPage()
			{
				Title = "Test",
				Icon = "calculator.png",
				Content = new StackLayout()
				{
					Children =
					{
						new Button()
						{
							Text = "Swap shift mode toolbar",
							Command = new Command(()=>
							{
								toolbarShiftmode = toolbarShiftmode == ToolbarShiftMode.Shifted ?
														ToolbarShiftMode.Fixed : ToolbarShiftMode.Shifted;
								On<Android>().SetBottomToolbarShiftMode(toolbarShiftmode, toolbarItemShiftmode);
							})
						},
						new Button()
						{
							Text = "Swap shift mode item toolbar",
							Command = new Command(()=>
							{
								toolbarItemShiftmode = toolbarItemShiftmode == ToolbarShiftMode.Shifted ?
														ToolbarShiftMode.Fixed : ToolbarShiftMode.Shifted;
								On<Android>().SetBottomToolbarShiftMode(toolbarShiftmode, toolbarItemShiftmode);
							})
						}
					}
				}
			};

			Children.Add(managedContentPage);

			Children.Add(new ContentPage() { Title = "Page 4", Content = new Label(), Icon = "bank.png" });
			Children.Add(new ContentPage() { Title = "Page 5", Content = new Label(), Icon = "coffee.png" });
		}

#if UITEST && __ANDROID__
		[Test]
		public void BottomNavigationShiftModes()
		{
			RunningApp.Tap(q => q.Marked("Test"));
			RunningApp.Screenshot("ShiftModes Fixed&Fixed");

			RunningApp.Tap(q => q.Marked("Swap shift mode toolbar"));
			RunningApp.Screenshot("ShiftModes Default&Fixed");

			RunningApp.Tap(q => q.Marked("Swap shift mode toolbar"));
			RunningApp.Tap(q => q.Marked("Swap shift mode item toolbar"));
			RunningApp.Screenshot("ShiftModes Fixed&Default");

			RunningApp.Tap(q => q.Marked("Swap shift mode toolbar"));
			RunningApp.Screenshot("ShiftModes Default&Default");
		}
#endif
	}
}
