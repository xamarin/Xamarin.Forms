using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms.CustomAttributes;
using Xamarin.Forms.Internals;
using Xamarin.Forms.PlatformConfiguration;
using Xamarin.Forms.PlatformConfiguration.AndroidSpecific;

namespace Xamarin.Forms.Controls.Issues
{
	[Preserve(AllMembers = true)]
	[Issue(IssueTracker.Github, 3083, "Label Visibility on TabbedPage", PlatformAffected.Android)]
	class Issue3083 : TestTabbedPage
	{
		protected override void Init()
		{
			this.Children.Add(new Issue1900 { IconImageSource = "bank" });
			this.Children.Add(new Issue1900 { IconImageSource = "bank" });
			this.On<Android>().SetBottomToolBarLabelVisibilityMode(BottomToolBarLabelVisibilityMode.Labeled);
			this.On<Android>().SetToolbarPlacement(ToolbarPlacement.Bottom);
		}
	}
}
