//using Android.Widget;

//namespace Xamarin.Platform.Handlers
//{
//	public partial class EditorHandler : AbstractViewHandler<IEditor, EditText>
//	{
//		static TextColorSwitcher? TextColorSwitcher { get; set; }

//		protected override EditText CreateNativeView()
//		{
//			return new EditText(Context);
//		}

//		protected override void SetupDefaults(EditText nativeView)
//		{
//			base.SetupDefaults(nativeView);

//			TextColorSwitcher = new TextColorSwitcher(nativeView.TextColors);
//		}

//		public static void MapText(EditorHandler handler, IEditor editor)
//		{
//			ViewHandler.CheckParameters(handler, editor);

//			handler.TypedNativeView?.UpdateText(editor);
//		}

//		public static void MapTextColor(EditorHandler handler, IEditor editor)
//		{
//			ViewHandler.CheckParameters(handler, editor);

//			handler.TypedNativeView?.UpdateTextColor(editor, TextColorSwitcher);
//		}
//	}
//}

using Android.Content.Res;
using Android.Widget;

namespace Xamarin.Platform.Handlers
{
	public partial class EditorHandler : AbstractViewHandler<IEditor, EditText>
	{
		static ColorStateList? DefaultTextColors { get; set; }

		protected override EditText CreateNativeView()
		{
			return new EditText(Context);
		}

		protected override void SetupDefaults(EditText nativeView)
		{
			base.SetupDefaults(nativeView);
			DefaultTextColors = nativeView.TextColors;
		}

		public static void MapText(EditorHandler handler, IEditor editor)
		{
			ViewHandler.CheckParameters(handler, editor);

			handler.TypedNativeView?.UpdateText(editor);
		}

		public static void MapTextColor(EditorHandler handler, IEditor editor)
		{
			ViewHandler.CheckParameters(handler, editor);

			handler.TypedNativeView?.UpdateTextColor(editor, DefaultTextColors);
		}
	}
}