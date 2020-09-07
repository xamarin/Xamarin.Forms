using System;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using Xamarin.Forms.Core;
using Xamarin.Forms.CustomAttributes;
using Xamarin.Forms.Internals;
#if UITEST
using NUnit.Framework;
using Xamarin.Forms.Core.UITests;
using Xamarin.UITest.Queries;
#endif

namespace Xamarin.Forms.Controls.Issues
{
	[Preserve(AllMembers = true)]
	[Issue(IssueTracker.None, 0, "Shell Badges Test",
		PlatformAffected.All)]
#if UITEST
	[NUnit.Framework.Category(UITestCategories.Shell)]
#endif
	public class ShellBadges : TestShell
	{
		const string ToggleFlyout = "Flyout";

		const string SetContentBadgeText = "CBT";

		const string SetContentBadgeTextColor = "CBTC";

		const string SetContentBadgeUnselectedTextColor = "CBUTC";

		const string SetContentBadgeBackground = "CBB";

		const string SetContentBadgeUnselectedBackground = "CBUB";

		const string SetSectionBadgeText = "SBT";

		const string SetSectionBadgeTextColor = "SBTC";

		const string SetSectionBadgeUnselectedTextColor = "SBUTC";

		const string SetSectionBadgeBackground = "SBB";

		const string SetSectionBadgeUnselectedBackground = "SBUB";

		const string SetItemBadgeText = "IBT";

		const string SetItemBadgeTextColor = "IBTC";

		const string SetItemBadgeUnselectedTextColor = "IBUTC";

		const string SetItemBadgeBackground = "IBB";

		const string SetItemBadgeUnselectedBackground = "IBUB";

#if __ANDROID__
		// Color.White.ToAndroid().ToArgb();
		protected int TextColorDefault = -1;

		// Color.DarkBlue.ToAndroid().ToArgb();
		protected int TextColorSet = -16777077;

		// Color.DarkGreen.ToAndroid().ToArgb();
		protected int UnselectedTextColorSet = -16751616;

		// Color.FromRgb(255, 59, 48).ToAndroid().ToArgb();
		protected int BackgroundDefault = -50384;

		// Color.DarkOrange.ToAndroid().ToArgb()
		protected int BackgroundSet = -29696;

		// Color.DarkMagenta.ToAndroid().ToArgb();
		protected int UnselectedBackgroundSet = -7667573;
#endif

#if __IOS__
		// Color.White.ToUIColor().GetRGBA(out red, out green, out blue, out alpha);
		// $"rgb({(int) (red*255)},{(int) (green*255)},{(int) (blue*255)})";
		protected string TextColorDefault = "rgb(255,255,255)";

		// Color.DarkBlue.ToUIColor().GetRGBA(out red, out green, out blue, out alpha);
		// $"rgb({(int) (red*255)},{(int) (green*255)},{(int) (blue*255)})";
		protected string TextColorSet = "rgb(0,0,139)";

		// Color.DarkGreen.ToUIColor().GetRGBA(out red, out green, out blue, out alpha);
		// $"rgb({(int) (red*255)},{(int) (green*255)},{(int) (blue*255)})";
		protected string UnselectedTextColorSet = "rgb(0,100,0)";

		// Color.FromRgb(255, 59, 48).ToUIColor().GetRGBA(out red, out green, out blue, out alpha);
		// $"rgb({(int) (red*255)},{(int) (green*255)},{(int) (blue*255)})";
		protected string BackgroundDefault = "rgb(255,59,48)";

		// Color.DarkOrange.ToUIColor().GetRGBA(out red, out green, out blue, out alpha);
		// $"rgb({(int) (red*255)},{(int) (green*255)},{(int) (blue*255)})";
		protected string BackgroundSet = "rgb(255,140,0)";

		// Color.DarkMagenta.ToUIColor().GetRGBA(out red, out green, out blue, out alpha);
		// $"rgb({(int) (red*255)},{(int) (green*255)},{(int) (blue*255)})";
		protected string UnselectedBackgroundSet = "rgb(139,0,139)";
#endif

		// TODO: Missing test cases
		// Empty BadgeText, BadgeMoreText, implicit ShellItems and ShellSections, ...
		protected override void Init()
		{
			Items.Add(CreateShellItem("Item 1", new[]
			{
				CreateShellSection("Section 11", new[]
				{
					CreateShellContent("Content 111"),
					CreateShellContent("Content 112")
				}),
				CreateShellSection("Section 12", new[]
				{
					CreateShellContent("Content 121"),
					CreateShellContent("Content 122")
				}),
			}));

			Items.Add(CreateShellItem("Item 2", new[]
			{
				CreateShellSection("Section 21", new[]
				{
					CreateShellContent("Content 211"),
					CreateShellContent("Content 212")
				}),
				CreateShellSection("Section 22", new[]
				{
					CreateShellContent("Content 221"),
					CreateShellContent("Content 222")
				}),
			}));
		}

		static ShellContent CreateShellContent(string title)
		{
			var shellContent = new ShellContent
			{
				Title = title,
				Content = new ContentPage
				{
					Content = new StackLayout
					{
						Children =
						{
							CreateButton(ToggleFlyout, (sender, args) => { Shell.Current.FlyoutIsPresented = true; }),
							new StackLayout
							{
								Orientation = StackOrientation.Horizontal,
								Children =
								{
									CreateBadgeButton(SetContentBadgeText, () => { Shell.Current.ShellContentBadgeViewModel("Content 111").Text = "2"; }),
									CreateBadgeButton(SetContentBadgeTextColor, () => { Shell.Current.ShellContentBadgeViewModel("Content 111").TextColor = Color.DarkBlue; }),
									CreateBadgeButton(SetContentBadgeBackground, () => { Shell.Current.ShellContentBadgeViewModel("Content 111").Background = Brush.DarkOrange; }),
								}
							},
							new StackLayout
							{
								Orientation = StackOrientation.Horizontal,
								Children =
								{
									CreateBadgeButton(SetContentBadgeUnselectedTextColor, () => { /* TODO: Implement after VisualStateManager support for badges */ }),
									CreateBadgeButton(SetContentBadgeUnselectedBackground, () => { /* TODO: Implement after VisualStateManager support for badges */ }),
								}
							},
							new StackLayout
							{
								Orientation = StackOrientation.Horizontal,
								Children =
								{
									CreateBadgeButton(SetSectionBadgeText, () => { Shell.Current.ShellSectionBadgeViewModel("Section 11").Text = "2"; }),
									CreateBadgeButton(SetSectionBadgeTextColor, () => { Shell.Current.ShellSectionBadgeViewModel("Section 11").TextColor = Color.DarkBlue; }),
									CreateBadgeButton(SetSectionBadgeBackground, () => { Shell.Current.ShellSectionBadgeViewModel("Section 11").Background = Brush.DarkOrange; }),
								},
							},
							new StackLayout
							{
								Orientation = StackOrientation.Horizontal,
								Children =
								{
									CreateBadgeButton(SetSectionBadgeUnselectedTextColor, () => { /* TODO: Implement after VisualStateManager support for badges */ }),
									CreateBadgeButton(SetSectionBadgeUnselectedBackground, () => { /* TODO: Implement after VisualStateManager support for badges */ }),
								},
							},
							new StackLayout
							{
								Orientation = StackOrientation.Horizontal,
								Children =
								{
									CreateBadgeButton(SetItemBadgeText, () => { Shell.Current.ShellItemBadgeViewModel("Item 1").Text = "2"; }),
									CreateBadgeButton(SetItemBadgeTextColor, () => { Shell.Current.ShellItemBadgeViewModel("Item 1").TextColor = Color.DarkBlue; }),
									CreateBadgeButton(SetItemBadgeBackground, () => { Shell.Current.ShellItemBadgeViewModel("Item 1").Background = Brush.DarkOrange; }),
								},
							},
							new StackLayout
							{
								Orientation = StackOrientation.Horizontal,
								Children =
								{
									CreateBadgeButton(SetItemBadgeUnselectedTextColor, () => { /* TODO: Implement after VisualStateManager support for badges */ }),
									CreateBadgeButton(SetItemBadgeUnselectedBackground, () => { /* TODO: Implement after VisualStateManager support for badges */ }),
								},
							},
						}
					}
				},
				Icon = "coffee.png",
				BindingContext = new BadgeViewModel()
				{
					Text = "1",
				}
			};

			ApplyBadgeViewModel(shellContent);

			return shellContent;
		}

		static void ApplyBadgeViewModel(BindableObject bindableObject)
		{
			bindableObject.SetBinding(Badge.BadgeTextProperty, nameof(BadgeViewModel.Text));
			bindableObject.SetBinding(Badge.BadgeTextColorProperty, nameof(BadgeViewModel.TextColor));
			bindableObject.SetBinding(Badge.BadgeBackgroundProperty, nameof(BadgeViewModel.Background));
		}

		static ShellItem CreateShellItem(string title, params ShellSection[] shellSections)
		{
			var shellItem = new ShellItem()
			{
				Title = title,
				BindingContext = new BadgeViewModel()
				{
					Text = "1",
				}
			};

			shellItem.AutomationId = title;

			foreach (ShellSection shellSection in shellSections)
			{
				shellItem.Items.Add(shellSection);
			}

			ApplyBadgeViewModel(shellItem);

			return shellItem;
		}

		static ShellSection CreateShellSection(string title, params ShellContent[] shellContents)
		{
			var shellSection = new ShellSection()
			{
				Title = title,
				BindingContext = new BadgeViewModel()
				{
					Text = "1",
				}
			};

			foreach (ShellContent shellContent in shellContents)
			{
				shellSection.Items.Add(shellContent);
			}

			ApplyBadgeViewModel(shellSection);

			return shellSection;
		}

		static Button CreateButton(string text, Action<object, EventArgs> action)
		{
			Button button = new Button() { Text = text };
			button.Clicked += action.Invoke;

			return button;
		}

		static Button CreateBadgeButton(string text, Action action) => CreateButton(text, (sender, args) => { action(); });

#if UITEST && (__IOS__ || __ANDROID__)
		public void Test(string buttonIdentifier, Action preCondition, Action postCondition)
		{
			RunningApp.WaitForElement(buttonIdentifier);

			preCondition();
			RunningApp.Tap(buttonIdentifier);
			postCondition();
		}

		[Test]
		public void SetContentBadgeTextTest()
		{
			Test(SetContentBadgeText, () =>
			{
				RunningApp.WaitForElement(x => x.ContentBadge().ContentBadgeText("1"));
			}, () =>
			{
				RunningApp.WaitForElement(x => x.ContentBadge().ContentBadgeText("2"));
			});
		}

		[Test]
		public void SetSectionBadgeTextTest()
		{
			Test(SetSectionBadgeText, () =>
			{
				RunningApp.WaitForElement(x => x.SectionBadge().SectionBadgeText("1"));
			}, () =>
			{
				RunningApp.WaitForElement(x => x.SectionBadge().SectionBadgeText("2"));
			});
		}

		[Test]
		public void SetItemBadgeTextTest()
		{
			Test(SetItemBadgeText, () =>
			{
				RunningApp.Tap(ToggleFlyout);
				RunningApp.WaitForElement(x => x.ItemBadge().ItemBadgeText("1"));
				RunningApp.Tap("Item 1");
			}, () =>
			{
				RunningApp.Tap(ToggleFlyout);
				RunningApp.WaitForElement(x => x.ItemBadge().ItemBadgeText("2"));
			});
		}

		[Test]
		public void SetContentBadgeTextColorTest()
		{
			Test(SetContentBadgeTextColor, () =>
			{
				Assert.AreEqual(TextColorDefault, RunningApp.Query(x => x.ContentBadge().ContentBadgeTextColor()).Single());
			}, () =>
			{
				Assert.AreEqual(TextColorSet, RunningApp.Query(x => x.ContentBadge().ContentBadgeTextColor()).Single());
			});
		}

		[Test]
		public void SetSectionBadgeTextColorTest()
		{
			Test(SetSectionBadgeTextColor, () =>
			{
				Assert.AreEqual(TextColorDefault, RunningApp.Query(x => x.SectionBadge().SectionBadgeTextColor()).Single());
			}, () =>
			{
				Assert.AreEqual(TextColorSet, RunningApp.Query(x => x.SectionBadge().SectionBadgeTextColor()).Single());
			});
		}

		[Test]
		public void SetItemBadgeTextColorTest()
		{
			Test(SetItemBadgeTextColor, () =>
			{
				RunningApp.Tap(ToggleFlyout);
				RunningApp.WaitForElement("Item 1");
				Assert.AreEqual(TextColorDefault, RunningApp.Query(x => x.ItemBadge().ItemBadgeTextColor()).Single());
				RunningApp.Tap("Item 1");
			}, () =>
			{
				RunningApp.Tap(ToggleFlyout);
				RunningApp.WaitForElement("Item 1");
				Assert.AreEqual(TextColorSet, RunningApp.Query(x => x.ItemBadge().ItemBadgeTextColor()).Single());
			});
		}

		[Test]
		[Ignore("Reactivate after VisualStateManager support for badges")]
		public void SetContentBadgeUnselectedTextColorTest()
		{
			Test(SetContentBadgeUnselectedTextColor, () =>
			{
				Assert.AreEqual(TextColorDefault, RunningApp.Query(x => x.ContentBadge("Content 112").ContentBadgeTextColor()).Single());
			}, () =>
			{
				Assert.AreEqual(UnselectedTextColorSet, RunningApp.Query(x => x.ContentBadge("Content 112").ContentBadgeTextColor()).Single());
			});
		}

		[Test]
		[Ignore("Reactivate after VisualStateManager support for badges")]
		public void SetSectionBadgeUnselectedTextColorTest()
		{
			Test(SetSectionBadgeUnselectedTextColor, () =>
			{
				Assert.AreEqual(TextColorDefault, RunningApp.Query(x => x.SectionBadge("Section 12").SectionBadgeTextColor()).Single());
			}, () =>
			{
				Assert.AreEqual(UnselectedTextColorSet, RunningApp.Query(x => x.SectionBadge("Section 12").SectionBadgeTextColor()).Single());
			});
		}

		[Test]
		[Ignore("Reactivate after VisualStateManager support for badges")]
		public void SetItemBadgeUnselectedTextColorTest()
		{
			Test(SetItemBadgeUnselectedTextColor, () =>
			{
				RunningApp.Tap(ToggleFlyout);
				RunningApp.WaitForElement("Item 1");
				Assert.AreEqual(TextColorDefault, RunningApp.Query(x => x.ItemBadge("Item 2").ItemBadgeTextColor()).Single());
				RunningApp.Tap("Item 1");
			}, () =>
			{
				RunningApp.Tap(ToggleFlyout);
				RunningApp.WaitForElement("Item 1");
				Assert.AreEqual(UnselectedTextColorSet, RunningApp.Query(x => x.ItemBadge("Item 2").ItemBadgeTextColor()).Single());
			});
		}

		[Test]
#endif
#if UITEST && (__IOS__)
		[Ignore("Find a way to get the background color of the sublayer inserted by brush extensions")]
#endif
#if UITEST && (__IOS__ || __ANDROID__)
		public void SetContentBadgeBackgroundTest()
		{
			Test(SetContentBadgeBackground, () =>
			{
				Assert.AreEqual(BackgroundDefault, RunningApp.Query(x => x.ContentBadge().ContentBadgeBackground()).Single());
			}, () =>
			{
				Assert.AreEqual(BackgroundSet, RunningApp.Query(x => x.ContentBadge().ContentBadgeBackground()).Single());
			});
		}

		[Test]
		public void SetSectionBadgeBackgroundTest()
		{
			Test(SetSectionBadgeBackground, () =>
			{
				Assert.AreEqual(BackgroundDefault, RunningApp.Query(x => x.SectionBadge().SectionBadgeBackground()).Single());

			}, () =>
			{
				Assert.AreEqual(BackgroundSet, RunningApp.Query(x => x.SectionBadge().SectionBadgeBackground()).Single());
			});
		}

		[Test]
#endif
#if UITEST && (__IOS__)
		[Ignore("Find a way to get the background color of the sublayer inserted by brush extensions")]
#endif
#if UITEST && (__IOS__ || __ANDROID__)
		public void SetItemBadgeBackgroundTest()
		{
			Test(SetItemBadgeBackground, () =>
			{
				RunningApp.Tap(ToggleFlyout);
				RunningApp.WaitForElement("Item 1");
				Assert.AreEqual(BackgroundDefault, RunningApp.Query(x => x.ItemBadge().ItemBadgeBackground()).Single());
				RunningApp.Tap("Item 1");
			}, () =>
			{
				RunningApp.Tap(ToggleFlyout);
				RunningApp.WaitForElement("Item 1");
				Assert.AreEqual(BackgroundSet, RunningApp.Query(x => x.ItemBadge().ItemBadgeBackground()).Single());
			});
		}

		[Test]
		[Ignore("Reactivate after VisualStateManager support for badges")]
		public void SetContentBadgeUnselectedBackgroundTest()
		{
			Test(SetContentBadgeUnselectedBackground, () =>
			{
				Assert.AreEqual(BackgroundDefault, RunningApp.Query(x => x.ContentBadge("Content 112").ContentBadgeBackground()).Single());
			}, () =>
			{
				Assert.AreEqual(UnselectedBackgroundSet, RunningApp.Query(x => x.ContentBadge("Content 112").ContentBadgeBackground()).Single());
			});
		}

		[Test]
		[Ignore("Reactivate after VisualStateManager support for badges")]
		public void SetSectionBadgeUnselectedBackgroundTest()
		{
			Test(SetSectionBadgeUnselectedBackground, () =>
			{
				Assert.AreEqual(BackgroundDefault, RunningApp.Query(x => x.SectionBadge("Section 12").SectionBadgeBackground()).Single());
			}, () =>
			{
				Assert.AreEqual(UnselectedBackgroundSet, RunningApp.Query(x => x.SectionBadge("Section 12").SectionBadgeBackground()).Single());
			});
		}

		[Test]
		[Ignore("Reactivate after VisualStateManager support for badges")]
		public void SetItemBadgeUnselectedBackgroundTest()
		{
			Test(SetItemBadgeUnselectedBackground, () =>
			{
				RunningApp.Tap(ToggleFlyout);
				RunningApp.WaitForElement("Item 1");
				Assert.AreEqual(BackgroundDefault, RunningApp.Query(x => x.ItemBadge("Item 2").ItemBadgeBackground()).Single());
				RunningApp.Tap("Item 1");
			}, () =>
			{
				RunningApp.Tap(ToggleFlyout);
				RunningApp.WaitForElement("Item 1");
				Assert.AreEqual(UnselectedBackgroundSet, RunningApp.Query(x => x.ItemBadge("Item 2").ItemBadgeBackground()).Single());
			});
		}
#endif
	}

	public static class ShellExtension
	{
		public static BadgeViewModel ShellContentBadgeViewModel(this Shell shell, string title) =>
			(BadgeViewModel)shell.Items
			.SelectMany(x => x.Items)
			.SelectMany(x => x.Items)
			.Single(x => x.Title == title)
			.BindingContext;

		public static BadgeViewModel ShellSectionBadgeViewModel(this Shell shell, string title) => 
			(BadgeViewModel)shell.Items
			.SelectMany(x => x.Items)
			.Single(x => x.Title == title)
			.BindingContext;

		public static BadgeViewModel ShellItemBadgeViewModel(this Shell shell, string title) =>
			(BadgeViewModel)shell.Items
			.Single(x => x.Title == title)
			.BindingContext;
	}

#if UITEST && __IOS__

	public static class AppQueryExtension
	{
		// AKA Top Badge
		public static AppQuery ContentBadge(this AppQuery query, string content = "Content 111")
		{
			return query.Raw($"view marked:'{content}' parent view index:0");
		}

		// AKA Bottom Badge
		public static AppQuery SectionBadge(this AppQuery query, string section = "Section 11")
		{
			return query.Raw($"UITabBarButton marked:'{section}'");
		}

		// AKA Flyout Badge
		public static AppQuery ItemBadge(this AppQuery query, string item = "Item 1")
		{
			return query.Raw($"view marked:'{item}'");
		}

		public static AppQuery ContentBadgeText(this AppQuery query, string text)
		{
			return query.Raw("descendant UILabel index:1").Text(text);
		}

		public static AppQuery SectionBadgeText(this AppQuery query, string text)
		{
			return query.Class("UILabel").Index(1).Text(text);
		}

		public static AppQuery ItemBadgeText(this AppQuery query, string text)
		{
			return query.Raw("descendant UILabel index:1").Text(text);
		}

		public static AppTypedSelector<string> ContentBadgeTextColor(this AppQuery query)
		{
			return query.Raw("descendant UILabel index:1").Invoke("color").Invoke("styleString").Value<string>();
		}

		public static AppTypedSelector<string> SectionBadgeTextColor(this AppQuery query)
		{
			return query.Class("UILabel").Index(1).Invoke("color").Invoke("styleString").Value<string>();
		}	

		public static AppTypedSelector<string> ItemBadgeTextColor(this AppQuery query)
		{
			return query.Raw("descendant UILabel index:1").Invoke("color").Invoke("styleString").Value<string>();
		}

		public static AppTypedSelector<string> ContentBadgeBackground(this AppQuery query)
		{
			// TODO: Find a way to get the background color of the sublayer inserted by brush extensions
			throw new NotImplementedException();
		}

		public static AppTypedSelector<string> SectionBadgeBackground(this AppQuery query)
		{
			return query.Raw("descendant view:'_UIBadgeView'").Invoke("backgroundColor").Invoke("styleString").Value<string>();
		}

		public static AppTypedSelector<string> ItemBadgeBackground(this AppQuery query)
		{
			// TODO: Find a way to get the background color of the sublayer inserted by brush extensions
			throw new NotImplementedException();
		}
	}

#endif

#if UITEST && __ANDROID__

	public static class AppQueryExtension
	{
		// AKA Top Badge
		public static AppQuery ContentBadge(this AppQuery query, string content = "Content 111")
		{
			return query.Class("AppBarLayout").Descendant().Marked(content);
		}

		// AKA Bottom Badge
		public static AppQuery SectionBadge(this AppQuery query, string section = "Section 11")
		{
			return query.Class("BottomNavigationView").Descendant().Marked(section);
		}

		// AKA Flyout Badge
		public static AppQuery ItemBadge(this AppQuery query, string item = "Item 1")
		{
			return query.Raw($"* marked:'{item}' FrameRenderer");
		}

		public static AppQuery ContentBadgeText(this AppQuery query, string text)
		{
			return query.Class("TextView").Text(text);
		}

		public static AppQuery SectionBadgeText(this AppQuery query, string text) => query.ContentBadgeText(text);

		public static AppQuery ItemBadgeText(this AppQuery query, string text)
		{
			return query.Class("LabelRenderer").Text(text);
		}

		public static AppTypedSelector<int> ContentBadgeTextColor(this AppQuery query)
		{
			return query.Class("TextView").Invoke("getCurrentTextColor").Value<int>();
		}

		public static AppTypedSelector<int> SectionBadgeTextColor(this AppQuery query) => query.ContentBadgeTextColor();

		public static AppTypedSelector<int> ItemBadgeTextColor(this AppQuery query)
		{
			return query.Class("LabelRenderer").Invoke("getCurrentTextColor").Value<int>();
		}

		public static AppTypedSelector<int> ContentBadgeBackground(this AppQuery query)
		{
			return query.Class("BadgeHelper_BadgeFrameLayout").Invoke("getBackground").Invoke("getPaint").Invoke("getColor").Value<int>();
		}

		public static AppTypedSelector<int> SectionBadgeBackground(this AppQuery query) => query.ContentBadgeBackground();

		public static AppTypedSelector<int> ItemBadgeBackground(this AppQuery query)
		{
			return query.Invoke("getBackground").Invoke("getColor").Invoke("getDefaultColor").Value<int>();
		}
	}
#endif

	public class BadgeViewModel : INotifyPropertyChanged
	{
		string _text;

		public string Text
		{
			get => _text;
			set
			{
				_text = value;
				OnPropertyChanged();
			}
		}

		Color _textColor;

		public Color TextColor
		{
			get => _textColor;
			set
			{
				_textColor = value;
				OnPropertyChanged();
			}
		}

		Brush _brush;

		public Brush Background
		{
			get => _brush;
			set
			{
				_brush = value;
				OnPropertyChanged();
			}
		}

		public event PropertyChangedEventHandler PropertyChanged;

		protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}
	}
}