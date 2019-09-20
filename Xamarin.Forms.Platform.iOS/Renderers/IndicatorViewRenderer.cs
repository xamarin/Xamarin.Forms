using System.ComponentModel;
using UIKit;
using static Xamarin.Forms.IndicatorView;

namespace Xamarin.Forms.Platform.iOS
{
	public class IndicatorViewRenderer : ViewRenderer<IndicatorView, UIPageControl>
	{
		UIColor _defaultPagesIndicatorTintColor;
		UIColor _defaultCurrentPagesIndicatorTintColor;

		protected override void OnElementChanged(ElementChangedEventArgs<IndicatorView> e)
		{
			if (e.NewElement != null)
			{
				if (Control == null)
				{
					SetNativeControl(CreateNativeControl());
				}
				UpdatePagesIndicatorTintColor();
				UpdateCurrentPagesIndicatorTintColor();
				UpdatePages();
				UpdateHidesForSinglePage();
				UpdateCurrentPage();
			}
			base.OnElementChanged(e);
		}

		protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			base.OnElementPropertyChanged(sender, e);
			if (e.PropertyName == IndicatorColorProperty.PropertyName)
				UpdatePagesIndicatorTintColor();
			else if (e.PropertyName == SelectedIndicatorColorProperty.PropertyName)
				UpdateCurrentPagesIndicatorTintColor();
			else if (e.PropertyName == CountProperty.PropertyName)
				UpdatePages();
			else if (e.PropertyName == HidesForSingleIndicatorProperty.PropertyName)
				UpdateHidesForSinglePage();
			else if (e.PropertyName == PositionProperty.PropertyName)
				UpdateCurrentPage();
		}

		protected override UIPageControl CreateNativeControl()
			=> new UIPageControl();

		void UpdateCurrentPage()
			=> Control.CurrentPage = Element.Position;

		void UpdatePages()
			=> Control.Pages = Element.Count;

		void UpdateHidesForSinglePage()
			=> Control.HidesForSinglePage = Element.HidesForSingleIndicator;

		void UpdatePagesIndicatorTintColor()
		{
			_defaultPagesIndicatorTintColor = _defaultPagesIndicatorTintColor ?? Control.PageIndicatorTintColor;
			var color = Element.IndicatorColor;
			Control.PageIndicatorTintColor = color.IsDefault ? _defaultPagesIndicatorTintColor : color.ToUIColor();
		}

		void UpdateCurrentPagesIndicatorTintColor()
		{
			_defaultCurrentPagesIndicatorTintColor = _defaultCurrentPagesIndicatorTintColor ?? Control.CurrentPageIndicatorTintColor;
			var color = Element.IndicatorColor;
			Control.CurrentPageIndicatorTintColor = color.IsDefault ? _defaultCurrentPagesIndicatorTintColor : color.ToUIColor();
		}
	}
}