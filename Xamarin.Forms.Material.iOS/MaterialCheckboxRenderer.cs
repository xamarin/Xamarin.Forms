using System;
using Xamarin.Forms.Platform.iOS;

namespace Xamarin.Forms.Material.iOS
{
	public class MaterialCheckBoxRenderer : CheckBoxRendererBase<MaterialFormsCheckBox>
	{
		protected override float MinimumSize => 48f;
		bool _isDisposed = false;

		protected override MaterialFormsCheckBox CreateNativeControl()
		{
			return new MaterialFormsCheckBox();
		}

		protected override void OnElementChanged(ElementChangedEventArgs<CheckBox> e)
		{
			base.OnElementChanged(e);

			if (Control == null)
				return;

			if (e.OldElement != null)
				Control.CheckedChanged -= OnCheckedChanged;

			if (e.NewElement != null)
				Control.CheckedChanged += OnCheckedChanged;
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing && !_isDisposed && Control != null)
			{
				Control.CheckedChanged -= OnCheckedChanged;
				_isDisposed = true;
			}

			base.Dispose(disposing);
		}

		void OnCheckedChanged(object sender, EventArgs e)
		{
			UpdateTintColor();
		}

		protected override void UpdateTintColor()
		{
			if (Element.Color != Color.Default)
			{
				base.UpdateTintColor();
				return;
			}

			Control.CheckBoxTintColor = MaterialColors.GetCheckBoxColor(Control.IsChecked, Element.IsEnabled).ToColor();
		}
	}
}
