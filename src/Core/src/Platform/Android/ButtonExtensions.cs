using Android.Content.Res;
using Android.Util;
using AndroidX.AppCompat.Widget;
using AView = Android.Views.View;
using XColor = Microsoft.Maui.Color;

namespace Microsoft.Maui
{
	public static class ButtonExtensions
	{
		static bool ColorButtonNormalSet;
		static XColor ColorButtonNormal = XColor.Default;

		public static void UpdateText(this AppCompatButton appCompatButton, IButton button) =>
			appCompatButton.Text = button.Text;

		public static void UpdateTextColor(this AppCompatButton appCompatButton, IButton button) =>
			appCompatButton.UpdateTextColor(button.TextColor, appCompatButton.TextColors);

		public static void UpdateTextColor(this AppCompatButton button, XColor color, ColorStateList? defaultColor)
		{
			if (color.IsDefault)
				button.SetTextColor(defaultColor);
			else
				button.SetTextColor(color.ToNative());
		}

		public static void UpdateTextColor(this AppCompatButton appCompatButton, IButton button, XColor defaultColor) =>
			appCompatButton.SetTextColor(button.TextColor.Cleanse(defaultColor).ToNative());

		public static void UpdateCornerRadius(this AppCompatButton appCompatButton, IButton button)
		{
			appCompatButton.UpdateCornerRadius(button, null);
		}

		public static void UpdateCornerRadius(this AppCompatButton appCompatButton, IButton button, ButtonBorderBackgroundManager? borderBackgroundManager)
		{
			appCompatButton.UpdateDrawable(button, borderBackgroundManager);
		}

		public static XColor GetColorButtonNormal(this AView appCompatButton)
		{
			if (!ColorButtonNormalSet)
			{
				ColorButtonNormal = appCompatButton.GetButtonColor();
				ColorButtonNormalSet = true;
			}

			return ColorButtonNormal;
		}

		static XColor Cleanse(this XColor color, XColor defaultColor) => color.IsDefault ? defaultColor : color;

		static void UpdateDrawable(this AppCompatButton appCompatButton, IButton button, ButtonBorderBackgroundManager? borderBackgroundManager)
		{
			if (borderBackgroundManager == null)
				borderBackgroundManager = new ButtonBorderBackgroundManager(appCompatButton, button);

			borderBackgroundManager.UpdateDrawable();
		}

		static XColor GetButtonColor(this AView appCompatButton)
		{
			XColor rc = XColor.Default;

			if (appCompatButton == null || appCompatButton.Context == null)
				return rc;

			using (var value = new TypedValue())
			{
				if (appCompatButton.Context.Theme != null)
				{
					int? colorButtonNormal = appCompatButton.Context.Resources?.GetIdentifier("colorButtonNormal", "attr", appCompatButton.Context.PackageName);

					if (appCompatButton.Context.Theme.ResolveAttribute(global::Android.Resource.Attribute.ColorButtonNormal, value, true) && NativeVersion.IsAtLeast(21)) // Android 5.0+
					{
						rc = XColor.FromUint((uint)value.Data);
					}
					else if (colorButtonNormal.HasValue && appCompatButton.Context.Theme.ResolveAttribute(colorButtonNormal.Value, value, true))  // < Android 5.0
					{
						rc = XColor.FromUint((uint)value.Data);
					}
				}
			}

			return rc;
		}
	}
}
