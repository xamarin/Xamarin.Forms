using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading;
using System.Threading.Tasks;
using Xamarin.Forms.CustomAttributes;
using Xamarin.Forms.Internals;
using Xamarin.Forms.Xaml;

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
		int _itemCount = 100;
		int _maximumItemCount = 1000;
		int _pageSize = 100;
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

		private async void CollectionView_RemainingItemsThresholdReached(object sender, System.EventArgs e)
		{
			await SemaphoreSlim.WaitAsync();
			try
			{
				var itemsSource = (sender as CollectionView).ItemsSource as ObservableCollection<Model5623>;

				if (itemsSource.Count == _maximumItemCount)
				{
					System.Diagnostics.Debug.WriteLine("Count: " + itemsSource.Count);
					return;
				}

				var nextSet = await GetNextSetAsync();

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

		async Task<ObservableCollection<Model5623>> GetNextSetAsync()
		{
			return await Task.Run(() =>
			{
				var collection = new ObservableCollection<Model5623>();

				for (var i = _itemCount + 1; i <= _itemCount + _pageSize; i++)
				{
					collection.Add(new Model5623
					{
						Text = i.ToString(),
						BackgroundColor = i % 2 == 0 ? Color.AntiqueWhite : Color.Lavender
					});
				}

				_itemCount += _pageSize;

				return collection;
			});
		}

#if UITEST
		[Test]
		public void CollectionViewInfiniteScroll()
		{
			RunningApp.WaitForElement (q => q.Marked ("CollectionView"));
			
			var elementQuery = RunningApp.Query(c => c.Marked(_maximumItemCount.ToString()));
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

		public ViewModel5623()
		{
			var collection = new ObservableCollection<Model5623>();
			var pageSize = 100;

			for (var i = 0; i < pageSize; i++)
			{
				collection.Add(new Model5623
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
		public string Text { get; set; }

		public Color BackgroundColor { get; set; }
	}
}