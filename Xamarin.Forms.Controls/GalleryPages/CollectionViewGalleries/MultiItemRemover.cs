using System.Linq;

namespace Xamarin.Forms.Controls.GalleryPages.CollectionViewGalleries
{
	internal class MultiItemRemover : MultiTestObservableCollectionModifier
	{
		private readonly bool _withIndex;

		public MultiItemRemover (CollectionView cv, bool withIndex = false) : base(cv, "Remove")
		{
			Entry.Keyboard = Keyboard.Default;
			_withIndex = withIndex;
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

		protected override void ModifyObservableCollection(MultiTestObservableCollection<CollectionViewGalleryTestItem> observableCollection, params int[] indexes)
		{
			if (indexes.Length < 2)
			{
				return;
			}

			var index1 = indexes[0];
			var index2 = indexes[1];

			if (index1 > -1 && index2 > -1 && index1 < observableCollection.Count &&
				index2 < observableCollection.Count && index1 < index2)
			{
				if (_withIndex)
				{
					observableCollection.TestRemoveWithListAndIndex(index1, index2 - index1);
				}
				else
				{
					observableCollection.TestRemoveWithList(index1, index2 - index1);
				}
			}
		}
	}
}