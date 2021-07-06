using System;
using System.ComponentModel;
using Xamarin.Forms.Platform.GTK.Controls;
using Xamarin.Forms.Platform.GTK.Extensions;
using Xamarin.Forms.PlatformConfiguration.GTKSpecific;
using Xamarin.Forms.Shapes;

namespace Xamarin.Forms.Platform.GTK.Renderers
{
	public class PathRenderer : ShapeRenderer<Path, PathView>
	{
		protected override void OnElementChanged(ElementChangedEventArgs<Path> e)
		{
			base.OnElementChanged(e);

			if (e.NewElement != null)
			{
				SetData(Element.Data);
			}
		}

		protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			base.OnElementPropertyChanged(sender, e);

			if (e.PropertyName == Shapes.Path.DataProperty.PropertyName)
				SetData(Element.Data);
		}

		void SetData(Geometry data)
		{
			Control.UpdateGeometry(data as PathGeometry);
		}
	}
}
