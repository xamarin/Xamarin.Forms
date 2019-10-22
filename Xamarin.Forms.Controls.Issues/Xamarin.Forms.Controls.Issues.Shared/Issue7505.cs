using System.Collections.Generic;
using System.Linq;
using Xamarin.Forms.Controls.Issues;
using Xamarin.Forms.CustomAttributes;
using Xamarin.Forms.Internals;

namespace Xamarin.Forms.Controls.Issues
{
	[Preserve (AllMembers=true)]
	[Issue (IssueTracker.Github, 7505, "MasterDetailPage detail width broken when landscape", PlatformAffected.UWP)]
	public class Issue7505 : MasterDetailPage
	{
		public Issue7505()
		{
			Master = new ContentPage { Title = "master" };
			Detail = CreateDetailPage("Don't look here, look at the toolbar!");
		}

		static Page CreateDetailPage(string text)
		{
			var page = new ContentPage {
				Title = text,
				Content = new StackLayout {
					Children = {
						new Label { 
							Text = text,
							VerticalOptions = LayoutOptions.CenterAndExpand,
							HorizontalOptions = LayoutOptions.CenterAndExpand,
						}
					}
				}
			};

			var tbiBank = new ToolbarItem { Command = new Command (() => { }), IconImageSource = "bank.png" };
			var tbiCalc = new ToolbarItem { Command = new Command (() => { }), IconImageSource = "calculator.png" };
			var tbiXam = new ToolbarItem { Command = new Command (() => { }), IconImageSource = "xamarinlogo.png" };
			var tbiXamSecondary = new ToolbarItem { Command = new Command (() => { }), IconImageSource = "xamarinlogo.png", Order = ToolbarItemOrder.Secondary };
			var tbiCalcSecondary = new ToolbarItem { Command = new Command(() => { }), IconImageSource = "calculator.png", Order = ToolbarItemOrder.Secondary };


			page.ToolbarItems.Add (tbiBank);
			page.ToolbarItems.Add (tbiCalc);
			page.ToolbarItems.Add (tbiXam);
			page.ToolbarItems.Add (tbiXamSecondary);
			page.ToolbarItems.Add (tbiCalcSecondary);

			return new NavigationPage (page);
		}
	}
}
