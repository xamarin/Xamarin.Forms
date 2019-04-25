using Xamarin.Forms.CustomAttributes;
using Xamarin.Forms.Internals;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

#if UITEST
using Xamarin.UITest;
using NUnit.Framework;
#endif

namespace Xamarin.Forms.Controls.Issues
{
	[Preserve(AllMembers = true)]
	[Issue(IssueTracker.Github, 2210, "[Enhancement] Add better life cycle events", PlatformAffected.All)]
	public class Issue2210 : TestMasterDetailPage
	{
		const string startTestLabel = "GoTest";
		const string switchButtonLabel = "Switch Legacy Events";
		const string checkLabelText = "CheckMe";
		const string resultId = "Result";

		public enum State
		{
			PageLoaded,
			PageUnloaded,
			PageBeforeAppearing,
			PageAppearing,
			PageAppeared,
			PageDisappearing,
			PageDisappeared,
			ElementBeforeAppearing,
			ElementLoaded,
			ElementUnloaded,
		}

		protected override void Init()
		{
			MasterBehavior = MasterBehavior.Split;

			IsPresented = true;

			var states = new List<State>();
			var checkLabel = new Label();
			var resultLabel = new Label { AutomationId = resultId };

			var goTestButton = new Button
			{
				Text = startTestLabel,
				Command = new Command(async () =>
				{
					states.Clear();
					resultLabel.Text = "Let's assume that this text is not here.";
					checkLabel.Text = "But soon everything will change.";

					var button = new Button
					{
						Text = "Button"
					};
					button.Loaded += (_, __) => states.Add(State.ElementLoaded);
					button.Unloaded += (_, __) => states.Add(State.ElementUnloaded);
					button.BeforeAppearing += (_, __) => states.Add(State.ElementBeforeAppearing);

					var page = new ContentPage
					{
						Content = button
					};
					page.Loaded += (_, __) => states.Add(State.PageLoaded);
					page.Unloaded += (_, __) => states.Add(State.PageUnloaded);
					page.BeforeAppearing += (_, __) => states.Add(State.PageBeforeAppearing);
					page.Appearing += (_, __) => states.Add(State.PageAppearing);
					page.Appeared += (_, __) => states.Add(State.PageAppeared);
					page.Disappearing += (_, __) => states.Add(State.PageDisappearing);
					page.Disappeared += (_, __) => states.Add(State.PageDisappeared);

					await Detail.Navigation.PushAsync(page);
					// UWP not waiting for page creation
					await Task.Delay(800);
					Detail.Navigation.RemovePage(page);

					var eraserPage = new ContentPage();
					await Detail.Navigation.PushAsync(eraserPage);
					Detail.Navigation.RemovePage(eraserPage);
					await Task.Delay(200);

					var sb = new StringBuilder();
					for (int i = 0; i < states.Count; i++)
						sb.AppendLine(states[i].ToString());
					resultLabel.Text = sb.ToString();

					checkLabel.Text = checkLabelText;
				})
			};

			Master = new ContentPage
			{
				Title = "menu",
				// on iOS the goTestButton overlaps by notch. Until this is corrected added padding-top.
				Padding = new Thickness(5, 40, 5, 0), 
				Content = new StackLayout
				{
					Children =
					{
						goTestButton,
						resultLabel,
						new Button
						{
							Text = switchButtonLabel,
							Command = new Command(() => Application.Current.UseLegacyPageEvents = !Application.Current.UseLegacyPageEvents)
						},
						checkLabel
					}
				}
			};

			Detail = new NavigationPage(
				new ContentPage
				{
					Content = new StackLayout
					{
						Children =
						{
							new Button
							{
								Text = "Open menu",
								Command = new Command(() => IsPresented = true)
							}
						}
					}
				}
			);
		}

#if UITEST
		string[] result;

		int IndexOf(State state) => result?.IndexOf(state.ToString()) ?? -1;

		void GetResult()
		{
			var resultLabel = RunningApp.Query(c => c.Marked(resultId))[0].Text;
			result = resultLabel.Split(new [] { '\r', '\n' }, System.StringSplitOptions.RemoveEmptyEntries);
		}

		[Test]
		public void Issue2210_CycleEvents()
		{
			RunningApp.Tap(startTestLabel);
			RunningApp.WaitForElement(checkLabelText);
			GetResult();
			Assert.AreEqual(10, result.Length);
			CollectionAssert.AllItemsAreUnique(result);

			var pageBeforeAppearing = IndexOf(State.PageBeforeAppearing);
			var pageAppearing = IndexOf(State.PageAppearing);
			var pageAppeared = IndexOf(State.PageAppeared);
			var pageDisappearing = IndexOf(State.PageDisappearing);
			var pageDisappeared = IndexOf(State.PageDisappeared);
			var pageLoaded = IndexOf(State.PageLoaded);
			var pageUnloaded = IndexOf(State.PageUnloaded);

			var buttonBeforeAppearing = IndexOf(State.ElementBeforeAppearing);
			var buttonLoaded = IndexOf(State.ElementLoaded);
			var buttonUnloaded = IndexOf(State.ElementUnloaded);

			Assert.Less(pageBeforeAppearing, pageAppearing);
			Assert.Less(pageAppearing, pageAppeared);
			Assert.Less(pageAppearing, buttonBeforeAppearing);
			Assert.Less(pageAppeared, pageDisappearing);
			Assert.Less(pageDisappearing, pageDisappeared);
			Assert.Less(pageLoaded, pageUnloaded);
			Assert.Less(pageLoaded, pageDisappearing);
			Assert.Less(pageLoaded, buttonUnloaded);

			Assert.Less(buttonBeforeAppearing, buttonLoaded);
			Assert.Less(buttonLoaded, buttonUnloaded);
			Assert.Less(buttonUnloaded, pageUnloaded);

			// check legacy events
			RunningApp.Tap(switchButtonLabel);
			RunningApp.Tap(startTestLabel);
			RunningApp.WaitForElement(checkLabelText);
			GetResult();
			Assert.AreEqual(8, result.Length);
			CollectionAssert.AllItemsAreUnique(result);
			CollectionAssert.DoesNotContain(result, State.PageAppeared.ToString());
			CollectionAssert.DoesNotContain(result, State.PageDisappeared.ToString());

			pageBeforeAppearing = IndexOf(State.PageBeforeAppearing);
			pageAppearing = IndexOf(State.PageAppearing);
			pageDisappearing = IndexOf(State.PageDisappearing);
			pageLoaded = IndexOf(State.PageLoaded);
			pageUnloaded = IndexOf(State.PageUnloaded);

			buttonBeforeAppearing = IndexOf(State.ElementBeforeAppearing);
			buttonLoaded = IndexOf(State.ElementLoaded);
			buttonUnloaded = IndexOf(State.ElementUnloaded);

			Assert.Less(pageBeforeAppearing, pageAppearing);
			Assert.Less(pageBeforeAppearing, buttonBeforeAppearing);
			Assert.Less(pageAppearing, pageDisappearing);
			Assert.Less(pageLoaded, pageUnloaded);
			Assert.Less(pageLoaded, pageDisappearing);
			Assert.Less(pageLoaded, buttonUnloaded);

			Assert.Less(buttonBeforeAppearing, buttonLoaded);
			Assert.Less(buttonLoaded, buttonUnloaded);
			Assert.Less(buttonUnloaded, pageUnloaded);
		}
#endif
	}
}