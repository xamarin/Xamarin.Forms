﻿using System;
using System.Linq;
using Xamarin.Forms.CustomAttributes;
using Xamarin.Forms.Internals;

#if UITEST
using NUnit.Framework;
using Xamarin.UITest;
using Xamarin.Forms.Core.UITests;
#endif

namespace Xamarin.Forms.Controls.Issues
{
	[Preserve (AllMembers = true)]
	[Issue (IssueTracker.Github, 764, "Keyboard does not dismiss on SearchBar", PlatformAffected.Android)]
	public class Issue764 : TestContentPage
	{
		protected override void Init ()
		{
			var instructions = new Label { Text = "Tap the SearchBar. Type something into it with the software " +
				"keyboard. Tap the 'Search' button on the keyboard. The software keyboard should be dismissed. If " +
				"the software keyboard is still visible, this test has failed." };

			Title = "Issue 764";

			var searchBar = new SearchBar {
				Placeholder = "Search Me!"
			};

			var label = new Label {
				Text = "Pending Search"
			};

			searchBar.SearchButtonPressed += (s, e) => label.Text = "Search Activated";

			var layout = new StackLayout { 
				Children =  {
					searchBar,
					label,
					instructions
				}
			};

			Content = layout;
		}
    }
}
