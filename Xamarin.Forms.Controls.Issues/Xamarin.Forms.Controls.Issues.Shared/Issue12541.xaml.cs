﻿using System;
using System.Diagnostics;
using Xamarin.Forms.CustomAttributes;
using Xamarin.Forms.Internals;

#if UITEST
using Xamarin.UITest;
using NUnit.Framework;
using Xamarin.Forms.Core.UITests;
#endif

namespace Xamarin.Forms.Controls.Issues
{
#if UITEST
	[Category(UITestCategories.SwipeView)]
#endif
	[Preserve(AllMembers = true)]
	[Issue(IssueTracker.Github, 12541, "[Bug] Android Swipeview finicky swipe to close", PlatformAffected.Android | PlatformAffected.iOS)]
	public partial class Issue12541 : ContentPage
	{
		public Issue12541()
		{
#if APP
			InitializeComponent();
#endif
		}

#if APP
		void OnSwipeStarted(object sender, SwipeStartedEventArgs e)
		{
			Debug.WriteLine("Swipe started");
		}

		void OnSwipeEnded(object sender, SwipeEndedEventArgs e)
		{
			Debug.WriteLine("Swipe ended");
		}

		void OnSwipeViewTapped(object sender, EventArgs e)
		{
			Debug.WriteLine("SwipeView tapped");
		}

		void OnSwipeViewContentTapped(object sender, EventArgs e)
		{
			Debug.WriteLine("SwipeView content tapped");
		}

		void OnSwipeItem1Tapped(object sender, EventArgs e)
		{
			Debug.WriteLine("SwipeItem 1 tapped");
		}

		void OnSwipeItem2Tapped(object sender, EventArgs e)
		{
			Debug.WriteLine("SwipeItem 2 tapped");
		}
#endif
	}
}