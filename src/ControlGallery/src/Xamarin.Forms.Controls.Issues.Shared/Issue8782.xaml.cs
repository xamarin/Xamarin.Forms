﻿using System;
using System.Collections.Generic;
using Microsoft.Maui.Controls.CustomAttributes;
using Microsoft.Maui.Controls.Internals;
using Microsoft.Maui.Controls.Xaml;

#if UITEST
using Xamarin.UITest;
using Xamarin.UITest.Queries;
using NUnit.Framework;
using Microsoft.Maui.Controls.UITests;
#endif

namespace Microsoft.Maui.Controls.ControlGallery.Issues
{
#if UITEST
	[Category(UITestCategories.SwipeView)]
#endif
#if APP
	[XamlCompilation(XamlCompilationOptions.Compile)]
#endif
	[Preserve(AllMembers = true)]
	[Issue(IssueTracker.Github, 8782, "[Bug] SwipeViewItems cut off on one or more sides", PlatformAffected.All)]
	public partial class Issue8782 : TestContentPage
	{
		public Issue8782()
		{
#if APP
			Title = "Issue 8782";
			InitializeComponent();
#endif
		}

		protected override void Init()
		{

		}

		async void OnIncorrectAnswerInvoked(object sender, EventArgs e)
		{
			await DisplayAlert("Incorrect!", "Try again.", "OK");
		}

		async void OnCorrectAnswerInvoked(object sender, EventArgs e)
		{
			await DisplayAlert("Correct!", "The answer is 2.", "OK");
		}
	}
}