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

		protected override void Init()
		{
			SetupLandingPage();
		}

		void SetupLandingPage()
		{
			var page = CreateContentPage();
			CheckBox cbox = new CheckBox();
			Entry entryPadding = new Entry() { WidthRequest = 150 };

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
					new StackLayout()
					{
						Orientation = StackOrientation.Horizontal,
						Children =
						{
							cbox,
							new Button()
							{
								Text = "Safe Area",
								Command = new Command(() => SafeArea(cbox.IsChecked))
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
								Command = new Command(() => PaddingPage(entryPadding.Text))
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
					new Label() { Text = $"Padding: {text}"},
					new Button(){Text = "Reset", Command = new Command(() => SetupLandingPage() )}
				}
			};

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
					new Label(){ Text = "Top Label", HeightRequest = 200},
					new Label(){Text = value ? "You should see two labels" : "You should see one label"},
					new Button(){Text = "Reset", Command = new Command(() => SetupLandingPage() )}
				}
			};

			page.On<iOS>().SetUseSafeArea(value);
			CurrentItem = Items.Last();
			Items.RemoveAt(0);
		}

		void EntryInset()
		{
			var page = CreateContentPage();
			page.Title = "Main";
			page.Content = CreateEntryInsetView();

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
							new Button(){ Text = "Top Tab", Command = new Command(() => AddTopTab("top"))},
							new Button(){ Text = "Bottom Tab", Command = new Command(() => AddBottomTab("bottom"))},
							new Button(){ Text = "Change Navbar Visible", Command = new Command(() => Shell.SetNavBarIsVisible(view.Parent, !(Shell.GetNavBarIsVisible(view.Parent))))},
							new Button()
							{
								Text = "Push On Page",
								Command = new Command(() => Navigation.PushAsync(new ContentPage(){ Content = CreateEntryInsetView() }))
							},
							new Button(){Text = "Reset", Command = new Command(() => SetupLandingPage() )},
							new Entry()
							{
								AutomationId = EntryToClick
							}
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
			RunningApp.TapCoordinates(0, 0);
			RunningApp.WaitForElement(EntrySuccess);

		}
#endif
	}
}
