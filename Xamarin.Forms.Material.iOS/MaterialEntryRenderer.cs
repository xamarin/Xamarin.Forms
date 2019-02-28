﻿using CoreGraphics;
using UIKit;
using Xamarin.Forms.Platform.iOS;

namespace Xamarin.Forms.Material.iOS
{
	public class MaterialEntryRenderer : EntryRendererBase<MaterialTextField>, IMaterialEntryRenderer
	{
		protected override MaterialTextField CreateNativeControl() => new MaterialTextField(this, Element);
		protected override void SetBackgroundColor(Color color) => ApplyTheme();
		protected internal override void UpdateColor() => Control?.UpdateTextColor(this);
		protected virtual void ApplyTheme() => Control?.ApplyTheme(this);
		protected internal override void UpdatePlaceholder() => Control?.UpdatePlaceholder(this);

		protected internal override void UpdateFont()
		{
			base.UpdateFont();
			Control?.ApplyTypographyScheme(Element);
		}

		Color IMaterialEntryRenderer.TextColor => Element?.TextColor ?? Color.Default;
		Color IMaterialEntryRenderer.PlaceholderColor => Element?.PlaceholderColor ?? Color.Default;
		Color IMaterialEntryRenderer.BackgroundColor => Element?.BackgroundColor ?? Color.Default;
		string IMaterialEntryRenderer.Placeholder => Element?.Placeholder ?? string.Empty;
	}
}