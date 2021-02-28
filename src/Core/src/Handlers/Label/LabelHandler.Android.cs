using System;
using Android.Widget;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Maui;

namespace Microsoft.Maui.Handlers
{
	public partial class LabelHandler : AbstractViewHandler<ILabel, TextView>
	{
		static Color DefaultTextColor { get; set; }

		protected override TextView CreateNativeView() => new TextView(Context);

		protected override void SetupDefaults(TextView nativeView)
		{
			if (nativeView.TextColors == null)
			{
				DefaultTextColor = Color.Default;
			}
			else
			{
				DefaultTextColor = Color.FromUint((uint)nativeView.TextColors.DefaultColor);
			}
		}

		public static void MapText(LabelHandler handler, ILabel label)
		{
			handler.TypedNativeView?.UpdateText(label);
		}

		public static void MapTextColor(LabelHandler handler, ILabel label)
		{
			handler.TypedNativeView?.UpdateTextColor(label, DefaultTextColor);
		}

		public static void MapFontFamily(LabelHandler handler, ILabel label)
		{
			var context = MauiApp.Current?.Context ?? throw new InvalidOperationException($"The MauiApp.Current.Context can't be null.");
			var fontManager = context.Services.GetRequiredService<IFontManager>();

			handler.TypedNativeView?.UpdateFont(label, fontManager);
		}
	}
}