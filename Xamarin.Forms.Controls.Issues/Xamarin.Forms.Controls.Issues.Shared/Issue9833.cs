using System.Collections.ObjectModel;
using Xamarin.Forms.CustomAttributes;
using Xamarin.Forms.Internals;

#if UITEST
using NUnit.Framework;
using Xamarin.Forms.Core.UITests;
#endif

namespace Xamarin.Forms.Controls.Issues
{
	[Issue(IssueTracker.Github, 9833, "[Bug] [UWP] Propagate CollectionView BindingContext to EmptyView",
		PlatformAffected.UWP)]
#if UITEST
	[Category(UITestCategories.CollectionView)]
	[Category(UITestCategories.UwpIgnore)] // CollectionView contents aren't currently visible to test automation
#endif
	public class Issue9833 : TestContentPage
	{
		readonly CollectionView _collectionView;
		readonly Label _emptyLabel;
		const string Success = "Success";

		public Issue9833()
		{
			Title = "Issue 9833";

			BindingContext = new Issue9833ViewModel();

			var layout = new StackLayout
			{
				Padding = 0
			};

			var instructions = new Label
			{
				Text = $"If the EmptyView BindingContext is not null, EmptyView will display a Label with the text {Success}, and this test will pass.",
				BackgroundColor = Color.Black,
				TextColor = Color.White
			};

			_collectionView = new CollectionView();
			_collectionView.SetBinding(ItemsView.ItemsSourceProperty, "Items");

			var emptyView = new StackLayout
			{
				BackgroundColor = Color.LightGray
			};

			_emptyLabel = new Label
			{
				Text = "This is the EmptyView. "
			};

			emptyView.Children.Add(_emptyLabel);

			_collectionView.EmptyView = emptyView;

			layout.Children.Add(instructions);
			layout.Children.Add(_collectionView);

			Content = layout;
		}

		protected override void Init()
		{

		}

		protected override void OnAppearing()
		{
			base.OnAppearing();

			var emptyView = (View)_collectionView.EmptyView;
			_emptyLabel.Text = emptyView.BindingContext == null ? "Failed" : Success;
		}

#if UITEST
		[Test]
		public void EmptyViewBindingContextShouldNotBeNull()
		{
			RunningApp.WaitForElement(Success);
		}
#endif
	}

	public class Issue9833ViewModel : BindableObject
	{
		ObservableCollection<string> _items;

		public Issue9833ViewModel()
		{
			LoadData();
		}

		public ObservableCollection<string> Items
		{
			get { return _items; }
			set
			{
				_items = value;
				OnPropertyChanged();
			}
		}

		void LoadData()
		{
			Items = new ObservableCollection<string>();
		}
	}
}
