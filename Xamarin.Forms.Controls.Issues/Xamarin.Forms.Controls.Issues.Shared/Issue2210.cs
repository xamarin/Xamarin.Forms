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
		const string successlText = "Success";
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
			Application.Current.UseLegacyPageEvents = false;
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

					bool success = false;

					if (Application.Current.UseLegacyPageEvents)
					{
						success =
							states.Count == 8 &&
							states.IndexOf(State.PageAppeared) == -1 &&
							states.IndexOf(State.PageDisappeared) == -1 &&
							states.IndexOf(State.PageBeforeAppearing) < states.IndexOf(State.PageAppearing) &&
							states.IndexOf(State.PageBeforeAppearing) < states.IndexOf(State.ElementBeforeAppearing) &&
							states.IndexOf(State.PageAppearing) < states.IndexOf(State.PageDisappearing) &&
							states.IndexOf(State.PageLoaded) < states.IndexOf(State.PageUnloaded) &&
							states.IndexOf(State.PageLoaded) < states.IndexOf(State.PageDisappearing) &&
							states.IndexOf(State.PageLoaded) < states.IndexOf(State.ElementUnloaded) &&
							states.IndexOf(State.ElementBeforeAppearing) < states.IndexOf(State.ElementLoaded) &&
							states.IndexOf(State.ElementLoaded) < states.IndexOf(State.ElementUnloaded) &&
							states.IndexOf(State.ElementUnloaded) < states.IndexOf(State.PageUnloaded);
					}
					else
					{
						success =
							states.Count == 10 &&
							states.IndexOf(State.PageBeforeAppearing) < states.IndexOf(State.PageAppearing) &&
							states.IndexOf(State.PageAppearing) < states.IndexOf(State.PageAppeared) &&
							states.IndexOf(State.PageAppearing) < states.IndexOf(State.ElementBeforeAppearing) &&
							states.IndexOf(State.PageAppeared) < states.IndexOf(State.PageDisappearing) &&
							states.IndexOf(State.PageDisappearing) < states.IndexOf(State.PageDisappeared) &&
							states.IndexOf(State.PageLoaded) < states.IndexOf(State.PageUnloaded) &&
							states.IndexOf(State.PageLoaded) < states.IndexOf(State.PageDisappearing) &&
							states.IndexOf(State.PageLoaded) < states.IndexOf(State.ElementUnloaded) &&
							states.IndexOf(State.ElementBeforeAppearing) < states.IndexOf(State.ElementLoaded) &&
							states.IndexOf(State.ElementLoaded) < states.IndexOf(State.ElementUnloaded) &&
							states.IndexOf(State.ElementUnloaded) < states.IndexOf(State.PageUnloaded);
					}

					checkLabel.Text = success ? successlText : "Failed";
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
		[Test]
		public void Issue2210_CycleEvents()
		{
			RunningApp.Tap(startTestLabel);
			RunningApp.WaitForElement(successlText);

			// check legacy events
			RunningApp.Tap(switchButtonLabel);
			RunningApp.Tap(startTestLabel);
			RunningApp.WaitForElement(successlText);
		}
#endif
	}
}