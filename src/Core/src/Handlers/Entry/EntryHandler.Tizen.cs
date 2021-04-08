using System;
using Tizen.UIExtensions.ElmSharp;

namespace Microsoft.Maui.Handlers
{
	public partial class EntryHandler : AbstractViewHandler<IEntry, Entry>
	{
		protected override Entry CreateNativeView()
		{
			if (NativeParent == null)
			{
				throw new InvalidOperationException($"{nameof(NativeParent)} cannot be null");
			}
			return new Entry(NativeParent);
		}

		public static void MapText(EntryHandler handler, IEntry entry)
		{
			handler.TypedNativeView?.UpdateText(entry);
		}

		public static void MapTextColor(EntryHandler handler, IEntry entry)
		{
			handler.TypedNativeView?.UpdateTextColor(entry);
		}

		public static void MapIsPassword(EntryHandler handler, IEntry entry)
		{
			handler.TypedNativeView?.UpdateIsPassword(entry);
		}
	}
}