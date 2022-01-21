using System;
using System.ComponentModel;
using Xamarin.Forms.Platform.GTK.Extensions;

namespace Xamarin.Forms.Platform.GTK.Renderers
{
	public class RadioButtonRenderer : ViewRenderer<RadioButton, Controls.RadioButton>
	{
		Gtk.RadioButton _ghost;

		#region VisualElementRenderer overrides

		protected override void Dispose(bool disposing)
		{
			var formsButton = Control;
			if (formsButton != null)
			{
				formsButton.Clicked -= Button_Clicked;
			}

			base.Dispose(disposing);
		}

		protected override void OnElementChanged(ElementChangedEventArgs<RadioButton> e)
		{
			if (e.NewElement != null)
			{
				if (Control == null)
				{
					var button = new Controls.RadioButton();
					button.Clicked += Button_Clicked;

					_ghost = new Gtk.RadioButton(button);
					_ghost.Active = true;

					SetNativeControl(button);
				}

				UpdateContent();
			}

			base.OnElementChanged(e);
		}

		protected override bool PreventGestureBubbling { get; set; } = true;

		public override SizeRequest GetDesiredSize(double widthConstraint, double heightConstraint)
		{
			var req = Control.SizeRequest();

			var widthFits = widthConstraint >= req.Width;
			var heightFits = heightConstraint >= req.Height;

			var size = new Size(widthFits ? req.Width : (int)widthConstraint, heightFits ? req.Height : (int)heightConstraint);

			return new SizeRequest(size);
		}

		protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			base.OnElementPropertyChanged(sender, e);

			if (e.PropertyName == RadioButton.ContentProperty.PropertyName)
			{
				UpdateContent();
			}
			else if (e.PropertyName == RadioButton.TextColorProperty.PropertyName)
			{
				UpdateTextColor();
			}
			else if (e.PropertyName == RadioButton.FontFamilyProperty.PropertyName ||
				e.PropertyName == RadioButton.FontSizeProperty.PropertyName ||
				e.PropertyName == RadioButton.FontAttributesProperty.PropertyName)
			{
				UpdateFont();
			}
			else if (e.PropertyName == RadioButton.BorderColorProperty.PropertyName)
			{
				UpdateBorderColor();
			}
			else if (e.PropertyName == RadioButton.BorderWidthProperty.PropertyName)
			{
				UpdateBorderWidth();
			}
			else if (e.PropertyName == RadioButton.CornerRadiusProperty.PropertyName)
			{
				UpdateBorderRadius();
			}
			else if (e.PropertyName == RadioButton.PaddingProperty.PropertyName)
			{
				UpdatePadding();
			}
			else if (e.PropertyName == RadioButton.IsCheckedProperty.PropertyName)
			{
				UpdateCheck();
			}
		}

		protected override void UpdateBackgroundColor()
		{
			if (Element == null)
				return;

			if (Element.BackgroundColor.IsDefault)
			{
				Control.ResetBackgroundColor();
			}
			else if (Element.BackgroundColor != Color.Transparent)
			{
				Control.SetBackgroundColor(Element.BackgroundColor.ToGtkColor());
			}
			else
			{
				Control.SetBackgroundColor(null);
			}
		}

		#endregion VisualElementRenderer overrides

		#region Private methods

		void UpdateContent()
		{
			var content = Element?.Content;

			Control.Label = content?.ToString();
		}

		void UpdateTextColor() { }

		void UpdateFont() { }

		void UpdateBorderColor() { }

		void UpdateBorderWidth() { }

		void UpdateBorderRadius() { }

		void UpdatePadding() { }

		void UpdateCheck()
		{
			if (Element.IsChecked)
			{
				Control.Active = true;
				_ghost.Active = false;
			}
			else
			{
				_ghost.Active = true;
				Control.Active = false;
			}
		}

		#endregion Private methods

		#region Handlers

		private void Button_Clicked(object sender, EventArgs e)
		{
			if (Element == null || sender == null)
			{
				return;
			}

			Element.IsChecked = (sender as Controls.RadioButton).Active;
		}

		#endregion Handlers
	}
}