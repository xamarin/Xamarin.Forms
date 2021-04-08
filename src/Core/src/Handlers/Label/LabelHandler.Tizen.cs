using System;
using Microsoft.Extensions.DependencyInjection;
using Tizen.UIExtensions.ElmSharp;

namespace Microsoft.Maui.Handlers
{
	public partial class LabelHandler : AbstractViewHandler<ILabel, Label>
	{
#pragma warning disable CS8604 // Possible null reference argument.
		protected override Label CreateNativeView() => new Label(NativeParent);
#pragma warning restore CS8604 // Possible null reference argument.

		public static void MapText(LabelHandler handler, ILabel label)
		{
			handler.TypedNativeView?.UpdateText(label);
		}

		public static void MapTextColor(LabelHandler handler, ILabel label)
		{
			handler.TypedNativeView?.UpdateTextColor(label);
		}

		public static void MapFontFamily(LabelHandler handler, ILabel label)
		{
			MapFont(handler, label);
		}

		public static void MapFontSize(LabelHandler handler, ILabel label)
		{
			MapFont(handler, label);
		}

		public static void MapFontAttributes(LabelHandler handler, ILabel label)
		{
			MapFont(handler, label);
		}

		static void MapFont(LabelHandler handler, ILabel label)
		{
			var services = App.Current?.Services ?? throw new InvalidOperationException($"Unable to find service provider, the App.Current.Services was null.");
			var fontManager = services.GetRequiredService<IFontManager>();

			handler.TypedNativeView?.UpdateFont(label, fontManager);
		}
	}
}