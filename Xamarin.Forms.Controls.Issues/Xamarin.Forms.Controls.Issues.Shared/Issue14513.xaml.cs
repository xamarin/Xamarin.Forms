using System.Collections.ObjectModel;
using Xamarin.Forms.CustomAttributes;
using Xamarin.Forms.Internals;

#if UITEST
using Xamarin.UITest;
using NUnit.Framework;
using Xamarin.Forms.Core.UITests;
#endif

namespace Xamarin.Forms.Controls.Issues
{
#if UITEST
	[Category(UITestCategories.CollectionView)]
#endif
	[Preserve(AllMembers = true)]
	[Issue(IssueTracker.Github, 14513,
		"[Bug] [iOS] SelectedItems custom image not displaying in iOS for CollectionView until tapped",
		PlatformAffected.iOS)]
	public partial class Issue14513 : TestContentPage
	{
		public Issue14513()
		{
#if APP
			InitializeComponent();
#endif
		}

		protected override void Init()
		{
			BindingContext = new Issue14513ViewModel();
		}
	}

	public class Issue14513Model
	{
		public string Name { get; set; }
	}

	public class Issue14513ViewModel : BindableObject
	{
		Issue14513Model _selectedItem;

		public Issue14513ViewModel()
		{
			Items = new ObservableCollection<Issue14513Model>
			{
				new Issue14513Model { Name = "Item 1" },
				new Issue14513Model { Name = "Item 2" },
				new Issue14513Model { Name = "Item 3" }
			};

			SelectedItem = Items[1];
		}

		public ObservableCollection<Issue14513Model> Items { get; set; }

		public Issue14513Model SelectedItem
		{
			get { return _selectedItem; }
			set
			{
				_selectedItem = value;
				OnPropertyChanged();
			}
		}
	}
}