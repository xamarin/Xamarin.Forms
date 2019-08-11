using System;

namespace Xamarin.Forms.Platform.Android
{
	internal interface IItemsViewSource : IDisposable
	{
		int Count { get; }
		object this[int index] { get; }

		bool HasHeader { get; set; }
		bool HasFooter { get; set; }

		bool IsHeader(int index);
		bool IsFooter(int index);
	}
}