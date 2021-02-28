using System;
using Microsoft.Extensions.DependencyInjection;
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

		public static void MapFontFamily(LabelHandler handler, ILabel label)
		{
			var context = MauiApp.Current?.Context ?? throw new InvalidOperationException($"The MauiApp.Current.Context can't be null.");
			var fontManager = context.Services.GetRequiredService<IFontManager>();

			handler.TypedNativeView?.UpdateFont(label, fontManager);
		}
	}
}