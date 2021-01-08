using UIKit;
using Xamarin.Forms.Platform.iOS;

namespace Xamarin.Forms.Material.iOS
{
	public class MaterialTimePickerRenderer : TimePickerRendererBase<UIControl>, IMaterialEntryRenderer
	{
		MaterialTextField MaterialTextField => Control as MaterialTextField;
		internal void UpdatePlaceholder() => MaterialTextField?.UpdatePlaceholder(this);

		protected internal override void UpdateFont()
		{
			base.UpdateFont();
			MaterialTextField?.ApplyTypographyScheme(Element);
		}

		protected override UIControl CreateNativeControl() => new NoCaretMaterialTextField(this, Element);
		protected override void SetBackgroundColor(Color color) => ApplyTheme();
		protected override void SetBackground(Brush brush) => ApplyTheme();
		protected internal override void UpdateTextColor() => MaterialTextField?.UpdateTextColor(this);
		protected virtual void ApplyTheme() => MaterialTextField?.ApplyTheme(this);
		protected virtual void ApplyThemeIfNeeded() => MaterialTextField?.ApplyThemeIfNeeded(this);

		protected override void OnElementChanged(ElementChangedEventArgs<TimePicker> e)
		{
			base.OnElementChanged(e);

			if (e.NewElement != null)
				UpdatePlaceholder();
		}

		public override void LayoutSubviews()
		{
			base.LayoutSubviews();

			ApplyThemeIfNeeded();
		}

		string IMaterialEntryRenderer.Placeholder => string.Empty;
		Color IMaterialEntryRenderer.PlaceholderColor => Color.Default;
		Color IMaterialEntryRenderer.TextColor => Element?.TextColor ?? Color.Default;
		Color IMaterialEntryRenderer.BackgroundColor => Element?.BackgroundColor ?? Color.Default;
		Brush IMaterialEntryRenderer.Background => Element?.Background ?? Brush.Default;
		
	}
}