using System.ComponentModel;
using EColor = ElmSharp.Color;
using ERectangle = ElmSharp.Rectangle;

namespace Xamarin.Forms.Platform.Tizen
{
	public class BoxViewRenderer : VisualElementRenderer<BoxView>
	{
		ERectangle _control;

		public BoxViewRenderer()
		{
		}

		protected override void OnElementChanged(ElementChangedEventArgs<BoxView> e)
		{
			if (_control == null)
			{
				_control = new ERectangle(Forms.Context.MainWindow);
				SetNativeControl(_control);
			}

			if (e.NewElement != null)
			{
				UpdateColor();
			}

			base.OnElementChanged(e);
		}

		protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			if (e.PropertyName == BoxView.ColorProperty.PropertyName)
			{
				UpdateColor();
			}
			base.OnElementPropertyChanged(sender, e);
		}

		protected override void UpdateBackgroundColor()
		{
			UpdateColor();
		}

		protected override void UpdateOpacity()
		{
			UpdateColor();
		}

		void UpdateColor()
		{
			if (Element.Color.IsDefault)
			{
				if (Element.BackgroundColor.IsDefault)
				{
					// Set to default color. (Transparent)
					_control.Color = EColor.Transparent;
				}
				else
				{
					// Use BackgroundColor only if color is default and background color is not default.
					_control.Color = Element.BackgroundColor.MultiplyAlpha(Element.Opacity).ToNative();
				}
			}
			else
			{
				// Color has higer priority than BackgroundColor.
				_control.Color = Element.Color.MultiplyAlpha(Element.Opacity).ToNative();
			}
		}
	}
}
