using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using Xamarin.Forms.CustomAttributes;
using Xamarin.Forms.Internals;

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
				button1.Clicked += Button1_Clicked;

				var button2 = new Button { Text = "GC" };
				button2.Clicked += Button2_Clicked;

				var entry = new Entry();
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
	}
}
