using System;
using System.ComponentModel;
using System.Drawing;
using CoreGraphics;
using Foundation;
using UIKit;

namespace Xamarin.Forms.Platform.iOS
{
	public class IndicatorsViewRenderer : ViewRenderer<IndicatorsView, UIPageControl>
	{
		public override SizeRequest GetDesiredSize(double widthConstraint, double heightConstraint)
		{
			return new SizeRequest(new Size(widthConstraint, heightConstraint));
		}

		protected override void OnElementChanged(ElementChangedEventArgs<IndicatorsView> e)
		{
			base.OnElementChanged(e);
			SetUpNewElement(e.NewElement);
		}

		protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs changedProperty)
		{
			if (changedProperty.Is(IndicatorsView.IndicatorsShapeProperty))
			{
				UpdateShapes();
			}
			else if (changedProperty.Is(IndicatorsView.IndicatorsTintColorProperty))
			{
				UpdatePageIndicatorTintColor();
			}
			else if (changedProperty.Is(IndicatorsView.SelectedIndicatorTintColorProperty))
			{
				UpdateCurrentPageIndicatorTintColor();
			}
			else if (changedProperty.IsOneOf(IndicatorsView.ItemsSourceByProperty, IndicatorsView.PositionProperty, IndicatorsView.CountProperty))
			{
				UpdateIndicators();
			}
		}

		protected virtual void SetUpNewElement(IndicatorsView newElement)
		{
			if (newElement == null)
			{
				return;
			}

			SetNativeControl(new UIPageControl());

			UpdatePageIndicatorTintColor();
			UpdateCurrentPageIndicatorTintColor();
			UpdateIndicators();

		}

		void UpdatePageIndicatorTintColor()
		{
			if (Control == null)
				return;

			Control.PageIndicatorTintColor = Element.IndicatorsTintColor.ToUIColor();
			UpdateShapes();
		}

		void UpdateCurrentPageIndicatorTintColor()
		{
			if (Control == null)
				return;

			Control.CurrentPageIndicatorTintColor = Element.SelectedIndicatorTintColor.ToUIColor();
			UpdateShapes();
		}

		void UpdateIndicators()
		{
			if (Control == null)
				return;

			Control.Pages = Element.Count;
			Control.CurrentPage = Element.Position;
			UpdateShapes();
		}

		void UpdateShapes()
		{
			if (Control == null)
				return;

			if (Element.IndicatorsShape == IndicatorsShape.Square)
			{
				foreach (var view in Control.Subviews)
				{
					if (view.Frame.Width == 7)
					{
						view.Layer.CornerRadius = 0;
						var frame = new CGRect(view.Frame.X, view.Frame.Y, view.Frame.Width - 1, view.Frame.Height - 1);
						view.Frame = frame;
					}
				}
			}
			else
			{
				foreach (var view in Control.Subviews)
				{
					if (view.Frame.Width == 6)
					{
						view.Layer.CornerRadius = 3.5f;
						var frame = new CGRect(view.Frame.X, view.Frame.Y, view.Frame.Width + 1, view.Frame.Height + 1);
						view.Frame = frame;
					}
				}
			}
		}
	}
}
