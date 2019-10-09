using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms.CustomAttributes;
using Xamarin.Forms.Internals;

namespace Xamarin.Forms.Controls.Issues
{
#if APP
	[Preserve(AllMembers = true)]
	[Issue(IssueTracker.Github, 7886, "PushModalAsync modal page with Entry crashes on close for MacOS (NRE)", PlatformAffected.macOS)]
	public partial class Issue7886 : ContentPage
	{
		public Issue7886()
		{
			InitializeComponent();
		}

		void Handle_Clicked(object sender, EventArgs e)
		{
			Navigation.PushModalAsync(new NavigationPage(new ModalPage()));
		}

		class ModalPage : ContentPage
		{
			public ModalPage()
			{
				BackgroundColor = Color.Orange;
				ToolbarItems.Add(new ToolbarItem("Done", null, () => Navigation.PopModalAsync()));

				Content = new Entry
				{
					HorizontalOptions = LayoutOptions.Center,
					VerticalOptions = LayoutOptions.Center
				};
			}
		}
	}
#endif
}