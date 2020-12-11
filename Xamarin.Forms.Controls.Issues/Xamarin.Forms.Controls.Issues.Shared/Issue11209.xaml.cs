﻿using System;
using System.Collections.Generic;
using Xamarin.Forms.CustomAttributes;
using Xamarin.Forms.Internals;
using Xamarin.Forms.Xaml;

#if UITEST
using Xamarin.UITest;
using Xamarin.UITest.Queries;
using NUnit.Framework;
using Xamarin.Forms.Core.UITests;
using System.Linq;
#endif

namespace Xamarin.Forms.Controls.Issues
{
#if UITEST
	[Category(UITestCategories.SwipeView)]
#endif
#if APP
	[XamlCompilation(XamlCompilationOptions.Compile)]
#endif
	[Preserve(AllMembers = true)]
	[Issue(IssueTracker.Github, 11209, "[Bug] [iOS][SwipeView] Swipe view not handling tap gesture events until swiped", PlatformAffected.Android)]
	public partial class Issue11209 : TestContentPage
	{
		const string SwipeViewContent = "SwipeViewContent";
		const string Success = "Success";

		public Issue11209()
		{
#if APP
			InitializeComponent();
#endif
		}

		public List<string> Items => new List<string> { "short", "long word", "Extra long word", "word up" };

		protected override void Init()
		{

		}

#if APP
		void SwipeItem_Invoked(object sender, EventArgs e)
		{
			Console.WriteLine("Hey i was invoked");
		}

		async void TapGestureRecognizer_Tapped(object sender, EventArgs e)
		{
			await Navigation.PushAsync(new Issue11209SecondPage());
		}
#endif

#if UITEST
		[Category(UITestCategories.SwipeView)]
		[Test]
		public void TapSwipeViewAndNavigateTest()
		{
			RunningApp.WaitForElement(SwipeViewContent);
			RunningApp.Tap(SwipeViewContent);
			RunningApp.WaitForElement(Success);
		}
#endif

		[Preserve(AllMembers = true)]
		public class Issue11209SecondPage : ContentPage
		{
			public Issue11209SecondPage()
			{
				Title = "Issue 11209";

				var layout = new StackLayout();

				var instructions = new Label
				{
					AutomationId = Success,
					Padding = 12,
					BackgroundColor = Color.Black,
					TextColor = Color.White,
					Text = "If navigated tapping an item from the CollectionView, the test has passed."
				};

				layout.Children.Add(instructions);

				Content = layout;
			}
		}
	}
}