using System;
using System.Collections.Generic;
using Xamarin.Forms;
using Xamarin.Forms.CustomAttributes;
using Xamarin.Forms.Internals;

namespace Xamarin.Forms.Controls.Issues
{
	[Preserve(AllMembers = true)]
	[Issue(IssueTracker.Github, 14801, "[Bug] RadialGradientBrush behavior is wrong on UWP", PlatformAffected.UWP)]
	public partial class Issue14801 : ContentPage
	{
		public Issue14801()
		{
#if APP
			InitializeComponent();
#endif
		}
	}

}