using Xamarin.Forms.CustomAttributes;
using Xamarin.Forms.Internals;
using System.Collections.ObjectModel;
using System;
using System.Diagnostics;

#if UITEST
using Xamarin.Forms.Core.UITests;
using Xamarin.UITest;
using NUnit.Framework;
#endif

namespace Xamarin.Forms.Controls.Issues
{
#if UITEST
	[Category(UITestCategories.CollectionView)]
#endif
	[Preserve(AllMembers = true)]
	[Issue(
		IssueTracker.Github,
		13429,
		"[Bug] Xamarin.Forms 5.0.0.x ObservableCollection.Add performance decrease on iOS",
		PlatformAffected.iOS)]
	public class Issue13429 : TestContentPage
	{
		ObservableCollection<Item> _items;
		Label _statusLabel, _timeoutLabel;
		int _itemIndex;
		const int INITIAL_SIZE = 1000;
		const int DELTA_SIZE = INITIAL_SIZE / 5;
		const string ADDITION_COMPLETED = "addition completed";
		const string REMOVAL_COMPLETED = "removal completed";
		const string REPLACEMENT_COMPLETED = "replacement completed";
		const string ADD = "Add";
		const string REMOVE = "Remove";
		const string REPLACE = "Replace";
		const string TIMEOUT_ID = nameof(_timeoutLabel);

		public Issue13429()
		{
		}
			
		protected override void Init()
		{
			Button addButton, removeButton, replaceButton;
			CollectionView collectionView;	
			Title = "Bug 13429";
			_items = new ObservableCollection<Item>();
			var grid = new Grid()
			{
				ColumnDefinitions = new ColumnDefinitionCollection
				{
					new ColumnDefinition { Width = GridLength.Star },
					new ColumnDefinition { Width = GridLength.Star },
					new ColumnDefinition { Width = GridLength.Star },
				},
				RowDefinitions = new RowDefinitionCollection
				{
					new RowDefinition { Height = GridLength.Auto },
					new RowDefinition { Height = GridLength.Auto },
					new RowDefinition { Height = GridLength.Auto },
					new RowDefinition { Height = GridLength.Star },
				},
				Children =
				{
					(addButton = new Button { Text = ADD }),
					(removeButton = new Button { Text = REMOVE }),
					(replaceButton = new Button { Text = REPLACE }),
					(_statusLabel = new Label { Text = "Waiting" }),
					(_timeoutLabel = new Label { Text = "", AutomationId = TIMEOUT_ID }),
					(collectionView = new CollectionView
					{
						ItemTemplate = new DataTemplate(() =>
						{
							Label name, description;
							var sl = new StackLayout
							{
								Children =
								{
									(name = new Label()),
									(description = new Label())
								}
							};
							name.SetBinding(Label.TextProperty, new Binding(nameof(Item.Name)));
							description.SetBinding(Label.TextProperty, new Binding(nameof(Item.Description)));
							return sl;
						}),
						ItemsSource = _items,
					})
				}
			};

			Grid.SetColumn(addButton, 0);
			Grid.SetColumn(removeButton, 1);
			Grid.SetColumn(replaceButton, 2);

			Grid.SetRow(_statusLabel, 1);
			Grid.SetColumnSpan(_statusLabel, 3);

			Grid.SetRow(_timeoutLabel, 2);
			Grid.SetColumnSpan(_timeoutLabel, 3);

			Grid.SetRow(collectionView, 3);
			Grid.SetColumnSpan(collectionView, 3);

			grid.Children.Add(addButton);
			grid.Children.Add(removeButton);
			grid.Children.Add(replaceButton);
			grid.Children.Add(collectionView);

			addButton.Clicked += (o, e) => AddItems();
			removeButton.Clicked += (o, e) => RemoveItems();
			replaceButton.Clicked += (o, e) => ReplaceItems();

			for (int i = 0; i < INITIAL_SIZE; i++)
				_items.Add(NewItem());
			Content = grid;
		}

		Item NewItem()
		{
			var result = new Item($"item #{_itemIndex}", $"description of #{_itemIndex} {Guid.NewGuid()}");
			_itemIndex++;
			return result;
		}

		void AddItems(int count = DELTA_SIZE)
		{
			IsBusy = true;
			_statusLabel.Text = "adding items";
			var sw = new Stopwatch();
			sw.Start();
			for (int i = 0; i < count; i++)
				_items.Add(NewItem());
			_timeoutLabel.Text = sw.ElapsedMilliseconds.ToString();
			_statusLabel.Text = ADDITION_COMPLETED;
			IsBusy = false;
		}

		void RemoveItems(int count = DELTA_SIZE)
		{
			IsBusy = true;
			_statusLabel.Text = "removing items";
			var sw = new Stopwatch();
			sw.Start();
			if (_items.Count < count)
				count = _items.Count;
			while (count-- > 0)
				_items.RemoveAt(_items.Count - 1);
			_timeoutLabel.Text = sw.ElapsedMilliseconds.ToString();
			_statusLabel.Text = REMOVAL_COMPLETED;
			IsBusy = false;
		}

		void ReplaceItems(int count = DELTA_SIZE)
		{
			IsBusy = true;
			_statusLabel.Text = "replacing items";
			var sw = new Stopwatch();
			sw.Start();
			for (int i = _items.Count - count; i < _items.Count; i++)
				_items[i] = NewItem();
			_timeoutLabel.Text = sw.ElapsedMilliseconds.ToString();
			_statusLabel.Text = REPLACEMENT_COMPLETED;
			IsBusy = false;
		}

		public class Item
		{
			public string Name { get; }
			public string Description { get; }

			public Item(string name, string description)
			{
				Name = name;
				Description = description;
			}
		}

#if UITEST
		const long EXPECTED_TIMEOUT = 2000;

		[Test]
		public void CollectionViewItemAddtionPerformanceCheck()
		{
			RunningApp.Tap(ADD);
			RunningApp.WaitForElement(q => q.Marked(ADDITION_COMPLETED));
			var actualTimeout = long.Parse(RunningApp.WaitForElement(q => q.Marked(TIMEOUT_ID))[0].Text);
			Assert.Less(actualTimeout, EXPECTED_TIMEOUT);
		}
		[Test]
		public void CollectionViewItemRemovalPerformanceCheck()
		{
			RunningApp.Tap(REMOVE);
			RunningApp.WaitForElement(q => q.Marked(REMOVAL_COMPLETED));
			var actualTimeout = long.Parse(RunningApp.WaitForElement(q => q.Marked(TIMEOUT_ID))[0].Text);
			Assert.Less(actualTimeout, EXPECTED_TIMEOUT);
		}
		[Test]
		public void CollectionViewItemReplacementPerformanceCheck()
		{
			RunningApp.Tap(REPLACE);
			RunningApp.WaitForElement(q => q.Marked(REPLACEMENT_COMPLETED));
			var actualTimeout = long.Parse(RunningApp.WaitForElement(q => q.Marked(TIMEOUT_ID))[0].Text);
			Assert.Less(actualTimeout, EXPECTED_TIMEOUT);
		}
#endif
	}
}