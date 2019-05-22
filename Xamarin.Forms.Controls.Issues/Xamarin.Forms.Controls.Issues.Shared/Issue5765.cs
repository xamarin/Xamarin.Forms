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
	[Category(UITestCategories.CollectionView)]
#endif
	[Preserve(AllMembers = true)]
	[Issue(IssueTracker.Github, 5765, "[Frame, CollectionView, Android]The Label.Text is invisible on Android if DataTemplate have frame as layout",
		PlatformAffected.Android)]
	class Issue5765 : TestNavigationPage
	{
		const string Target = "FirstLabel";

		protected override void Init()
		{
			FlagTestHelpers.SetCollectionViewTestFlag();

			PushAsync(CreateRoot());
		}

		private Page CreateRoot()
		{
			var page = new ContentPage() { Title = "Issue5765" };

			var cv = new CollectionView();

			cv.ItemTemplate = new DataTemplate(() => {

				var frame = new Frame() { CornerRadius = 10 };

				var flexLayout = new FlexLayout()
				{
					Direction = FlexDirection.Row,
					JustifyContent = FlexJustify.SpaceBetween,
					AlignItems = FlexAlignItems.Stretch
				};

				var label1 = new Label { Text = "First Label", AutomationId = Target };
				var label2 = new Label { Text = "Second Label" };

				flexLayout.Children.Add(label1);
				flexLayout.Children.Add(label2);

				frame.Content = flexLayout;

				return frame;

			});

			cv.ItemsSource = new List<string> { "one", "two" };

			page.Content = cv;

			return page;
		}

#if UITEST
		[Test]
		public void LabelsInFramesShouldBeVisible()
		{
			// If the first label is visible at all, then this has succeeded
			RunningApp.WaitForElement(Target);
		}
#endif
	}
}
