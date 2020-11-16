using System;
using System.Collections.Generic;
using Xamarin.Forms.CustomAttributes;
using Xamarin.Forms.Internals;

#if UITEST
using Xamarin.Forms.Core.UITests;
using NUnit.Framework;
#endif

namespace Xamarin.Forms.Controls.Issues
{
	[Preserve(AllMembers = true)]
	[Issue(IssueTracker.Github, 12848, "[Bug] CarouselView position resets when visibility toggled",
		PlatformAffected.Android)]
	public partial class Issue12848 : TestContentPage
	{
		protected override void Init()
		{
#if APP
			InitializeComponent();

			BindingContext = new List<int> { 1, 2, 3 };
#endif
		}

#if APP
		void OnShowButtonClicked(object sender, EventArgs e)
		{
			carousel.IsVisible = true;
		}

		void OnHideButtonClicked(object sender, EventArgs e)
		{
			carousel.IsVisible = false;
		}
#endif
	}
}