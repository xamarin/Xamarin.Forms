using System;

namespace Xamarin.Platform.Handlers
{
	public partial class EditorHandler : AbstractViewHandler<IEditor, object>
	{
		protected override object CreateNativeView() => throw new NotImplementedException();

		public static void MapText(IViewHandler handler, IEditor editor) { }
		public static void MapTextColor(IViewHandler handler, IEditor editor) { }
	}
}