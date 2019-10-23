using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using Xamarin.Forms.CustomAttributes;
using Xamarin.Forms.Internals;

namespace Xamarin.Forms.Controls.Issues
{
	[Preserve(AllMembers = true)]
	[Issue(IssueTracker.Github, 5560, "[Android] Disposed EntryCell throws ObjectDisposed exception after updating an object that the EntryCell was previously bound to",
		PlatformAffected.Android)]
	public class Issue5560 : TestNavigationPage
	{
		protected override void Init()
		{
			PushAsync(new MainPage());
		}

		class MainPage : ContentPage
		{
			public Entry SharedObject { get; } = new Entry { Text = "test" };

			protected override void OnAppearing()
			{
				base.OnAppearing();

				BindingContext = SharedObject;

				Title = "Issue5560";

				var stackLayout = new StackLayout();

				var leakButton = new Button()
				{
					Text = "Open leaking page",
				};
				leakButton.Clicked += LeakButton_Clicked;
				stackLayout.Children.Add(leakButton);

				var gcButton = new Button()
				{
					Text = "GC",
				};
				gcButton.Clicked += GcButton_Clicked;
				stackLayout.Children.Add(gcButton);

				var entry = new Entry();
				entry.SetBinding(Entry.TextProperty, "Text");
				stackLayout.Children.Add(entry);

				Content = stackLayout;
			}

			private void GcButton_Clicked(object sender, EventArgs e)
			{
				GC.Collect();
				GC.WaitForPendingFinalizers();
				GC.Collect();
			}

			private void LeakButton_Clicked(object sender, EventArgs e)
			{
				Navigation.PushAsync(new Leaks(SharedObject));
			}
		}

		class Leaks : ContentPage
		{
			public object SharedObject { get; }
			
			public Leaks(object sharedObject)
			{
				SharedObject = sharedObject;
			}

			protected override void OnAppearing()
			{
				base.OnAppearing();
				BindingContext = SharedObject;

				var stackLayout = new StackLayout();
				Content = stackLayout;

				var tableView = new TableView();
				stackLayout.Children.Add(tableView);

				var tableRoot = new TableRoot();
				tableView.Root = tableRoot;

				var tableSection = new TableSection();
				tableRoot.Add(tableSection);

				var entreyCell = new EntryCell();
				entreyCell.SetBinding(EntryCell.TextProperty, "Text");
				tableSection.Add(entreyCell);
			}

			~Leaks()
			{
				Debug.WriteLine("~Leaks() ***************************");
			}
		}
	}
}
