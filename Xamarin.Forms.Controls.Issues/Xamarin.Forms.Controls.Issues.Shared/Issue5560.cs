using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using Xamarin.Forms.CustomAttributes;
using Xamarin.Forms.Internals;
#if UITEST
using Xamarin.UITest;
using NUnit.Framework;
#endif

namespace Xamarin.Forms.Controls.Issues
{
	[Preserve(AllMembers = true)]
	[Issue(IssueTracker.Github, 5560, "[Android] Disposed EntryCell throws ObjectDisposed exception after updating an object that the EntryCell was previously bound to", PlatformAffected.Android)]
	public class Issue5560 : TestNavigationPage
	{
		protected override void Init()
		{
			Navigation.PushAsync(new NavigationPage(new Page1()));
		}

		[Preserve(AllMembers = true)]
		class Page1 : ContentPage
		{
			public Entry SharedObject { get; } = new Entry { Text = "test" };

			public Page1()
			{
				var button1 = new Button { Text = "Open leaking page" };
				button1.AutomationId = "button1";
				button1.Clicked += Button1_Clicked;

				var button2 = new Button { Text = "GC" };
				button2.AutomationId = "button2";
				button2.Clicked += Button2_Clicked;

				var entry = new Entry();
				entry.AutomationId = "entry";
				entry.SetBinding(Entry.TextProperty, new Binding("Text", source: SharedObject));

				Content = new StackLayout
				{
					Children = {
						button1,
						button2,
						entry
					}
				};
			}

			void Button2_Clicked(object sender, EventArgs e)
			{
				GC.Collect();
				GC.WaitForPendingFinalizers();
				GC.Collect();
			}

			void Button1_Clicked(object sender, EventArgs e)
			{
				Navigation.PushAsync(new Page2(SharedObject));
			}
		}

		[Preserve(AllMembers = true)]
		class Page2 : ContentPage
		{
			public object SharedObject { get; }

			public Page2(object sharedObject)
			{
				SharedObject = sharedObject;

				var item = new EntryCell { Label = "i leak" };
				item.AutomationId = "entrycell";
				item.SetBinding(EntryCell.TextProperty, new Binding("Text", source: SharedObject));

				var tableView = new TableView
				{
					Root = new TableRoot
					{
						new TableSection
						{
							item
						}
					}
				};

				Content = new StackLayout
				{
					Children = {
						tableView
					}
				};
			}
		}

#if UITEST && __ANDROID__
		[Test]
		public void Issue5560Test()
		{
			RunningApp.WaitForElement(q => q.Marked("NoResourceEntry-112"));
			RunningApp.Tap(q => q.Marked("NoResourceEntry-112"));
			RunningApp.Tap(c => c.Class("EntryCellEditText"));
			RunningApp.EnterText(c => c.Class("EntryCellEditText"), " edit");
			RunningApp.Back();
			RunningApp.Back();
			RunningApp.Tap(q => q.Marked("NoResourceEntry-113"));
			RunningApp.Tap(q => q.Marked("NoResourceEntry-114"));
			RunningApp.EnterText(c => c.Marked("NoResourceEntry-114"), " twice");

			RunningApp.Screenshot("EntryCell did not crash app by disposing shared resource");
		}
#endif

	}
}
