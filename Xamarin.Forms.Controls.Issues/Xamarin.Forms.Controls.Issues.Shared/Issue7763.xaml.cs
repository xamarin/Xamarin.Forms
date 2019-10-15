using System.Collections.ObjectModel;
using Xamarin.Forms.CustomAttributes;
using Xamarin.Forms.Internals;
using System;
using System.Security.Cryptography;
using Xamarin.Forms.Xaml;
using System.Threading;
using System.Collections.Generic;
using System.Threading.Tasks;

#if UITEST
using Xamarin.UITest;
using Xamarin.UITest.Queries;
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
	[Issue(IssueTracker.Github, 7763, "[CollectionView] Add ability to disable item animations", PlatformAffected.All)]
	public partial class Issue7763 : TestContentPage
	{
#if APP
		public Issue7763()
		{
			Device.SetFlags(new List<string> { CollectionView.CollectionViewExperimental });

			InitializeComponent();

			BindingContext = new ViewModel7763();
		}

		void AddButtonClicked(object sender, EventArgs e)
		{
			var button = sender as Button;
			var grid = button.Parent.Parent.Parent as Grid;
			var collectionView = grid.Children[1] as CollectionView;

			var items = (BindingContext as ViewModel7763).Items;

			items.Insert(0, new Model7763(true));

			collectionView.ScrollTo(0, groupIndex: -1, animate: false);
		}

		void RemoveButtonClicked(object sender, EventArgs e)
		{
			var button = sender as Button;
			var grid = button.Parent.Parent.Parent as Grid;
			var collectionView = grid.Children[1] as CollectionView;

			var items = (BindingContext as ViewModel7763).Items;

			items.RemoveAt(0);

			collectionView.ScrollTo(0, groupIndex: -1, animate: false);
		}

		void NewItemsSourceButtonClicked(object sender, EventArgs e)
		{
			BindingContext = new ViewModel7763();
		}
#endif

		protected override void Init()
		{

		}
	}

	[Preserve(AllMembers = true)]
	public class ViewModel7763
	{
		public ObservableCollection<Model7763> Items { get; set; }

		public ViewModel7763()
		{
			var collection = new ObservableCollection<Model7763>();

			for (var i = 0; i < 15; i++)
			{
				collection.Add(new Model7763(true));
			}

			Items = collection;
		}
	}

	[Preserve(AllMembers = true)]
	public class Model7763
	{
		RNGCryptoServiceProvider provider = new RNGCryptoServiceProvider();

		public int Height { get; set; } = 200;

		public string HeightText { get; private set; }

		public Model7763(bool isUneven)
		{
			var byteArray = new byte[4];
			provider.GetBytes(byteArray);

			if (isUneven)
				Height = 100 + (BitConverter.ToInt32(byteArray, 0) % 300 + 300) % 300;

			HeightText = "(Height: " + Height + ")";
		}
	}
}