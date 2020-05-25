using System.ComponentModel;
using Xamarin.Forms.Platform.WPF.Extensions;
using WFillRule = System.Windows.Media.FillRule;
using WPolyline = System.Windows.Shapes.Polyline;

namespace Xamarin.Forms.Platform.WPF.Renderers
{
	public class PolylineRenderer : ShapeRenderer<Polyline, WPolyline>
	{
		protected override void OnElementChanged(ElementChangedEventArgs<Polyline> args)
		{
			if (Control == null && args.NewElement != null)
			{
				SetNativeControl(new WPolyline());
			}

			base.OnElementChanged(args);

			if (args.NewElement != null)
			{
				UpdatePoints();
				UpdateFillRule();
			}
		}

		protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs args)
		{
			base.OnElementPropertyChanged(sender, args);

			if (args.PropertyName == Polyline.PointsProperty.PropertyName)
				UpdatePoints();
			else if (args.PropertyName == Polyline.FillRuleProperty.PropertyName)
				UpdateFillRule();
		}

		void UpdatePoints()
		{
			Control.Points = Element.Points.ToWindows();
		}

		void UpdateFillRule()
		{
			Control.FillRule = Element.FillRule == FillRule.EvenOdd ?
				WFillRule.EvenOdd :
				WFillRule.Nonzero;
		}
	}
}