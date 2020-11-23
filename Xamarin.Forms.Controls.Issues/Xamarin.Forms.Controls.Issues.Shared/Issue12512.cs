﻿using System;
using System.Diagnostics;
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
	[NUnit.Framework.Category(UITestCategories.ManualReview)]
#endif
	[Preserve(AllMembers = true)]
	[Issue(IssueTracker.Github, 12512, "[Bug] Application.RequestedThemeChanged raised twice on iOS when switching to home screen and back", PlatformAffected.Default)]
	public class Issue12512 : TestContentPage
	{
		int _count;
		readonly Label _themes;

		public Issue12512()
		{
			var layout = new StackLayout();

			var instructions = new Label
			{
				Padding = 12,
				BackgroundColor = Color.Black,
				TextColor = Color.White,
				Text = "Go to system settings and change the theme. Go back to the App to see that only one theme has been registered. Repeat the process again. When you return to the App you must have registered two theme changes."
			};

			_themes = new Label();

			layout.Children.Add(instructions);
			layout.Children.Add(_themes);

			Content = layout;
		}

		protected override void Init()
		{

		}

		protected override void OnAppearing()
		{
			base.OnAppearing();
			Application.Current.RequestedThemeChanged += OnRequestedThemeChanged;
		}

		protected override void OnDisappearing()
		{
			base.OnDisappearing();
			Application.Current.RequestedThemeChanged -= OnRequestedThemeChanged;
		}

		void OnRequestedThemeChanged(object sender, AppThemeChangedEventArgs e)
		{
			string requestedTheme = $"{_count}: {e.RequestedTheme}";
			Debug.WriteLine(requestedTheme);

			_themes.Text += requestedTheme + Environment.NewLine;
			_count++;
		}
	}
}