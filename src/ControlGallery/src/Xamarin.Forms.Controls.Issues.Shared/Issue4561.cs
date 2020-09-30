﻿using Xamarin.Forms.CustomAttributes;
using Xamarin.Forms.Internals;

namespace Xamarin.Forms.Controls.Issues
{
	[Preserve(AllMembers = true)]
	[Issue(IssueTracker.Github, 4561, "Keyboard navigation does not work", PlatformAffected.Android)]
	public class Issue4561 : TestContentPage
	{
		[Preserve(AllMembers = true)]
		public class CustomView : View
		{
		}

		protected override void Init()
		{
			Content = new StackLayout
			{
				Children = {
					new Label { Text = "Select any editbox below and try using the TAB key navigation. Focus should change from one editbox to another" },
					new CustomView()
				}
			};
		}
	}
}
