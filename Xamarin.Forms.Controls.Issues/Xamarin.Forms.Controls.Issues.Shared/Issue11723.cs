using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using Xamarin.Forms.CustomAttributes;
using Xamarin.Forms.Internals;
using System.Threading.Tasks;
using System.Runtime.CompilerServices;
using System.Linq;


#if UITEST
using Xamarin.UITest;
using NUnit.Framework;
using Xamarin.Forms.Core.UITests;
#endif

namespace Xamarin.Forms.Controls.Issues
{
	[Preserve(AllMembers = true)]
	[Issue(IssueTracker.Github, 11723, "[Bug] ContentPage in NavigationStack misplaced initially",
		PlatformAffected.iOS)]
#if UITEST
	[NUnit.Framework.Category(Core.UITests.UITestCategories.Github10000)]
	[NUnit.Framework.Category(UITestCategories.Shell)]
#endif
	public class Issue11723 : TestShell
	{
		ContentPage CreateContentPage()
		{
			var page = new ContentPage()
			{
				Content = new StackLayout()
				{
					Children =
					{
						new Label()
						{
							Text = "As you navigate this text should show up in the correct spot. If it's hidden and then shows up this test has failed."
						},
						new Label()
						{
							AutomationId = "LabelResult"
						},
						new Button()
						{
							Text = "Push Page",
							AutomationId = "PushPage",
							Command = new Command(async () =>
							{
								await Navigation.PushAsync(CreateContentPage());
							})
						}
					}
				}
			};

			page.PropertyChanged += OnPagePropertyChanged;
			return page;
		}

		bool navigated = false;
		protected override void OnNavigating(ShellNavigatingEventArgs args)
		{
			navigated = false;
			base.OnNavigating(args);
		}

		protected override void OnNavigated(ShellNavigatedEventArgs args)
		{
			base.OnNavigated(args);
			Device.BeginInvokeOnMainThread(() => navigated = true);
		}

		void OnPagePropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
		{
			if(sender is ContentPage page && e.PropertyName == "Padding")
			{
				page.PropertyChanged -= OnPagePropertyChanged;

				var label = page.Content
					.LogicalChildren
					.OfType<Label>()
					.First(x=> x.AutomationId == "LabelResult");

				if (navigated)
				{
					label.Text = "Failed";
				}
				else
				{
					label.Text = "Success";
				}
			}
		}

		protected override void Init()
		{
			AddContentPage(CreateContentPage());
		}


#if UITEST
		[Test]
		public void PaddingIsSetOnPageBeforeItsVisible()
		{
			RunningApp.WaitForElement("Success");
			RunningApp.WaitForElement("PushPage");
			RunningApp.WaitForElement("Success");
			RunningApp.WaitForElement("PushPage");
			RunningApp.WaitForElement("Success");
			RunningApp.WaitForElement("PushPage");
			TapBackArrow();
			RunningApp.WaitForElement("PushPage");
			TapBackArrow();
			RunningApp.WaitForElement("PushPage");
			TapBackArrow();
			RunningApp.WaitForElement("PushPage");
		}
#endif
	}
}
