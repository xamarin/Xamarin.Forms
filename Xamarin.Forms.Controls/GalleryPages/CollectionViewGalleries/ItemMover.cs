using System.Collections.ObjectModel;
using System.Linq;

namespace Xamarin.Forms.Controls.GalleryPages.CollectionViewGalleries
{
	internal class ItemMover : ObservableCollectionModifier
	{
		public ItemMover(CollectionView cv) : base(cv, "Move")
		{
			Entry.Keyboard = Keyboard.Default;
		}

		static int ParseToken(string value)
		{
			if (!int.TryParse(value, out int index))
			{
				return -1;
			}

			return index;
		}

		protected override bool ParseIndexes(out int[] indexes)
		{
			var text = Entry.Text;

			indexes = text.Split(',').Select(v => ParseToken(v.Trim())).ToArray();

			return indexes.Length == 2;
		}

		protected override void ModifyObservableCollection(ObservableCollection<CollectionViewGalleryTestItem> observableCollection, params int[] indexes)
		{
			if (indexes.Length < 2)
			{
				return;
			}

			var index1 = indexes[0];
			var index2 = indexes[1];

			if (index1 > -1 && index2 > -1 && index1 < observableCollection.Count &&
				index2 < observableCollection.Count)
			{
				observableCollection.Move(index1, index2);
			}
		}
	}
}