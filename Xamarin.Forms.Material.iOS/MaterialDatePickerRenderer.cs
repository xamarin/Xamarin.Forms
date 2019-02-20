using System.ComponentModel;
using UIKit;

namespace Xamarin.Forms.Platform.iOS.Material
{
	public class MaterialDatePickerRenderer : DatePickerRendererBase<MaterialTextField>, IMaterialEntryRenderer
	{
		public MaterialDatePickerRenderer()
		{
			VisualElement.VerifyVisualFlagEnabled();
		}

		protected override MaterialTextField CreateNativeControl()
		{
			var field = new NoCaretMaterialTextField(this, Element);
			return field;
		}

		protected override void SetBackgroundColor(Color color)
		{
			ApplyTheme();
		}

		protected internal override void UpdateFont()
		{
			base.UpdateFont();
			Control?.ApplyTypographyScheme(Element);
		}
		

		protected internal override void UpdateTextColor()
		{
			Control?.UpdateTextColor(this);
		}


		protected virtual void ApplyTheme()
		{
			Control?.ApplyTheme(this);
		}

		internal void UpdatePlaceholder()
		{
			Control?.UpdatePlaceholder(this);
		}

		protected override void OnElementChanged(ElementChangedEventArgs<DatePicker> e)
		{
			base.OnElementChanged(e);
			UpdatePlaceholder();
		}

		protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			base.OnElementPropertyChanged(sender, e);
			if (e.PropertyName == Xamarin.Forms.Material.DatePicker.PlaceholderProperty.PropertyName)
				UpdatePlaceholder();
			else if (e.PropertyName == Xamarin.Forms.Material.DatePicker.PlaceholderColorProperty.PropertyName)
				UpdatePlaceholder();

		}

		string IMaterialEntryRenderer.Placeholder => Xamarin.Forms.Material.DatePicker.GetPlaceholder(Element);
		Color IMaterialEntryRenderer.PlaceholderColor => Xamarin.Forms.Material.DatePicker.GetPlaceholderColor(Element);

		Color IMaterialEntryRenderer.TextColor => Element?.TextColor ?? Color.Default;
		Color IMaterialEntryRenderer.BackgroundColor => Element?.BackgroundColor ?? Color.Default;
	}
}