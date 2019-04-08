using Xamarin.Forms.CustomAttributes;
using Xamarin.Forms.Internals;
using System;
using System.Runtime.CompilerServices;


#if UITEST
using Xamarin.UITest;
using NUnit.Framework;
using Xamarin.Forms.Core.UITests;
#endif

namespace Xamarin.Forms.Controls.Issues
{
	[Preserve(AllMembers = true)]
	[Issue(IssueTracker.Bugzilla, 43663, "ModalPushed and ModalPopped not working on WinRT", PlatformAffected.WinRT)]


#if UITEST
	[NUnit.Framework.Category(UITestCategories.Navigation)]
#endif
	public class Bugzilla43663 : TestNavigationPage
	{
		const string _message = "Message";

		const string _goBack = "Go back";

		const string _cancel = "Cancel";

		const string _pushModal = "Push Modal";

		const string _popModal = "Pop Modal";

		const string _modal = "Modal";
		protected override void Init()
		{
			Application.Current.ModalPushed += ModalPushed;
			Application.Current.ModalPopped += ModalPopped;

			var initialPage = new ContentPage();
			var insertedPage = new ContentPage
			{
				Content = new StackLayout
				{
					Children =
					{
						new Label
						{
							Text = "This page's appearing unsubscribes from the ModalPushed/ModalPopped events",
							HorizontalTextAlignment = TextAlignment.Center
						},
						new Button
						{
							Text = _goBack,
							Command = new Command(async () => await Navigation.PopModalAsync())
						}
					}
				}
			};
			insertedPage.Appearing += (s, e) =>
			{
				Application.Current.ModalPushed -= ModalPushed;
				Application.Current.ModalPopped -= ModalPopped;
			};

			var modalPage = new ContentPage();
			modalPage.Content = new StackLayout
			{
				Children =
				{
					new Label { Text = _modal },
					new Label
					{
						Text = "Now press the button bellow, and verify if you go back to previous page. If back's you've success!",
						HorizontalTextAlignment= TextAlignment.Center
					},
					new Button
					{
						Text = "Click to dismiss modal",
						Command = new Command(async() =>
						{
							await Navigation.PopModalAsync();
						}),
						AutomationId = _popModal
					}
				},
			};

			initialPage.Content = new StackLayout
			{
				VerticalOptions = LayoutOptions.Center,
				Children =
				{
					new Label
					{
						Text = "Verify if after you press the \"Click to push Modal\" button, you navigate to Modal Page.",
						HorizontalTextAlignment = TextAlignment.Center
					},
					new Button
					{
						Text = "Click to push Modal",
						Command = new Command(async () => await Navigation.PushModalAsync(modalPage)),
						AutomationId = _pushModal
					},
					new Button
					{
						Text = _goBack,
						Command = new Command(async () => await Navigation.PopAsync())
					}
				}
			};

			PushAsync(initialPage);
			Navigation.InsertPageBefore(insertedPage, initialPage);
		}

		void ModalPushed(object sender, ModalPushedEventArgs e)
		{
			DisplayAlert("Pushed", _message, _cancel);
		}

		void ModalPopped(object sender, ModalPoppedEventArgs e)
		{
			DisplayAlert("Popped", _message, _cancel);
		}

#if UITEST
		[Test]
		public void ModalNavigation()
		{
			DissmissAlert();
			RunningApp.WaitForElement(q => q.Marked(_pushModal));
			RunningApp.Tap(q => q.Marked(_pushModal));
			DissmissAlert();
			RunningApp.WaitForElement(q => q.Marked(_modal));
			RunningApp.Tap(q => q.Marked(_popModal));
			DissmissAlert();
			RunningApp.WaitForElement(q => q.Marked(_pushModal));
		}

		void DissmissAlert()
		{
			RunningApp.WaitForElement(_message);
			RunningApp.Tap(_cancel);
		}
#endif
	}
}