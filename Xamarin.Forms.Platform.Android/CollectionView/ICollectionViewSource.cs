namespace Xamarin.Forms.Platform.Android
{
	internal interface ICollectionViewSource
	{
		// TODO hartez 2018/07/30 15:06:20 Maybe this should be ItemsViewSource?
		// Depends on where we settle for the name of CollectionViewAdapter (ItemsViewAdapter?)

		int Count { get; }
		object this[int index] { get; }
	}
}