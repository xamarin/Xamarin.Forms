using System.ComponentModel;
using Xamarin.Forms.Platform.WPF.Extensions;
using WPath = System.Windows.Shapes.Path;

namespace Xamarin.Forms.Platform.WPF
{
	public class PathRenderer : ShapeRenderer<Path, WPath>
	{
		protected override void OnElementChanged(ElementChangedEventArgs<Path> args)
		{
			if (Control == null && args.NewElement != null)
			{
				SetNativeControl(new WPath());
			}

			base.OnElementChanged(args);

			if (args.NewElement != null)
			{
				UpdateData();
				UpdateRenderTransform();
			}
		}

		protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs args)
		{
			base.OnElementPropertyChanged(sender, args);

			if (args.PropertyName == Path.DataProperty.PropertyName)
				UpdateData();
			else if (args.PropertyName == Path.RenderTransformProperty.PropertyName)
				UpdateRenderTransform();
		}

		void UpdateData()
		{
			Control.Data = Element.Data.ToWindows();
		}

		void UpdateRenderTransform()
		{
			if (Element.RenderTransform != null)
				Control.RenderTransform = Element.RenderTransform.ToWindows();
		}
	}
}