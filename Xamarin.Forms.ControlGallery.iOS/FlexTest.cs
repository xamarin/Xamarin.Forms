using System;
using System.Runtime.InteropServices;
using static Xamarin.Forms.ControlGallery.iOS.NativeFunctions;

namespace Xamarin.Forms.ControlGallery.iOS
{
	public static class FlexTest
	{
		public static Item CreateFlexItem()
		{
			return new Item();
		}
	}

	[StructLayout(LayoutKind.Sequential)]
	public partial class Item
	{
		private IntPtr item = IntPtr.Zero;

		public Item()
		{
			item = flex_item_new();
		}
	}

	internal class NativeFunctions
	{
		const string dll_name = "__Internal"; //flex

		[DllImport(dll_name)] public static extern IntPtr flex_item_new();
	}


}
