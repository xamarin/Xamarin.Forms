using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using Xamarin.Forms.CustomAttributes;
using Xamarin.Forms.Internals;
using System.Linq;
using Xamarin.Forms.PlatformConfiguration;
using Xamarin.Forms.PlatformConfiguration.iOSSpecific;


#if UITEST
using Xamarin.UITest;
using NUnit.Framework;
using Xamarin.Forms.Core.UITests;
#endif

namespace Xamarin.Forms.Controls.Issues
{
	[Preserve(AllMembers = true)]
	[Issue(IssueTracker.None, 0, "Shell Inset Test",
		PlatformAffected.All)]
#if UITEST
	[NUnit.Framework.Category(UITestCategories.Shell)]
#endif
	public class ShellInsets : TestShell
	{
		const string EntryTest = "EntryTest";
		const string EntryToClick = "EntryToClick";
		const string EntrySuccess = "EntrySuccess";
		const string Reset = "Reset";

		const string ToggleSafeArea = "ToggleSafeArea";
		const string SafeAreaTest = "SafeAreaTest";
		const string SafeAreaTopLabel = "SafeAreaTopLabel";
		const string SafeAreaBottomLabel = "SafeAreaBottomLabel";

		const string ListViewTest = "ListViewTest";

		const string PaddingTest = "PaddingTest";
		const string PaddingEntry = "PaddingEntry";
		const string PaddingLabel = "PaddingLabel";

		const string EmptyPageSafeAreaTest = "EmptyPageSafeAreaTest";

		protected override void Init()
		{
			SetupLandingPage();
		}

		void SetupLandingPage()
		{
			var page = CreateContentPage();
			CheckBox cbox = new CheckBox() { AutomationId = ToggleSafeArea };
			Entry entryPadding = new Entry() { WidthRequest = 150, AutomationId = PaddingEntry };

			page.Content = new StackLayout()
			{
				Children =
				{
					new Button()
					{
						Text = "Entry Inset",
						Command = new Command(() => EntryInset()),
						AutomationId = EntryTest
					},
					new Button()
					{
						Text = "Safe Area on Page with no header",
						Command = new Command(() => EmptyPageSafeArea()),
						AutomationId = EmptyPageSafeAreaTest
					},
					new Button()
					{
						Text = "List View Scroll Test",
						Command = new Command(() => ListViewPage()),
						AutomationId = ListViewTest
					},
					new StackLayout()
					{
						Orientation = StackOrientation.Horizontal,
						Children =
						{
							cbox,
							new Button()
							{
								Text = "Safe Area",
								Command = new Command(() => SafeArea(cbox.IsChecked)),
								AutomationId = SafeAreaTest
							}
						},
						HorizontalOptions = LayoutOptions.Center
					},
					new StackLayout()
					{
						Orientation = StackOrientation.Horizontal,
						Children =
						{
							entryPadding,
							new Button()
							{
								Text = "Padding",
								Command = new Command(() => PaddingPage(entryPadding.Text)),
								AutomationId = PaddingTest
							}
						},
						HorizontalOptions = LayoutOptions.Center
					}
				}
			};

			CurrentItem = Items.Last();
			if (Items.Count > 1)
				Items.RemoveAt(0);
		}

		void EmptyPageSafeArea()
		{
			var page = CreateContentPage();
			var topLabel = new Label() { Text = "Top Label", HeightRequest = 200, AutomationId = SafeAreaTopLabel, VerticalOptions = LayoutOptions.Start };
			page.Content =
				new StackLayout()
				{
					Children =
					{
						topLabel,
						new StackLayout()
						{
							VerticalOptions = LayoutOptions.FillAndExpand,
							Children =
							{
								new Label() { Text = "This page should have no safe area padding at the top" },
								new Button() { Text = "Reset", Command = new Command(() => SetupLandingPage()) }
							},
							BackgroundColor = Color.White,

						}
					}
				};

			page.BackgroundColor = Color.Yellow;

			page.Appearing += (_, __) =>
			{
				topLabel.HeightRequest = page.On<iOS>().SafeAreaInsets().Top;
			};

			Shell.SetTabBarIsVisible(page, false);
			Shell.SetNavBarIsVisible(page, false);
			CurrentItem = Items.Last();
			Items.RemoveAt(0);
		}

		void ListViewPage()
		{
			var page = CreateContentPage();

			page.Content = new ListView(ListViewCachingStrategy.RecycleElement)
			{
				ItemTemplate = new DataTemplate(() =>
				{
					ViewCell cell = new ViewCell();
					var label = new Label() { Text = " I am a label" };
					label.SetBinding(Label.TextProperty, ".");
					label.SetBinding(Label.AutomationIdProperty, ".");

					cell.View =
					new StackLayout()
					{
						Orientation = StackOrientation.Horizontal,
						Children =
						{
							label,
							new Entry(),
							new Button() { Text = "Reset", Command = new Command(() => SetupLandingPage()) }
						}
					};

					return cell;
				}),
				ItemsSource = Enumerable.Range(0, 1000).Select(x => $"Item{x}").ToArray()
			};
			page.BackgroundColor = Color.Yellow;

			CurrentItem = Items.Last();
			Items.RemoveAt(0);
		}

		void PaddingPage(string text)
		{
			var page = CreateContentPage();

			int padding = 0;
			if (Int32.TryParse(text, out padding))
				page.Padding = padding;

			page.On<iOS>().SetUseSafeArea(false);
			page.Content = new StackLayout()
			{
				Children =
				{
					new Label(){ Text = "Top Label", HeightRequest = 200},
					new Label() { Text = $"Padding: {text}", AutomationId = PaddingLabel},
					new Button(){Text = "Reset", Command = new Command(() => SetupLandingPage() )}
				}
			};
			page.BackgroundColor = Color.Yellow;

			CurrentItem = Items.Last();
			Items.RemoveAt(0);
		}


		void SafeArea(bool value)
		{
			var page = CreateContentPage();
			page.Content = new StackLayout()
			{
				Children =
				{
					new Label(){ Text = "Top Label", HeightRequest = 200, AutomationId = SafeAreaTopLabel},
					new Label(){ Text = value ? "You should see two labels" : "You should see one label", AutomationId = SafeAreaBottomLabel},
					new Button(){ Text = "Reset", Command = new Command(() => SetupLandingPage() )}
				}
			};
			page.BackgroundColor = Color.Yellow;

			page.On<iOS>().SetUseSafeArea(value);
			CurrentItem = Items.Last();
			Items.RemoveAt(0);
		}

		void EntryInset()
		{
			var page = CreateContentPage();
			page.Title = "Main";
			page.Content = CreateEntryInsetView();
			page.BackgroundColor = Color.Yellow;

			CurrentItem = Items.Last();
			Items.RemoveAt(0);
		}

		View CreateEntryInsetView()
		{
			ScrollView view = null;
			view = new ScrollView()
			{
				Content = new StackLayout()
				{
					Children =
						{
							new Label(){ AutomationId = EntrySuccess, VerticalOptions= LayoutOptions.FillAndExpand, Text = "Click the entry and it should scroll up and stay visible. Click off entry and this label should still be visible"},
							new Button(){ Text = "Change Navbar Visible", Command = new Command(() => Shell.SetNavBarIsVisible(view.Parent, !(Shell.GetNavBarIsVisible(view.Parent))))},
							new Button()
							{
								Text = "Push On Page",
								Command = new Command(() => Navigation.PushAsync(new ContentPage(){ Content = CreateEntryInsetView() }))
							},
							new Button(){Text = "Reset", Command = new Command(() => SetupLandingPage() )},
							new Button()
							{
								Text = "Click Me"

							},
							new Entry()
							{
								AutomationId = EntryToClick
							},
							new Button()
							{
								Text = "Click Me"

							},
							new Button(){ Text = "Top Tab", Command = new Command(() => AddTopTab("top"))},
							new Button(){ Text = "Bottom Tab", Command = new Command(() => AddBottomTab("bottom"))},
						}
				}
			};

			return view;
		}


#if UITEST && __IOS__
		[Test]
		public void EntryScrollTest()
		{
			RunningApp.Tap(EntryTest);
			RunningApp.Tap(EntryToClick);
			RunningApp.WaitForNoElement(EntrySuccess);
			RunningApp.Tap("Click Me");
			RunningApp.WaitForElement(EntrySuccess);

		}

		[Test]
		public void ListViewScrollTest()
		{
			RunningApp.Tap(ListViewTest);
			RunningApp.WaitForElement("Item0");

		}

		[Test]
		public void SafeAreaOnBlankPage()
		{
			RunningApp.Tap(EmptyPageSafeAreaTest);
			var noSafeAreaLocation = RunningApp.WaitForElement(SafeAreaTopLabel);
			Assert.AreEqual(0, noSafeAreaLocation[0].Rect.Y);
		}

		[Test]
		public void SafeArea()
		{
			RunningApp.Tap(SafeAreaTest);
			var noSafeAreaLocation = RunningApp.WaitForElement(SafeAreaBottomLabel);

			Assert.AreEqual(1, noSafeAreaLocation.Length);
			RunningApp.Tap(Reset);

			RunningApp.Tap(ToggleSafeArea);
			RunningApp.Tap(SafeAreaTest);
			var safeAreaLocation = RunningApp.WaitForElement(SafeAreaBottomLabel);
			Assert.AreEqual(1, safeAreaLocation.Length);

			Assert.Greater(safeAreaLocation[0].Rect.Y, noSafeAreaLocation[0].Rect.Y);
		}

		[Test]
		public void PaddingWithoutSafeArea()
		{
			RunningApp.EnterText(q => q.Raw($"* marked:'{PaddingEntry}'"), "0");
			RunningApp.Tap(PaddingTest);
			var zeroPadding = RunningApp.WaitForElement(PaddingLabel);

			Assert.AreEqual(1, zeroPadding.Length);
			RunningApp.Tap(Reset);

			RunningApp.EnterText(PaddingEntry, "100");
			RunningApp.Tap(PaddingTest);
			var somePadding = RunningApp.WaitForElement(PaddingLabel);
			Assert.AreEqual(1, somePadding.Length);

			Assert.Greater(somePadding[0].Rect.Y, zeroPadding[0].Rect.Y);
		}
#endif
	}
}
