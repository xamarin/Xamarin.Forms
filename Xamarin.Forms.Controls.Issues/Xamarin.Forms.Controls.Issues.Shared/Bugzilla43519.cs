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
	[Issue(IssueTracker.Bugzilla, 43519, "[UWP] MasterDetail page ArguementException when nested in a TabbedPage and returning from modal page"
		, PlatformAffected.UWP)]

#if UITEST
	[NUnit.Framework.Category(UITestCategories.Navigation)]
#endif
	public class Bugzilla43519 : TestTabbedPage
	{
		const string _pop = "PopModal";

		const string _push = "PushModal";

		const string _page2 = "Page 2";

		protected override void Init()
		{
			var modalPage = new ContentPage
			{
				Title = "ModalPage",
				Content = new StackLayout
				{
					Children =
					{
						new Button
						{
							Command = new Command(() => Navigation.PopModalAsync()),
							Text = "Pop modal page -- should not crash on UWP",
							AutomationId = _pop
						}
					}
				}
			};

			var mdp = new MasterDetailPage
			{
				Title = "Page 1",
				Master = new ContentPage
				{
					Title = "Master",
					Content = new StackLayout()
				},
				Detail = new ContentPage
				{
					Title = "Detail",
					Content = new StackLayout()
				}
			};

			Children.Add(mdp);
			Children.Add(new ContentPage
			{
				Title = _page2,
				Content = new StackLayout
				{
					Children =
					{
						new Button
						{
							Command = new Command(() => Navigation.PushModalAsync(modalPage)),
							Text = "Click to display modal",
							AutomationId = _push
						}
					}
				}
			});
		}

#if UITEST
		[Test]
		public void TabbedModalNavigation()
		{
			RunningApp.WaitForElement(_page2);
			RunningApp.Tap(_page2);
			RunningApp.WaitForElement(_push);
			RunningApp.Tap(_push);
			RunningApp.WaitForElement(_pop);
			RunningApp.Tap(_pop);
			RunningApp.WaitForElement(_page2);

		}
#endif
	}
}