using System;
using Xamarin.Forms.CustomAttributes;
using Xamarin.Forms.Internals;

#if UITEST
using Xamarin.UITest;
using NUnit.Framework;
using Xamarin.Forms.Core.UITests;
#endif


namespace Xamarin.Forms.Controls.Issues
{
	[Preserve(AllMembers = true)]
	[Issue(IssueTracker.Github, 13387, "[Bug] [Android] Upgrade from version 4.8 to 5.0 breaks Label Texttype Html Text rendering", PlatformAffected.Android)]
#if UITEST
	[NUnit.Framework.Category(UITestCategories.ScrollView)]
#endif
	public class Issue13387 : TestContentPage
	{
		public Issue13387()
		{
			Title = "Issue 13387";

			var layout = new StackLayout();

			var infoLabel = new Label
			{
				Padding = 12,
				Text = "Navigate to the next Page and scroll to the bottom. If can read text until the end of the page, the test has passed."
			};

			var navigateButton = new Button
			{
				Text = "Navigate"
			};

			navigateButton.Clicked += (sender, args) =>
			{
				Navigation.PushAsync(new Issue13387SecondPage());
			};

			layout.Children.Add(infoLabel);
			layout.Children.Add(navigateButton);

			Content = layout;
		}

		protected override void Init()
		{

		}
	}

	public class Issue13387SecondPage : ContentPage
	{
		readonly Label _htmlLabel;

		public Issue13387SecondPage()
		{
			var scrollView = new ScrollView();

			var layout = new StackLayout();

			_htmlLabel = new Label { TextType = TextType.Html };
			
			layout.Children.Add(_htmlLabel);

			scrollView.Content = layout;

			Content = scrollView;
		}

		protected override void OnAppearing()
		{
			base.OnAppearing();

			_htmlLabel.Text = @"<H1>Privacy Policy terms of service</H1>
				< BR >
				< H2 > Password Wallet Sync</ H2 >
				   A safe place to save your passwords and the secret codes of cards and web sites or software products keys.The data is synchronized on different devices so I can use the same App on multiple computers or tablets or smartphones and data are always up to date.The data is saved to OneDrive and all data are encrypted with AES. It must be used the same Microsoft User Account on all the devices to ensure data synchronization.This App supports any biometric authentication methods(fingerprint, face, iris scan or numeric PIN on smartphones without biometric devices).
				< BR >
				< H2 > Privacy policy </ H2 >
				   Your privacy is a top priority.You always maintain ownership of your Data.We NEVER collect, process and store your Personal Information as you use this mobile application, and services.
				The Personal Information required to create an account is limited to a numeric PIN, an email address, and a password and a secret question and relative answer to PIN rescue.All that data are stored in your device only.
				This App uses the internet connection just to save information on your own Microsoft OneDrive account and all data uploaded/ downloaded are encrypted with AES-256.You have to login with your Microsoft Account.No personal information is collected or stored or sent to any remote or cloud services.
				Password Wallet Sync collects and logs aggregate usage statistics.Such information includes crash errors, date and time of crashes, device type used and operating system version, frequency of usages and to which functions, etc.We use this information to improve the services delivered to our customers, to track and diagnose performance problems.
				In accordance with the European Union General Data Protection Regulation(GDPR) Password Wallet Sync does not intentionally collect personally identifiable information from nor solicit children under the age of sixteen(16) years of age.
				< BR >
				The same policy are valid for the Windows companion App you can find on Windows Store.
				< BR >
				< H2 > Limitation of Liability </ H2 >
				The author assumes no liability for any data loss caused by incorrect use of the App.Any deficiencies or inaccuracies are not to be charged for this service.
				In no event will Password Wallet Sync be liable for any damages, including without limitation, incidental, special, consequential or punitive damages, whether under a contract, tort or any other theory of liability, arising in connection with any party's use of the app or in connection with any failure of performance, error, omission, interruption, defect, delay in operation or transmission, device virus, line system failure, loss of data, or loss of use related to this app, even if Password Wallet Sync is aware of the possibility of such damages.
				<BR>
				<H2>Spam and Sale of Information</H2>
				We despise spam.We will never do something that we hate ourselves.Thus, you can be rest assured that we will not spam your mailboxes with unsolicited emails, and we will never sell your information to other parties.
				<BR>
				<H2>Changes to This Privacy Policy</H2>
				The above policy is subject to change.This may be done without prior notice to all existing customers, but would be put in writing here on our website before it is implemented.This may occur due to new types of software, changing industry standards, and/or due to regulation / restrictions by international regulatory authorities as they apply.Thus, please check this policy on our website regularly.
				<BR>
				<H2>Contact us</H2>
				Support email : mb.swdev @gmail.com";
		}
	}
}
