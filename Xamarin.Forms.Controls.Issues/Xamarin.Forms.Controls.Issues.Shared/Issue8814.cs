﻿using System;
using System.Collections.Generic;
using Xamarin.Forms.CustomAttributes;
using Xamarin.Forms.Internals;

#if UITEST
using Xamarin.Forms.Core.UITests;
using Xamarin.UITest;
using NUnit.Framework;
#endif

namespace Xamarin.Forms.Controls.Issues
{
#if UITEST
	[Category(UITestCategories.Shell)]
	[Category(UITestCategories.CollectionView)]
#endif
	[Issue(IssueTracker.Github, 8814,
		"[Bug] UWP Shell cannot host CollectionView/CarouselView",
		PlatformAffected.UWP)]
	public class Issue8814 : TestShell
	{
		const string Success = "Success";

		protected override void Init()
		{
			var cv = new CollectionView();
			var items = new List<string>() { Success, "two", "three" };
			cv.ItemTemplate = new DataTemplate(() =>
			{

				var layout = new StackLayout();

				var label = new Label();
				label.SetBinding(Label.TextProperty, new Binding("."));

				layout.Children.Add(label);

				return layout;
			});

			cv.ItemsSource = items;

			var page = CreateContentPage<FlyoutItem>("CollectionView");

			var instructions = new Label { Text = "The should be a CollectionView visible below. If not, this test has failed. Unfortunately, without the fix for this bug, these instructions also won't be visible. 🤔" };

			page.Content = new StackLayout()
			{
				Children =
				{
					instructions,
					cv
				}
			};
		}

#if UITEST
		[Test]
		public void CollectionViewInShellShouldBeVisible()
		{
			RunningApp.WaitForElement(Success);
		}
#endif
	}
}
