using System;
using System.Threading.Tasks;
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
	[Category(UITestCategories.Navigation)]
#endif
	[Preserve(AllMembers = true)]
	[Issue(IssueTracker.Github, 8843, "[Bug] [macOS] Fix back button for nested nav pages",
		PlatformAffected.macOS)]
	public partial class Issue8843 : TestContentPage
	{
		public Issue8843()
		{
#if APP
			InitializeComponent();
#endif
		}

		private void UpdateLabel(Label label)
		{
			var tapGestureRecognizer = new TapGestureRecognizer();
			tapGestureRecognizer.Tapped += (_s, _e) =>
			{
				var page2 = new Issue8843_Page2();
				SwitchToNewPage(this, page2, true);
			};
			label.GestureRecognizers.Add(tapGestureRecognizer);
		}

		protected override void Init()
		{
			Device.BeginInvokeOnMainThread(() =>
			{
				var mainLayout = this.FindByName<StackLayout>("mainLayout");
				var theLabel = mainLayout.FindByName<Label>("theLabel");
				UpdateLabel(theLabel);

				// workaround for bug https://github.com/xamarin/Xamarin.Forms/issues/9526
				theLabel.TextColor = Color.Black;
			});
		}

		private void SwitchToNewPage(Page currentPage, Page newPage, bool navBar)
		{
			NavigationPage.SetHasNavigationBar(newPage, navBar);
			var navPage = new NavigationPage(newPage);
			NavigationPage.SetHasNavigationBar(navPage, navBar);
			Device.BeginInvokeOnMainThread(() => DoubleCheckCompletionNonGeneric(currentPage.Navigation.PushAsync(navPage))
			);
		}
		private void DoubleCheckCompletionNonGeneric(Task task)
		{
			task.ContinueWith((Task t) => { MaybeCrash(false, t.Exception); }, TaskContinuationOptions.OnlyOnFaulted);
		}

		private void MaybeCrash(bool canBeCancelled, Exception ex)
		{
			if (ex == null)
			{
				return;
			}
			else
			{
				Device.BeginInvokeOnMainThread(() => { throw ex; });
			}
		}
	}
}