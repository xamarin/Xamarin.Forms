using System;
using System.ComponentModel;
using Xamarin.Forms.Platform.GTK.Extensions;

namespace Xamarin.Forms.Platform.GTK.Renderers
{
	public class RadioButtonRenderer : ViewRenderer<RadioButton, Gtk.RadioButton>
	{
		#region VisualElementRenderer overrides

		protected override void OnElementChanged(ElementChangedEventArgs<RadioButton> e)
		{
			base.OnElementChanged(e);

			if (e.NewElement != null)
			{
				if (Control == null)
				{
					var button = new Gtk.RadioButton("label");
					button.Activated += Button_Activated;
				}

				//UpdateContent();
				//UpdateFont();
			}
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

		#endregion VisualElementRenderer overrides

		#region Private methods

		void UpdateContent() { }

		void UpdateTextColor() { }

		void UpdateFont() { }

		void UpdateBorderColor() { }

		void UpdateBorderWidth() { }

		void UpdateBorderRadius() { }

		void UpdatePadding() { }

		void UpdateCheck() { }

		#endregion Private methods

		#region Handlers

		private void Button_Activated(object sender, EventArgs e)
		{
			throw new NotImplementedException();
		}

		#endregion Handlers
	}
}