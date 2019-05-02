﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using Xamarin.Forms.Controls.Issues;
using Xamarin.Forms.CustomAttributes;
using Xamarin.Forms.Internals;

#if UITEST
using Xamarin.Forms.Core.UITests;
using Xamarin.UITest;
using NUnit.Framework;
#endif

namespace Xamarin.Forms.Controls.Issues
{
#if UITEST
	[Category(UITestCategories.ListView)]
#endif

	[Preserve(AllMembers = true)]
	[Issue(IssueTracker.Bugzilla, 42329, "ListView in Frame and FormsAppCompatActivity Memory Leak")]
	public class Bugzilla42329 : TestMasterDetailPage
	{
		const string DestructorMessage = "ContentPageEx Destructor called";
		const string Page1Title = "Page1";
		const string Page2Title = "Page2";
		const string Page3Title = "Page3";
		const string LabelPage1 = "Open the drawer menu and select Page2";
		const string LabelPage2 = "Open the drawer menu and select Page3";
		readonly static string LabelPage3 = $"The console should have displayed the text '{DestructorMessage}' at least once. If not, this test has failed.";
		static string Success { get; set; } = string.Empty;
		static MasterDetailPage Reference;

		protected override void Init()
		{
			var masterPage = new MasterPage();
			Master = masterPage;
			masterPage.ListView.ItemSelected += (sender, e) =>
			{
				var item = e.SelectedItem as MasterPageItem;
				if (item != null)
				{
					Detail = new NavigationPage((Page)Activator.CreateInstance(item.TargetType));
					masterPage.ListView.SelectedItem = null;
					IsPresented = false;
				}
			};

			Detail = new NavigationPage(new _42329_FrameWithListView());
			Reference = this;
		}

		[Preserve(AllMembers = true)]
		public class MasterPage : ContentPage
		{
			public MasterPage()
			{
				Title = "Menu";
				ListView = new ListView { VerticalOptions = LayoutOptions.FillAndExpand, SeparatorVisibility = SeparatorVisibility.None };

				ListView.ItemTemplate = new DataTemplate(() =>
				{
					var ic = new ImageCell();
					ic.SetBinding(TextCell.TextProperty, "Title");
					return ic;
				});

				Content = new StackLayout
				{
					Children = { ListView }
				};

				var masterPageItems = new List<MasterPageItem>();
				masterPageItems.Add(new MasterPageItem
				{
					Title = Page1Title,
					TargetType = typeof(Bugzilla42329._42329_FrameWithListView)
				});
				masterPageItems.Add(new MasterPageItem
				{
					Title = Page2Title,
					TargetType = typeof(Bugzilla42329._42329_Page2)
				});
				masterPageItems.Add(new MasterPageItem
				{
					Title = Page3Title,
					TargetType = typeof(Bugzilla42329._42329_Page3)
				});

				ListView.ItemsSource = masterPageItems;
			}

			public ListView ListView { get; }
		}

		[Preserve(AllMembers = true)]
		public class MasterPageItem
		{
			public string IconSource { get; set; }

			public Type TargetType { get; set; }

			public string Title { get; set; }
		}

		[Preserve(AllMembers = true)]
		public class ContentPageEx : ContentPage
		{
			~ContentPageEx()
			{
				Success = "Desctructor called";
				Log.Warning("Bugzilla42329", DestructorMessage);
			}
		}

		[Preserve(AllMembers = true)]
		public class _42329_FrameWithListView : ContentPageEx
		{
			public _42329_FrameWithListView()
			{
				var lv = new ListView();
				var label = new Label() { Text =  LabelPage1};
				label.GestureRecognizers.Add(new TapGestureRecognizer
				{
					Command = new Command(OpenMaster)
				});
				var frame = new Frame { Content = lv };

				Title = Page1Title;
				Content = new StackLayout
				{
					Children =
					{
						label,
						frame
					}
				};
			}
		}

		static void OpenMaster()
		{
			Reference.IsPresented = true;
		}

		[Preserve(AllMembers = true)]
		public class _42329_Page2 : ContentPage
		{
			public _42329_Page2()
			{
				var lbl = new Label
				{
					Text = LabelPage2
				};
				lbl.GestureRecognizers.Add(new TapGestureRecognizer
				{
					Command = new Command(OpenMaster)
				});

				Title = Page2Title;
				Content = new StackLayout
				{
					Children =
					{
						lbl
					}
				};
			}
		}

		[Preserve(AllMembers = true)]
		public class _42329_Page3 : ContentPage
		{
			Label lblFlag;
			public _42329_Page3()
			{
				Title = Page3Title;
				Success = Success;
				lblFlag = new Label
				{
					Text = LabelPage3,
					HorizontalTextAlignment = TextAlignment.Center,
					TextColor = Color.Red
				};
				Content = new StackLayout
				{
					Children =
					{
						lblFlag
					}
				};
			}

			protected override async void OnAppearing()
			{
				base.OnAppearing();
				GarbageCollectionHelper.Collect();
				await DisplayAlert("Alert", Success, "ok");
			}
		}

#if UITEST
		[Test]
		public void MemoryLeakB42329()
		{
			RunningApp.WaitForElement(Page1Title);
			RunningApp.Tap(LabelPage1);
			RunningApp.WaitForElement(Page1Title);
			RunningApp.Tap(Page2Title);
			RunningApp.WaitForElement(LabelPage2);
			RunningApp.Tap(LabelPage2);
			RunningApp.WaitForElement(Page2Title);
			RunningApp.Tap(Page3Title);
			RunningApp.WaitForElement("Desctructor called");
			RunningApp.Tap("ok");
		}
#endif
	}
}
