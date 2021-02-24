using UIKit;

namespace Microsoft.Maui.Handlers
{
	public partial class LabelHandler : AbstractViewHandler<ILabel, UILabel>
	{
		protected override UILabel CreateNativeView() => new UILabel();

		public static void MapText(LabelHandler handler, ILabel label)
		{
			ViewHandler.CheckParameters(handler, label);

			handler.TypedNativeView?.UpdateText(label);
		}

		public static void MapTextColor(LabelHandler handler, ILabel label)
		{
			ViewHandler.CheckParameters(handler, label);

			handler.TypedNativeView?.UpdateTextColor(label);
		}

		public static void MapHorizontalTextAlignment(LabelHandler handler, ILabel label)
		{
			ViewHandler.CheckParameters(handler, label);

			handler.TypedNativeView?.UpdateHorizontalTextAlignment(label);
		}

		public static void MapVerticalTextAlignment(LabelHandler handler, ILabel label)
		{
			ViewHandler.CheckParameters(handler, label);

			handler.TypedNativeView?.UpdateVerticalTextAlignment(label);
		}
	}
}