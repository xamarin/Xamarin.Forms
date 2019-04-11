using System;

using Xamarin.Forms.CustomAttributes;
using Xamarin.Forms.Internals;
using System.Collections.Generic;

#if UITEST
using Xamarin.Forms.Core.UITests;
using Xamarin.UITest;
using NUnit.Framework;
#endif

namespace Xamarin.Forms.Controls.Issues
{
	[Preserve(AllMembers = true)]
	[Issue(IssueTracker.Bugzilla, 40824, "ListView item's contextual action menu not being closed upon navigation in AppCompat"
		, PlatformAffected.Android)]
#if UITEST
	[Category(UITestCategories.ListView)]
#endif
	public class Bugzilla40824 : TestContentPage
	{
		const string _cat = "Cat";
		const string _menu = "Action";
		const string _navigate = "Go to next page";
		const string _pageTitle = "Next Page";
		protected override void Init()
		{
			var list = new ListView
			{
				ItemsSource = new List<string>
				{
					_cat,
					"Dog",
					"Rat",
					"The contextual action should disapper when",
					"navigating to another page"
				},
				ItemTemplate = new DataTemplate(() =>
				{
					var cell = new TextCell();
					cell.SetBinding(TextCell.TextProperty, ".");
					cell.ContextActions.Add(new MenuItem
					{
						Text = _menu,
						AutomationId = _menu,
						Icon = "icon",
						IsDestructive = true,
						Command = new Command(() => DisplayAlert("TITLE", "Context action invoked", "Ok")),
					});
					return cell;
				}),
			};

			Content = new StackLayout
			{
				Children =
				{
					new Button
					{
						Text = _navigate,
						Command = new Command(() => Navigation.PushAsync(new ContentPage { Title = _pageTitle, Content = new Label { Text = "Here" } }))
					},
					list
				}
			};
		}

#if UITEST && __ANDROID__
		[Test]
		public void NavigationWithContextualAction()
		{
			RunningApp.WaitForElement(_cat);
			RunningApp.TouchAndHold(_cat);
			RunningApp.WaitForElement(_menu);
			RunningApp.Tap(_navigate);
			RunningApp.WaitForElement(_pageTitle);
			RunningApp.WaitForNoElement(_menu);
		}
#endif
	}
}