using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading;
using System.Threading.Tasks;
using Xamarin.Forms.CustomAttributes;
using Xamarin.Forms.Internals;
using Xamarin.Forms.Xaml;
using System;
using System.Security.Cryptography;

#if UITEST
using Xamarin.UITest;
using NUnit.Framework;
using Xamarin.Forms.Core.UITests;
using System.Linq;
#endif

namespace Xamarin.Forms.Controls.Issues
{
#if UITEST
	[Category(UITestCategories.CollectionView)]
#endif
#if APP
	[XamlCompilation(XamlCompilationOptions.Compile)]
#endif
	[Preserve(AllMembers = true)]
	[Issue(IssueTracker.Github, 5623, "CollectionView with Incremental Collection (RemainingItemsThreshold)", PlatformAffected.All)]
	public partial class Github5623 : TestContentPage
	{
		int _itemCount = 10;
		int _maximumItemCount = 100;
		int _pageSize = 10;
		static SemaphoreSlim SemaphoreSlim = new SemaphoreSlim(1, 1);

		public Github5623()
		{
#if APP
			Device.SetFlags(new List<string> { CollectionView.CollectionViewExperimental });

			InitializeComponent();

			BindingContext = new ViewModel5623();
#endif
		}

		protected override void Init()
		{

		}

		async void CollectionView_RemainingItemsThresholdReached(object sender, System.EventArgs e)
		{
			await SemaphoreSlim.WaitAsync();
			try
			{
				var itemsSource = (sender as CollectionView).ItemsSource as ObservableCollection<Model5623>;
				var nextSet = await GetNextSetAsync();

				// nothing to add
				if (nextSet.Count == 0)
					return;

				Device.BeginInvokeOnMainThread(() =>
				{
					foreach (var item in nextSet)
					{
						itemsSource.Add(item);
					}
				});

				System.Diagnostics.Debug.WriteLine("Count: " + itemsSource.Count);
			}
			finally
			{
				SemaphoreSlim.Release();
			}
		}

		void CollectionView_OnScrolled(object sender, ItemsViewScrolledEventArgs e)
		{
			Label1.Text = "HorizontalDelta: " + e.HorizontalDelta;
			Label2.Text = "VerticalDelta: " + e.VerticalDelta;
			Label3.Text = "HorizontalOffset: " + e.HorizontalOffset;
			Label4.Text = "VerticalOffset: " + e.VerticalOffset;
			Label5.Text = "FirstVisibleItemIndex: " + e.FirstVisibleItemIndex;
			Label6.Text = "CenterItemIndex: " + e.CenterItemIndex;
			Label7.Text = "LastVisibleItemIndex: " + e.LastVisibleItemIndex;
		}

		async Task<ObservableCollection<Model5623>> GetNextSetAsync()
		{
			return await Task.Run(() =>
			{
				var collection = new ObservableCollection<Model5623>();
				var count = _pageSize;

				if (_itemCount + count > _maximumItemCount)
					count = _maximumItemCount - _itemCount;

				for (var i = _itemCount; i < _itemCount + count; i++)
				{
					collection.Add(new Model5623((BindingContext as ViewModel5623).ItemSizingStrategy == ItemSizingStrategy.MeasureAllItems)
					{
						Text = i.ToString(),
						BackgroundColor = i % 2 == 0 ? Color.AntiqueWhite : Color.Lavender
					});
				}

				_itemCount += count;

				return collection;
			});
		}

#if UITEST
		[Test]
		public void CollectionViewInfiniteScroll()
		{
			RunningApp.WaitForElement (q => q.Marked ("CollectionView"));
			
			var elementQuery = RunningApp.Query(c => c.Marked((_maximumItemCount - 1).ToString()));
			var elementAvailable = elementQuery.Any();
			var attempt = 0;

			while (!elementAvailable)
			{
				RunningApp.ScrollDown();

				elementAvailable = elementQuery.Any();
				if (elementAvailable)
					break;

				++attempt;

				if (attempt == 30)
				{
					Assert.Fail("Failed to find the last element");
					break;
				}
			}
		}
#endif
	}

	[Preserve(AllMembers = true)]
	public class ViewModel5623
	{
		public ObservableCollection<Model5623> Items { get; set; }

		public Command RemainingItemsThresholdReachedCommand { get; set; }

		public ItemSizingStrategy ItemSizingStrategy { get; set; } = ItemSizingStrategy.MeasureAllItems;

		public ViewModel5623()
		{
			var collection = new ObservableCollection<Model5623>();
			var pageSize = 10;

			for (var i = 0; i < pageSize; i++)
			{
				collection.Add(new Model5623(ItemSizingStrategy == ItemSizingStrategy.MeasureAllItems)
				{
					Text = i.ToString(),
					BackgroundColor = i % 2 == 0 ? Color.AntiqueWhite : Color.Lavender
				});
			}

			Items = collection;

			RemainingItemsThresholdReachedCommand = new Command(() =>
			{
				System.Diagnostics.Debug.WriteLine($"{nameof(RemainingItemsThresholdReachedCommand)} called");
			});
		}
	}

	[Preserve(AllMembers = true)]
	public class Model5623
	{
		RNGCryptoServiceProvider provider = new RNGCryptoServiceProvider();

		public string Text { get; set; }

		public Color BackgroundColor { get; set; }

		public int Height { get; set; } = 200;

		public string HeightText { get; private set; } 

		public Model5623(bool isUneven)
		{
			var byteArray = new byte[4];
			provider.GetBytes(byteArray);

			if(isUneven)
				Height = 100 + (BitConverter.ToInt32(byteArray, 0) % 300 + 300) % 300;

			HeightText = "(Height: " + Height + ")";
		}
	}
}