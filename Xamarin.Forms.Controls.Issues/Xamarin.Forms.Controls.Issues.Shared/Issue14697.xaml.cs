using System;
using System.Collections.Generic;
using Xamarin.Forms;
using Xamarin.Forms.CustomAttributes;
using Xamarin.Forms.Internals;

namespace Xamarin.Forms.Controls.Issues
{
	[Preserve(AllMembers = true)]
	[Issue(IssueTracker.Github, 14697, "[Bug][Android] Shape no longer redrawing itself when the container changes size (sr4 and sr5)", PlatformAffected.iOS)]
	public partial class Issue14697 : ContentPage
	{
		public Issue14697()
		{
#if APP
			InitializeComponent();
#endif
		}
	}

}