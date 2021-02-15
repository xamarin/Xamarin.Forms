using System;

namespace Xamarin.Platform.Handlers
{
	public partial class LabelHandler : AbstractViewHandler<ILabel, object>
	{
		protected override object CreateNativeView() => throw new NotImplementedException();

		public static void MapText(IViewHandler handler, ILabel label) { }
		public static void MapTextColor(IViewHandler handler, ILabel label) { }
		public static void MapFont(IViewHandler handler, ILabel label) { }
		public static void MapTextTransform(IViewHandler handler, ILabel label) { }
		public static void MapCharacterSpacing(IViewHandler handler, ILabel label) { }
		public static void MapLineHeight(IViewHandler handler, ILabel label) { }
		public static void MapFontSize(IViewHandler handler, ILabel label) { }
		public static void MapFontAttributes(IViewHandler handler, ILabel label) { }
		public static void MapHorizontalTextAlignment(IViewHandler handler, ILabel label) { }
		public static void MapVerticalTextAlignment(IViewHandler handler, ILabel label) { }
		public static void MapTextDecorations(IViewHandler handler, ILabel label) { }
		public static void MapLineBreakMode(IViewHandler handler, ILabel label) { }
		public static void MapMaxLines(IViewHandler handler, ILabel label) { }
		public static void MapPadding(IViewHandler handler, ILabel label) { }
	}
}