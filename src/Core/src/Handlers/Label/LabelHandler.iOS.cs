using UIKit;

namespace Microsoft.Maui.Handlers
{
	public partial class LabelHandler : AbstractViewHandler<ILabel, UILabel>
	{
		protected override UILabel CreateNativeView() => new UILabel();

		public static void MapText(LabelHandler handler, ILabel label)
		{
			handler.TypedNativeView?.UpdateText(label);
		}

		public static void MapTextColor(LabelHandler handler, ILabel label)
		{
			handler.TypedNativeView?.UpdateTextColor(label);
		}

		public static void MapLineBreakMode(LabelHandler handler, ILabel label)
		{
			ViewHandler.CheckParameters(handler, label);

			handler.TypedNativeView?.UpdateLineBreakMode(label);
		}

		public static void MapMaxLines(LabelHandler handler, ILabel label)
		{
			ViewHandler.CheckParameters(handler, label);

			handler.TypedNativeView?.UpdateMaxLines(label);
		}
	}
}