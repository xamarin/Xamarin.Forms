using Xamarin.Forms.CustomAttributes;
using Xamarin.Forms.Internals;

#if UITEST
using NUnit.Framework;
using Xamarin.UITest;
#endif

namespace Xamarin.Forms.Controls.Issues
{
	[Preserve(AllMembers = true)]
	[Issue(IssueTracker.Bugzilla, 52510, "[iOS] ScrollView resets offset on device rotation.", PlatformAffected.iOS)]
	public class Bugzilla52510 : TestContentPage
	{
		protected override void Init()
		{
			var scrollContent = new StackLayout
			{
				Padding = new Thickness(10, 0),
				Spacing = 10
			};
			for (int i = 1; i <= 10; i++)
			{
				scrollContent.Children.Add(CreateChild(i));
			}

			Content = new ScrollView
			{
				Content = scrollContent
			};
		}

		View CreateChild(int index)
		{
			return new Item($"Child {index}")
			{
				AutomationId = $"Child{index}"
			};
		}

#if UITEST
		[Test]
		public void Bugzilla52510Test()
		{
			RunningApp.ScrollDownTo("Child10");
			RunningApp.Screenshot("The ScrollView has been scrolled to last child");

			RunningApp.SetOrientationLandscape();
			RunningApp.WaitForNoElement(q => q.Marked("Child1"));
			RunningApp.Screenshot("The ScrollView has not reset its scroll offset");
		}
#endif


		class Item : ContentView
		{
			public Item(string text)
			{
				BackgroundColor = Color.Silver;
				HeightRequest = 200;
				Padding = new Thickness(10);

				Content = new Label
				{
					Text = text
				};
			}
		}
	}
}
