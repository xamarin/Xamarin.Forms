using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using Xamarin.Forms.CustomAttributes;

#if UITEST
using Xamarin.UITest;
using NUnit.Framework;
using Xamarin.Forms.Core.UITests;
#endif

namespace Xamarin.Forms.Controls.Issues
{
	[Issue(IssueTracker.Github, 11853,
		"[Bug][iOS] Concurrent issue leading to crash in SemaphoreSlim.Release in ObservableItemsSource",
		platformsAffected: PlatformAffected.iOS)]
	public class Issue11853 : TestContentPage
	{
		const string Success = "Success";
		const string Run = "Run";

		ObservableCollection<string> _items;

		protected override void Init()
		{
			Title = "Issue 11853";

			var descriptionLabel = new Label { Text = "Press \"Start test\", if the App doesn't crash, it passed." };
			var successLabel = new Label { Text = Success, FontSize = Device.GetNamedSize(NamedSize.Large, typeof(Label)), IsVisible = false };

			var collectionView = new CollectionView();
			ResetItemsSource(collectionView);

			var startButton = new Button { Text = Run };
			startButton.Clicked += (sender, args) =>
			{
				ResetTest(successLabel, collectionView);

				var random = new Random();
				_items.RemoveAt(random.Next(0, 99));
				_items.RemoveAt(random.Next(0, 98));
				_items.RemoveAt(random.Next(0, 97));
				_items.Clear();
				successLabel.IsVisible = true;
			};
			var resetButton = new Button { Text = "Reset test" };
			resetButton.Clicked += (sender, args) =>
			{
				ResetTest(successLabel, collectionView);
			};

			Content = new StackLayout { Children = { descriptionLabel, successLabel, collectionView, startButton, resetButton } };
		}

		void ResetTest(VisualElement successLabel, ItemsView collectionView)
		{
			successLabel.IsVisible = false;
			if (_items.Count != 100)
				ResetItemsSource(collectionView);
		}

		void ResetItemsSource(ItemsView collectionView)
		{
			_items = new ObservableCollection<string>(Create100StringItems());

			void OnItemsOnCollectionChanged(object sender, NotifyCollectionChangedEventArgs args) =>
				System.Diagnostics.Debug.WriteLine("items.CollectionChanged: {0} (new: {1}, old: {2})", args.Action,
					args.NewItems?.Count, args.OldItems?.Count);

			_items.CollectionChanged += OnItemsOnCollectionChanged;

			collectionView.ItemsSource = _items;
		}

		static IEnumerable<string> Create100StringItems()
		{
			// create 100 items from Item 1 - Item 100
			return Enumerable.Range(1, 100).Select(i => $"Item {i}");
		}
		
#if UITEST
		[Category(UITestCategories.CollectionView)]
		[Test]
		public void GivenAnObservableCollection_WhenItemsAreRemovedAndCollectionIsCleared_ThenApplicationDoesNotCrash()
		{
			RunningApp.WaitForElement(Run);
			RunningApp.Tap(Run);
			RunningApp.WaitForElement(Success);
		}
#endif
	}
}