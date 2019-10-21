using System.ComponentModel;
using UIKit;
using static Xamarin.Forms.IndicatorView;

namespace Xamarin.Forms.Platform.iOS
{
	public class IndicatorViewRenderer : ViewRenderer<IndicatorView, UIView>
	{
		UIColor _defaultPagesIndicatorTintColor;
		UIColor _defaultCurrentPagesIndicatorTintColor;
		UIPageControl UIPager => Control as UIPageControl;
		protected override void OnElementChanged(ElementChangedEventArgs<IndicatorView> e)
		{
			if (e.NewElement != null)
			{
				if (Control == null)
				{
					UpdateControl();
					if(UIPager != null)
					{
						_defaultPagesIndicatorTintColor = UIPager.PageIndicatorTintColor;
						_defaultCurrentPagesIndicatorTintColor = UIPager.CurrentPageIndicatorTintColor;
						UIPager.ValueChanged += UIPagerValueChanged;
					}
				
				}
				if (UIPager != null)
				{
					UpdatePagesIndicatorTintColor();
					UpdateCurrentPagesIndicatorTintColor();
					UpdatePages();
					UpdateHidesForSinglePage();
					UpdateCurrentPage();
				}
			}
			base.OnElementChanged(e);
		}

		void UIPagerValueChanged(object sender, System.EventArgs e)
		{
			if (_updatingPosition)
				return;
			Element.Position = (int)UIPager.CurrentPage;
		}

		protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			base.OnElementPropertyChanged(sender, e);
			if (UIPager == null)
				return;
			if (e.PropertyName == IndicatorColorProperty.PropertyName)
				UpdatePagesIndicatorTintColor();
			else if (e.PropertyName == SelectedIndicatorColorProperty.PropertyName)
				UpdateCurrentPagesIndicatorTintColor();
			else if (e.PropertyName == CountProperty.PropertyName)
				UpdatePages();
			else if (e.PropertyName == HideSingleProperty.PropertyName)
				UpdateHidesForSinglePage();
			else if (e.PropertyName == PositionProperty.PropertyName)
				UpdateCurrentPage();
		}

		protected override UIView CreateNativeControl()
			=> new UIPageControl();

		void UpdateControl()
		{
			var control = Element.Visual == VisualMarker.Forms
				? (UIView)Element.IndicatorLayout.GetRenderer()
				: CreateNativeControl();

			SetNativeControl(control);
		}
		bool _updatingPosition;

		void UpdateCurrentPage()
		{
			_updatingPosition = true;
			UIPager.CurrentPage = Element.Position;
			_updatingPosition = false;
		}

		void UpdatePages()
			=> UIPager.Pages = Element.Count;

		void UpdateHidesForSinglePage()
			=> UIPager.HidesForSinglePage = Element.HideSingle;

		void UpdatePagesIndicatorTintColor()
		{
			var color = Element.IndicatorColor;
			UIPager.PageIndicatorTintColor = color.IsDefault ? _defaultPagesIndicatorTintColor : color.ToUIColor();
		}

		void UpdateCurrentPagesIndicatorTintColor()
		{
			var color = Element.SelectedIndicatorColor;
			UIPager.CurrentPageIndicatorTintColor = color.IsDefault ? _defaultCurrentPagesIndicatorTintColor : color.ToUIColor();
		}
	}
}