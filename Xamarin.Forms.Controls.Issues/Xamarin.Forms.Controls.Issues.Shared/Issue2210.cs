using Xamarin.Forms.CustomAttributes;
using Xamarin.Forms.Internals;
using System.Diagnostics;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

#if UITEST
using Xamarin.UITest;
using NUnit.Framework;
#endif

namespace Xamarin.Forms.Controls
{
	[Preserve(AllMembers = true)]
	[Issue(IssueTracker.Github, 2210, "[Enhancement] Add better life cycle events", PlatformAffected.All)]
	public class Issue2210 : TestMasterDetailPage
	{
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

		public List<State> Result = new List<State>();

		void LogEvent(State state)
		{
			Result.Add(state);
			Debug.WriteLine($"{state}");
		}

		Button addRemoveButton;

		protected override void Init()
		{
			MasterBehavior = MasterBehavior.Split;

			var label = new Label();
			var resultLabel = new Label();

			addRemoveButton = new Button
			{
				Text = "GoTest",
				Command = new Command(async () =>
				{
					Result.Clear();
					resultLabel.Text = "Let's assume that this text is not here.";
					label.Text = "But soon everything will change.";

					var button = new Button
					{
						Text = "Button"
					};
					button.Loaded += (_, __) => LogEvent(State.ElementLoaded);
					button.Unloaded += (_, __) => LogEvent(State.ElementUnloaded);
					button.BeforeAppearing += (_, __) => LogEvent(State.ElementBeforeAppearing);

					var page = new ContentPage
					{
						Content = button
					};
					page.Loaded += (_, __) => LogEvent(State.PageLoaded);
					page.Unloaded += (_, __) => LogEvent(State.PageUnloaded);
					page.BeforeAppearing += (_, __) => LogEvent(State.PageBeforeAppearing);
					page.Appearing += (_, __) => LogEvent(State.PageAppearing);
					page.Appeared += (_, __) => LogEvent(State.PageAppeared);
					page.Disappearing += (_, __) => LogEvent(State.PageDisappearing);
					page.Disappeared += (_, __) => LogEvent(State.PageDisappeared);

					await Detail.Navigation.PushAsync(page);
					// UWP not waiting for page creation
					await Task.Delay(800);
					Detail.Navigation.RemovePage(page);

					var eraserPage = new ContentPage();
					await Detail.Navigation.PushAsync(eraserPage);
					Detail.Navigation.RemovePage(eraserPage);
					await Task.Delay(200);

					var sb = new StringBuilder();
					if (Application.Current.UseLegacyPageEvents)
						sb.AppendLine("[!] Legacy events is enabled").AppendLine();
					for (int i = 0; i < Result.Count; i++)
						sb.AppendLine($"{i} {Result[i]}");

					resultLabel.Text = sb.ToString();

					label.Text = "CheckMe";
				})
			};

			Master = new ContentPage
			{
				Title = "menu",
				Content = new StackLayout
				{
					Children =
					{
						addRemoveButton,
						resultLabel,
						new Button
						{
							Text = "Swith Legacy Events",
							Command = new Command(() => Application.Current.UseLegacyPageEvents = !Application.Current.UseLegacyPageEvents)
						}
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
							label
						}
					}
				}
			);
		}

#if UITEST
		[Test]
		public void Issue2210_CycleEvents()
		{
			RunningApp.Tap("GoTest");
			RunningApp.WaitForElement("CheckMe");
			Assert.AreEqual(10, Result.Count);
			CollectionAssert.AllItemsAreUnique(Result);

			var pageBeforeAppearing = Result.IndexOf(State.PageBeforeAppearing);
			var pageAppearing = Result.IndexOf(State.PageAppearing);
			var pageAppeared = Result.IndexOf(State.PageAppeared);
			var pageDisappearing = Result.IndexOf(State.PageDisappearing);
			var pageDisappeared = Result.IndexOf(State.PageDisappeared);
			var pageLoaded = Result.IndexOf(State.PageLoaded);
			var pageUnloaded = Result.IndexOf(State.PageUnloaded);

			var buttonBeforeAppearing = Result.IndexOf(State.ElementBeforeAppearing);
			var buttonLoaded = Result.IndexOf(State.ElementLoaded);
			var buttonUnloaded = Result.IndexOf(State.ElementUnloaded);

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
			RunningApp.Tap("Swith Legacy Events");
			RunningApp.Tap("GoTest");
			RunningApp.WaitForElement("CheckMe");
			Assert.AreEqual(8, Result.Count);
			CollectionAssert.AllItemsAreUnique(Result);
			CollectionAssert.DoesNotContain(Result, State.PageAppeared);
			CollectionAssert.DoesNotContain(Result, State.PageDisappeared);

			pageBeforeAppearing = Result.IndexOf(State.PageBeforeAppearing);
			pageAppearing = Result.IndexOf(State.PageAppearing);
			pageDisappearing = Result.IndexOf(State.PageDisappearing);
			pageLoaded = Result.IndexOf(State.PageLoaded);
			pageUnloaded = Result.IndexOf(State.PageUnloaded);

			buttonBeforeAppearing = Result.IndexOf(State.ElementBeforeAppearing);
			buttonLoaded = Result.IndexOf(State.ElementLoaded);
			buttonUnloaded = Result.IndexOf(State.ElementUnloaded);

			Assert.Less(pageBeforeAppearing, pageAppearing);
			Assert.Less(pageAppearing, buttonBeforeAppearing);
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