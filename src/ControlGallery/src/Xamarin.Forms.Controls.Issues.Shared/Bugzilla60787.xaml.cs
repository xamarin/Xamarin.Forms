﻿using System;
using Microsoft.Maui.Controls.CustomAttributes;
using Microsoft.Maui.Controls.Internals;
using Microsoft.Maui.Controls.Xaml;

namespace Microsoft.Maui.Controls.ControlGallery.Issues
{
#if APP
#if UITEST
		[NUnit.Framework.Category(Core.UITests.UITestCategories.Bugzilla)]
#endif
	[Preserve(AllMembers = true)]
	[Issue(IssueTracker.Bugzilla, 60787, "Frames with border radius preset have this radius reset when their background color is changed.",
		PlatformAffected.Android, issueTestNumber: 1)]
	public partial class Bugzilla60787 : ContentPage
	{
		bool _colourIndicator;

		public Bugzilla60787()
		{
			InitializeComponent();

			this.btnChangeColour.Clicked += btnChangeColour_Click;
		}

		void btnChangeColour_Click(object sender, EventArgs e)
		{
			this.frmDoesChange.BackgroundColor = _colourIndicator ? Color.LightBlue : Color.LightGoldenrodYellow;

			_colourIndicator = !_colourIndicator;
		}
	}
#endif
}
