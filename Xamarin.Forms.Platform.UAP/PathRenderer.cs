using System.ComponentModel;
using WPath = Windows.UI.Xaml.Shapes.Path;

namespace Xamarin.Forms.Platform.UWP
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
			}
		}

		protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs args)
		{
			base.OnElementPropertyChanged(sender, args);

			if (args.PropertyName == Path.DataProperty.PropertyName)
				UpdateData();
		}

		void UpdateData()
		{
			Control.Data = Element.Data.ToWindows();
		}
	}
}