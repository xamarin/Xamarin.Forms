using CoreGraphics;
using UIKit;
using MaterialComponents;

namespace Xamarin.Forms.Platform.iOS.Material
{
	public class MaterialEditorRenderer : EditorRendererBase<MaterialMultilineTextField>, IMaterialEntryRenderer
	{
		public MaterialEditorRenderer()
		{
			VisualElement.VerifyVisualFlagEnabled();
			ClipsToBounds = true;
		}

		protected override MaterialMultilineTextField CreateNativeControl()
		{			
			return new MaterialMultilineTextField(this, Element);
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

		protected internal override void UpdatePlaceholderText()
		{
			Control?.UpdatePlaceholder(this);
		}
		protected internal override void UpdatePlaceholderColor()
		{
			Control?.UpdatePlaceholder(this);
		}

		Color IMaterialEntryRenderer.TextColor => Element?.TextColor ?? Color.Default;
		Color IMaterialEntryRenderer.PlaceholderColor => Element?.PlaceholderColor ?? Color.Default;
		Color IMaterialEntryRenderer.BackgroundColor => Element?.BackgroundColor ?? Color.Default;
		string IMaterialEntryRenderer.Placeholder => Element?.Placeholder ?? string.Empty;

		protected IntrinsicHeightTextView IntrinsicHeightTextView => (IntrinsicHeightTextView)TextView;
		protected override UITextView TextView => Control?.TextView;
	}
}
