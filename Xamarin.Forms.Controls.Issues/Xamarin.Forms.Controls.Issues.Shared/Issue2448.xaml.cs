using System;
using Xamarin.Forms.CustomAttributes;
using Xamarin.Forms.Internals;

namespace Xamarin.Forms.Controls.Issues
{
	[Preserve(AllMembers = true)]
	[Issue(IssueTracker.Github, 2448, "Setting FlowDirection of Alerts and ActionSheets", PlatformAffected.iOS | PlatformAffected.Android)]

	public partial class Issue2448 : ContentPage
	{
		public Issue2448()
		{
#if APP
			InitializeComponent();
#endif
		}

		void Button1_Clicked(object sender, EventArgs args)
		{
#if APP
			var alert = DisplayAlert("Alert", "You have been alerted", "OK");
#endif
		}

		void Button2_Clicked(object sender, EventArgs args)
		{
#if APP
			var alert = DisplayAlert("Alert", "You have been alerted", "OK", FlowDirection.MatchParent);
#endif
		}

		void Button3_Clicked(object sender, EventArgs args)
		{
#if APP
			var alert = DisplayAlert("Alert", "You have been alerted", "OK", FlowDirection.RightToLeft);
#endif
		}

		void Button4_Clicked(object sender, EventArgs args)
		{
#if APP
			var alert = DisplayAlert("Alert", "You have been alerted", "OK", FlowDirection.LeftToRight);
#endif
		}

		void Button5_Clicked(object sender, EventArgs args)
		{
#if APP
			var alert = DisplayActionSheet("ActionSheet: SavePhoto?", "Cancel", "Delete", "Photo Roll", "Email");
#endif
		}

		void Button6_Clicked(object sender, EventArgs args)
		{
#if APP
			var alert = DisplayActionSheet("ActionSheet: SavePhoto?", "Cancel", "Delete", FlowDirection.MatchParent, "Photo Roll", "Email");
#endif
		}

		void Button7_Clicked(object sender, EventArgs args)
		{
#if APP
			var alert = DisplayActionSheet("ActionSheet: SavePhoto?", "Cancel", "Delete", FlowDirection.RightToLeft, "Photo Roll", "Email");
#endif
		}

		void Button8_Clicked(object sender, EventArgs args)
		{
#if APP
			var alert = DisplayActionSheet("ActionSheet: SavePhoto?", "Cancel", "Delete", FlowDirection.LeftToRight, "Photo Roll", "Email");
#endif
		}
	}
}