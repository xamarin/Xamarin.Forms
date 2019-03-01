﻿using System.ComponentModel;
using UIKit;
using Xamarin.Forms.Platform.iOS;

namespace Xamarin.Forms.Material.iOS
{
	public class MaterialTimePickerRenderer : TimePickerRendererBase<MaterialTextField>, IMaterialEntryRenderer
	{
		internal void UpdatePlaceholder() => Control?.UpdatePlaceholder(this);

		protected internal override void UpdateFont()
		{
			base.UpdateFont();
			Control?.ApplyTypographyScheme(Element);
		}

		protected override MaterialTextField CreateNativeControl() => new NoCaretMaterialTextField(this, Element);
		protected override void SetBackgroundColor(Color color) => ApplyTheme();
		protected internal override void UpdateTextColor() => Control?.UpdateTextColor(this);
		protected virtual void ApplyTheme() => Control?.ApplyTheme(this);

		protected override void OnElementChanged(ElementChangedEventArgs<TimePicker> e)
		{
			base.OnElementChanged(e);
			UpdatePlaceholder();
		}

		string IMaterialEntryRenderer.Placeholder => string.Empty;
		Color IMaterialEntryRenderer.PlaceholderColor => Color.Default;
		Color IMaterialEntryRenderer.TextColor => Element?.TextColor ?? Color.Default;
		Color IMaterialEntryRenderer.BackgroundColor => Element?.BackgroundColor ?? Color.Default;
	}
}