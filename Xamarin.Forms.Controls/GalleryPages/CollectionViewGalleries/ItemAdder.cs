using System;
using System.Collections.ObjectModel;

namespace Xamarin.Forms.Controls.GalleryPages.CollectionViewGalleries
{
	internal class ItemAdder : CollectionModifier 
	{
		public ItemAdder(CollectionView cv) : base(cv, "Insert")
		{
		}

		protected override void ModifyCollection(ObservableCollection<TestItem> observableCollection, params int[] indexes)
		{
			var index = indexes[0];

			if (index > -1 && index < observableCollection.Count)
			{
				var item = new TestItem { Image = "oasis.jpg", Date = $"{DateTime.Now.ToLongDateString()}", Caption = "Inserted"};
				observableCollection.Insert(index, item);
			}
		}
	}
}