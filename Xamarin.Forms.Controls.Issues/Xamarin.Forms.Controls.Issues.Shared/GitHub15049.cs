using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms.CustomAttributes;
using Xamarin.Forms.Internals;

#if UITEST
using Xamarin.UITest;
using NUnit.Framework;
#endif

namespace Xamarin.Forms.Controls.Issues
{
#if UITEST
	[NUnit.Framework.Category(Core.UITests.UITestCategories.Github10000)]
	[NUnit.Framework.Category(Core.UITests.UITestCategories.CollectionView)]
	[NUnit.Framework.Category(Core.UITests.UITestCategories.ManualReview)]
#endif
	[Preserve(AllMembers = true)]
	[Issue(IssueTracker.Github, 15049, "[Android] CollectionView leaks GREF when items are removed", PlatformAffected.Android, issueTestNumber: 1)]
	public class GitHub15049 : TestContentPage
	{
		protected override void Init()
		{
			BindingContext = new GitHub15049ViewModel();
			var collectionView = new CollectionView();
			collectionView.ItemsLayout = new LinearItemsLayout(ItemsLayoutOrientation.Vertical);
			collectionView.ItemTemplate = new DataTemplate(() =>
			 {
				 var label = new Label();
				 label.SetBinding(Label.TextProperty, "Text");

				 return label;
			 });
			collectionView.SetBinding(CollectionView.ItemsSourceProperty, "Items");
			Content = collectionView;
		}

		public class GitHub15049ViewModel : INotifyPropertyChanged
		{
			public event PropertyChangedEventHandler PropertyChanged;
			private ObservableCollection<GitHub15049Model> _items;
			public ObservableCollection<GitHub15049Model> Items
			{
				get => _items;
				set
				{
					if (_items != value)
					{
						_items = value;
						RaisePropertyChanged(nameof(Items));
					}
				}
			}

			public bool IsStopped { get; set; } = false;

			public GitHub15049ViewModel()
			{
				var collection = new ObservableCollection<GitHub15049Model>();
				var pageSize = 10000;

				for (var i = 0; i < pageSize; i++)
				{
					collection.Add(new GitHub15049Model
					{
						Text = "Item " + i,
					});
				}

				Items = collection;

				//Kick off Test
				Task.Run(async () =>
				{
					while (Items.Count > 0 && !IsStopped)
					{
						await Task.Yield();

						await Device.InvokeOnMainThreadAsync(() =>
						{
							Items.RemoveAt(0);
						});
					}
				});
			}

			protected virtual void RaisePropertyChanged([CallerMemberName] string propertyName = null)
			{
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
			}
		}

		public class GitHub15049Model
		{
			public string Text { get; set; }
		}
	}
}
