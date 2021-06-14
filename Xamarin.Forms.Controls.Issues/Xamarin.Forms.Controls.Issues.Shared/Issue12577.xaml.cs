using System;
using Xamarin.Forms.CustomAttributes;
using Xamarin.Forms.Internals;
using Xamarin.Forms.Xaml;

#if UITEST
using Xamarin.Forms.Core.UITests;
using Xamarin.UITest;
using NUnit.Framework;
#endif

namespace Xamarin.Forms.Controls.Issues
{
#if UITEST
	[Category(UITestCategories.ManualReview)]
	[Category(UITestCategories.Shape)]
#endif
#if APP
	[XamlCompilation(XamlCompilationOptions.Compile)]
#endif
	[Preserve(AllMembers = true)]
	[Issue(IssueTracker.Github, 12577, "CarouselView doesn't update the CurrentItem on Swipe under strange condition", PlatformAffected.iOS)]
	public partial class Issue12577 : TestContentPage
	{
		public Issue12577()
		{
#if APP
			InitializeComponent();
#endif
		}

		protected override void Init()
		{

		}
#if APP
		void OnPath1Tapped(object sender, EventArgs e) => DisplayAlert("Issue 12577", "Path 1 Tapped", "Ok");

		void OnPath2Tapped(object sender, EventArgs e) => DisplayAlert("Issue 12577", "Path 2 Tapped", "Ok");

		void OnPath3Tapped(object sender, EventArgs e) => DisplayAlert("Issue 12577", "Path 3 Tapped", "Ok");

		void OnPath4Tapped(object sender, EventArgs e) => DisplayAlert("Issue 12577", "Path 4 Tapped", "Ok");

		void OnPath5Tapped(object sender, EventArgs e) => DisplayAlert("Issue 12577", "Path 5 Tapped", "Ok");

		void OnPath6Tapped(object sender, EventArgs e) => DisplayAlert("Issue 12577", "Path 6 Tapped", "Ok");

		void OnPath7Tapped(object sender, EventArgs e) => DisplayAlert("Issue 12577", "Path 7 Tapped", "Ok");

		void OnPath8Tapped(object sender, EventArgs e) => DisplayAlert("Issue 12577", "Path 8 Tapped", "Ok");

		void OnPath9Tapped(object sender, EventArgs e) => DisplayAlert("Issue 12577", "Path 9 Tapped", "Ok");

		void OnPath10Tapped(object sender, EventArgs e) => DisplayAlert("Issue 12577", "Path 10 Tapped", "Ok");

		void OnPath11Tapped(object sender, EventArgs e) => DisplayAlert("Issue 12577", "Path 11 Tapped", "Ok");

		void OnPath12Tapped(object sender, EventArgs e) => DisplayAlert("Issue 12577", "Path 12 Tapped", "Ok");

		void OnPath13Tapped(object sender, EventArgs e) => DisplayAlert("Issue 12577", "Path 13 Tapped", "Ok");

		void OnPath14Tapped(object sender, EventArgs e) => DisplayAlert("Issue 12577", "Path 14 Tapped", "Ok");

		void OnPath15Tapped(object sender, EventArgs e) => DisplayAlert("Issue 12577", "Path 15 Tapped", "Ok");

		void OnPath16Tapped(object sender, EventArgs e) => DisplayAlert("Issue 12577", "Path 16 Tapped", "Ok");

		void OnPath17Tapped(object sender, EventArgs e) => DisplayAlert("Issue 12577", "Path 17 Tapped", "Ok");
#endif
	}
}