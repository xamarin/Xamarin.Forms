using Xamarin.Forms.CustomAttributes;
using Xamarin.Forms.Internals;

#if UITEST
using Xamarin.UITest;
using NUnit.Framework;
#endif

// Apply the default category of "Issues" to all of the tests in this assembly
// We use this as a catch-all for tests which haven't been individually categorized
#if UITEST
[assembly: NUnit.Framework.Category("Issues")]
#endif

namespace Xamarin.Forms.Controls.Issues
{
	[Preserve(AllMembers = true)]
	[Issue(IssueTracker.Bugzilla, 932830, "Hiding navigation bar causes layouts to shift during navigation", PlatformAffected.iOS)]
	public class Bugzilla32830 : TestNavigationPage
	{
		const string Button1 = "button1";
		const string Button2 = "button2";
		const string BottomLabel = "I am visible at the bottom of the page";

		[Preserve(AllMembers = true)]
		class Page1 : ContentPage
		{
			public Page1()
			{
				Title = "Page 1";
				BackgroundColor = Color.Gray;

				var relativeLayout = new RelativeLayout { };

				relativeLayout.Children.Add(new StackLayout
				{
					VerticalOptions = LayoutOptions.Center,
					Children = {
						new Label {
							HorizontalTextAlignment = TextAlignment.Center,
							Text = "Page 1",
							TextColor = Color.White
						},
						new Button {
							Text = "Go to page 2",
							Command = new Command(async () => await Navigation.PushAsync(new Page2())),
							AutomationId = Button1,
							TextColor = Color.White
						},
						new Button {
							Text = "Toggle Nav Bar",
							Command = new Command(() => NavigationPage.SetHasNavigationBar(this, !NavigationPage.GetHasNavigationBar(this))),
							AutomationId = Button2,
							TextColor = Color.White
						}
					}
				}, yConstraint: Xamarin.Forms.Constraint.RelativeToParent(parent => { return parent.Y; }));

				relativeLayout.Children.Add(new Label
				{
					Text = BottomLabel,
					TextColor = Color.White
				}, yConstraint: Xamarin.Forms.Constraint.RelativeToParent(parent => { return parent.Height - 30; }));

				Content = relativeLayout;

				NavigationPage.SetHasNavigationBar(this, false);
			}
		}

		[Preserve(AllMembers = true)]
		class Page2 : ContentPage
		{
			public Page2()
			{
				Title = "Page 2";
				BackgroundColor = Color.Gray;
				var relativeLayout = new RelativeLayout { };
				relativeLayout.Children.Add(new StackLayout
				{
					VerticalOptions = LayoutOptions.Center,
					Children = {
							new Label {
								HorizontalTextAlignment = TextAlignment.Center,
								Text = "Page 2",
									TextColor = Color.White
							},
							new Button {
								Text = "Go to page 1",
								Command = new Command(async () => await Navigation.PushAsync(new Page1())),
								TextColor = Color.White
							},
							new Button {
								Text = "Toggle Nav Bar",
								Command = new Command(() => NavigationPage.SetHasNavigationBar(this, !NavigationPage.GetHasNavigationBar(this))),
								TextColor = Color.White
							}
						}
				}, yConstraint: Xamarin.Forms.Constraint.RelativeToParent(parent => { return parent.Y; }));

				relativeLayout.Children.Add(new Label
				{
					Text = BottomLabel,
					TextColor = Color.White
				}, yConstraint: Xamarin.Forms.Constraint.RelativeToParent(parent => { return parent.Height - 30; }));

				Content = relativeLayout;
			}
		}

		protected override void Init()
		{
			Navigation.PushAsync(new Page1());

		}

#if UITEST
		[Test]
		public void Bugzilla32830Test()
		{
			RunningApp.WaitForElement(q => q.Marked(BottomLabel));
			RunningApp.WaitForElement(q => q.Marked(Button1));
			RunningApp.Tap(q => q.Marked(Button1));
			RunningApp.WaitForElement(q => q.Marked(Button2));
			RunningApp.Tap(q => q.Marked(Button2));
			RunningApp.WaitForElement(q => q.Marked(BottomLabel));
		}
#endif
	}
}