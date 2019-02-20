﻿using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

using Xamarin.Forms.CustomAttributes;
using Xamarin.Forms.Internals;

#if UITEST
using NUnit.Framework;
using Xamarin.Forms.Core.UITests;
#endif

namespace Xamarin.Forms.Controls.Issues
{
	[Preserve(AllMembers = true)]
	[Issue(IssueTracker.Github, 1557, "Setting source crashes if view was detached from visual tree", PlatformAffected.iOS,
		navigationBehavior: NavigationBehavior.PushAsync)]
	public class Issue1557 : TestContentPage
	{
		const int Delay = 3000;

		ObservableCollection<string> _items = new ObservableCollection<string> { "foo", "bar" };

		protected override void Init()
		{
			var listView = new ListView
			{
				ItemsSource = _items
			};

			Content = listView;

			Task.Delay(Delay).ContinueWith(async t =>
			{
				var list = (ListView)Content;

				await Navigation.PopAsync();

				list.ItemsSource = new List<string> { "test" };
			}, TaskScheduler.FromCurrentSynchronizationContext());
		}

#if UITEST
		[Test]
		public void SettingSourceWhenDetachedDoesNotCrash()
		{
			Task.Delay(Delay + 1000).Wait();
			RunningApp.WaitForElement("Bug Repro's");
		}
#endif
	}
}
