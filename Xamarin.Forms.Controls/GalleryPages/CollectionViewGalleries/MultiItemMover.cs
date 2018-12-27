namespace Xamarin.Forms.Controls.GalleryPages.CollectionViewGalleries
{
	internal class MultiItemMover : MultiTestObservableCollectionModifier
	{
		public MultiItemMover(CollectionView cv) : base(cv, "Move")
		{
			Entry.Keyboard = Keyboard.Default;
		}

		protected override bool ParseIndexes(out int[] indexes)
		{
			return IndexParser.ParseIndexes(Entry.Text, 3, out indexes);
		}

		protected override void ModifyObservableCollection(MultiTestObservableCollection<CollectionViewGalleryTestItem> observableCollection, params int[] indexes)
		{
			if (indexes.Length < 2)
			{
				return;
			}

			var startIndex = indexes[0];
			var endIndex = indexes[1];
			var destinationIndex = indexes[2];

			if (startIndex  > -1 && endIndex > -1 && startIndex < observableCollection.Count &&
				endIndex < observableCollection.Count && startIndex < endIndex &&
				observableCollection.Count - (endIndex - startIndex) > destinationIndex)
			{
				observableCollection.TestMoveWithList(startIndex, endIndex - startIndex, destinationIndex);
			}
		}
	}
}