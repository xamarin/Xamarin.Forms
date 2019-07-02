using System;
using System.Collections.Generic;
using Xamarin.Forms.CustomAttributes;
using Xamarin.Forms.Internals;
using Xamarin.Forms.Xaml;

namespace Xamarin.Forms.Controls.Issues
{
#if APP
	[Preserve(AllMembers = true)]
	[Issue(IssueTracker.Github, 5046, "CornerRadius doesn't work in explicit style when implicit style exists", 
	       PlatformAffected.Android | PlatformAffected.iOS)]
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class Issue5046 : ContentPage
	{
		public Issue5046()
		{
			InitializeComponent();
		}

		void Button_Clicked(object sender, System.EventArgs e)
		{
			this.DisplayAlert("CornerRadius", "CornerRadius = " + ((Button)sender).CornerRadius, "OK");
		}
	}
#endif
}