using CoreGraphics;
using UIKit;
using Xamarin.Forms;

namespace Xamarin.Platform.Handlers
{
	public partial class EditorHandler : AbstractViewHandler<IEditor, UITextView>
	{
		static readonly int BaseHeight = 30;

		static UIColor? DefaultTextColor;

		protected override UITextView CreateNativeView()
		{
			return new UITextView(CGRect.Empty);
		}

		protected override void SetupDefaults(UITextView nativeView)
		{
			DefaultTextColor = nativeView.TextColor;
		}

		public override Size GetDesiredSize(double widthConstraint, double heightConstraint) =>
			new SizeRequest(new Size(widthConstraint, BaseHeight));

		public static void MapText(EditorHandler handler, IEditor editor)
		{
			ViewHandler.CheckParameters(handler, editor);

			handler.TypedNativeView?.UpdateText(editor);
		}

		public static void MapTextColor(EditorHandler handler, IEditor editor)
		{
			ViewHandler.CheckParameters(handler, editor);

			handler.TypedNativeView?.UpdateTextColor(editor, DefaultTextColor);
		}
	}
}