﻿using System;
using Xamarin.Forms.CustomAttributes;
using Xamarin.Forms.Internals;

#if UITEST
using Xamarin.UITest;
using NUnit.Framework;
#endif

namespace Xamarin.Forms.Controls.Issues
{
	[Preserve(AllMembers = true)]
	[Issue(IssueTracker.Bugzilla, 50787, "Can\'t animate Fragment transition when it\'s being removed from the stack", PlatformAffected.Android)]
	public class Bugzilla50787 : TestNavigationPage // or TestMasterDetailPage, etc ...
	{
		protected override void Init()
		{
			PushAsync(new ContentPage50787());
		}
	}

	[Preserve(AllMembers = true)]
	public class ContentPage50787 : ContentPage
	{
		public ContentPage50787()
		{
			BackgroundColor = Color.FromRgb(Guid.NewGuid().GetHashCode() % 255, Guid.NewGuid().GetHashCode() % 255, Guid.NewGuid().GetHashCode() % 255);

			var b = new Button
			{
				Text = "Push",
				Command = new Command(async () => { await Navigation.PushAsync(new ContentPage50787()); })
			};

			var b2 = new Button
			{
				Text = "Pop to root",
				Command = new Command(async () => { await Navigation.PopToRootAsync(); })
			};

			Content = new StackLayout
			{
				VerticalOptions = LayoutOptions.Center,
				Children =
				{
					b,
					b2
				}
			};
		}

		protected override void OnAppearing()
		{
			base.OnAppearing();
			System.Diagnostics.Debug.WriteLine("appearing");
		}

		protected override void OnDisappearing()
		{
			base.OnDisappearing();
			System.Diagnostics.Debug.WriteLine("disappearing");
		}
	}
}