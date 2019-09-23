using System.Collections.ObjectModel;
using Xamarin.Forms.CustomAttributes;
using Xamarin.Forms.Internals;
using System;
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
	[Issue(IssueTracker.Github, 5455, "ItemSizingStrategy MeasureAllItems does not work for iOS", PlatformAffected.iOS)]
	public partial class Issue5455 : TestContentPage
	{
		bool isMeasuringAllItems = false;

#if APP
		public Issue5455()
		{
			Device.SetFlags(new List<string> { CollectionView.CollectionViewExperimental });

			InitializeComponent();

			BindingContext = new ViewModel5455();
		}
#endif

		protected override void Init()
		{

		}

		private void ButtonClicked(object sender, EventArgs e)
		{
			var button = sender as Button;
			var grid = button.Parent.Parent as Grid;
			var collectionView = grid.Children[1] as CollectionView;

			isMeasuringAllItems = !isMeasuringAllItems;

			collectionView.ItemSizingStrategy = isMeasuringAllItems ? ItemSizingStrategy.MeasureAllItems : ItemSizingStrategy.MeasureFirstItem;
			button.Text = isMeasuringAllItems ? "Switch to MeasureFirstItem" : "Switch to MeasureAllItems";
		}
	}

	[Preserve(AllMembers = true)]
	public class ViewModel5455
	{
		public ObservableCollection<Model5455> Items { get; set; }

		public ViewModel5455()
		{
			var collection = new ObservableCollection<Model5455>();
			Color[] _colors =
			{
				Color.Red,
				Color.Blue,
				Color.Green,
				Color.Yellow
			};
			string[] _images =
			{
				"cover1.jpg",
				"oasis.jpg",
				"photo.jpg",
				"Vegetables.jpg"
			};

			for (var i = 0; i < 30; i++)
			{
				collection.Add(new Model5455
				{
					BackgroundColor = _colors[i % 4],
					Source = _images[i % 4]
				});
			}

			Items = collection;
		}
	}

	[Preserve(AllMembers = true)]
	public class Model5455
	{
		public Color BackgroundColor { get; set; }

		public string Source { get; set; }
	}
}