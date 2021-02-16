using System;

namespace Xamarin.Platform.Handlers
{
	public partial class ButtonHandler : AbstractViewHandler<IButton, object>
	{
		protected override object CreateNativeView() => throw new NotImplementedException();

		public static void MapColor(ButtonHandler handler, IButton button) { }
		public static void MapText(ButtonHandler handler, IButton button) { }
		public static void MapFont(ButtonHandler handler, IButton button) { }
		public static void MapCharacterSpacing(ButtonHandler handler, IButton button) { }
		public static void MapCornerRadius(ButtonHandler handler, IButton button) { }
		public static void MapBorderColor(ButtonHandler handler, IButton button) { }
		public static void MapBorderWidth(ButtonHandler handler, IButton button) { }
		public static void MapContentLayout(ButtonHandler handler, IButton button) { }
		public static void MapPadding(ButtonHandler handler, IButton button) { }
	}
}