using System;
using System.Collections.Generic;
using Xamarin.Forms.CustomAttributes;

#if UITEST
using Xamarin.UITest;
using NUnit.Framework;
using Xamarin.Forms.Core.UITests;
#endif

namespace Xamarin.Forms.Controls.Issues
{
	[Issue(IssueTracker.Github, 12760,
		"[Bug] Button event Clicked inside SwipeView inside ListView happens at the same time with ListView event ItemTapped",
		PlatformAffected.Android)]
	public partial class Issue12760 : TestContentPage
	{
		public Issue12760()
		{
#if APP
			InitializeComponent();
#endif
		}

		protected override void Init()
		{
		}
#if APP
		void OnItemTapped(object sender, ItemTappedEventArgs e)
		{
			DisplayAlert("Issue 12760", "Item Tapped", "Ok");
		}

		void OnButtonClicked(object sender, EventArgs e)
		{
			DisplayAlert("Issue 12760", "Button Clicked", "Ok");
		}
#endif
	}
}