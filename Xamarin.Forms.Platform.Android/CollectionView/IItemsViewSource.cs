using System;

namespace Xamarin.Forms.Platform.Android
{
	internal interface IItemsViewSource : IDisposable
	{
		int Count { get; }

		int GetPosition(object item);
		object GetItem(int position);

		bool HasHeader { get; set; }
		bool HasFooter { get; set; }

		bool IsHeader(int index);
		bool IsFooter(int index);
	}
}