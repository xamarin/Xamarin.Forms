using Xamarin.Forms.CustomAttributes;
using Xamarin.Forms.Internals;
using System.Collections.ObjectModel;
using System;

#if UITEST
using Xamarin.Forms.Core.UITests;
using Xamarin.UITest;
using NUnit.Framework;
#endif

namespace Xamarin.Forms.Controls.Issues
{
#if UITEST
	[Category(UITestCategories.ManualReview)]
#endif
	[Preserve(AllMembers = true)]
	[Issue(IssueTracker.Github, 15647, "[Bug] Android ListView SelectedItem highlighting", PlatformAffected.Android)]
	public class Issue15647 : TestContentPage
	{
		protected override void Init()
		{
			ViewModelIssue15647 viewModelIssue15647 = new ViewModelIssue15647();

			Label testInstructionsLabel = new Label()
			{
				HorizontalTextAlignment = TextAlignment.Center,
				Padding = new Thickness(3.0),
				Text = "Click on the buttons to select the first or second item of the ListView." + Environment.NewLine +
					"The correct item has to be marked as selected so that this test is successful.",
			};

			DataTemplate itemDataTemplate = new DataTemplate(() =>
			{
				Label itemLabel = new Label();
				itemLabel.SetBinding(Label.TextProperty, ".");
				return new ViewCell()
				{
					View = itemLabel,
				};
			});
			ListView itemsListView = new ListView()
			{
				// Header = "Header",
				ItemTemplate = itemDataTemplate,
			};
			itemsListView.SetBinding(ListView.ItemsSourceProperty, nameof(ViewModelIssue15647.Items));

			Button selectFirstItemButton = new Button()
			{
				Text = "Select first item",
			};
			selectFirstItemButton.Clicked += (_, __) =>
			{
				Console.WriteLine(itemsListView.SelectedItem);
				itemsListView.SelectedItem = viewModelIssue15647.Items[0];
				Console.WriteLine(itemsListView.SelectedItem);
			};

			Button selectSecondItemButton = new Button()
			{
				Text = "Select second item",
			};
			selectSecondItemButton.Clicked += (_, __) =>
			{
				Console.WriteLine(itemsListView.SelectedItem);
				itemsListView.SelectedItem = viewModelIssue15647.Items[1];
				Console.WriteLine(itemsListView.SelectedItem);
			};

			Content = new StackLayout()
			{
				Children = { testInstructionsLabel, selectFirstItemButton, selectSecondItemButton, itemsListView },
			};

			BindingContext = viewModelIssue15647;
		}
	}

	[Preserve(AllMembers = true)]
	public class ViewModelIssue15647
	{
		public ViewModelIssue15647()
		{
			Items = new ObservableCollection<string>() { "first item", "second item" };
		}

		public ObservableCollection<string> Items { get; }
	}
}
