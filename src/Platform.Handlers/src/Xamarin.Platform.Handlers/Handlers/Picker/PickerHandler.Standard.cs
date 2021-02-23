using System;

namespace Xamarin.Platform.Handlers
{
	public partial class PickerHandler : AbstractViewHandler<IPicker, object>
	{
		protected override object CreateNativeView() => throw new NotImplementedException();

		public static void MapTitle(PickerHandler handler, IPicker view) { }
		public static void MapTitleColor(PickerHandler handler, IPicker view) { }
		public static void MapTextColor(PickerHandler handler, IPicker view) { }
		public static void MapSelectedIndex(PickerHandler handler, IPicker view) { }
	}
}