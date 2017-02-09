using System;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Controls;

namespace Xamarin.Forms.Platform.WPF
{
	internal class BackgroundTracker<T> : VisualElementTracker<Page, T> where T : FrameworkElement
	{
		readonly DependencyProperty _backgroundProperty;

		bool _backgroundNeedsUpdate = true;

		public BackgroundTracker(DependencyProperty backgroundProperty)
		{
			if (backgroundProperty == null)
				throw new ArgumentNullException("backgroundProperty");

			_backgroundProperty = backgroundProperty;
		}

		protected override void HandlePropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			if (e.PropertyName == VisualElement.BackgroundColorProperty.PropertyName || e.PropertyName == Page.BackgroundImageProperty.PropertyName)
				UpdateBackground();

			base.HandlePropertyChanged(sender, e);
		}

		protected override void UpdateNativeControl()
		{
			base.UpdateNativeControl();

			if (_backgroundNeedsUpdate)
				UpdateBackground();
		}

		void UpdateBackground()
		{
			if (Model == null || Element == null)
				return;

			if (Model.BackgroundImage != null)
			{
				Element.SetValue(_backgroundProperty, new ImageBrush { ImageSource = new BitmapImage(new Uri(Model.BackgroundImage, UriKind.Relative)) });
			}
			else if (Model.BackgroundColor != Color.Default)
				Element.SetValue(_backgroundProperty, Model.BackgroundColor.ToBrush());

			_backgroundNeedsUpdate = false;
		}
	}

	public class CarouselPagePresenter : System.Windows.Controls.ContentPresenter
	{
		public override void OnApplyTemplate()
		{
			base.OnApplyTemplate();

			DependencyObject parent = VisualTreeHelper.GetParent(this);
			while (parent != null)
				parent = VisualTreeHelper.GetParent(parent);
            
			var element = (FrameworkElement)VisualTreeHelper.GetChild(this, 0);
			element.SizeChanged += (s, e) =>
			{
				if (element.ActualWidth > 0 && element.ActualHeight > 0)
				{
					var carouselItem = (Page)DataContext;
					((IPageController)carouselItem.RealParent).ContainerArea = new Rectangle(0, 0, element.ActualWidth, element.ActualHeight);
				}
			};
		}
	}

	public class CarouselPageRenderer : ContentControl, IVisualElementRenderer
	{


		CarouselPage _page;
        
		public UIElement ContainerElement
		{
			get { return this; }
		}

		public VisualElement Element
		{
			get { return _page; }
		}

		public event EventHandler<VisualElementChangedEventArgs> ElementChanged;

		public SizeRequest GetDesiredSize(double widthConstraint, double heightConstraint)
		{
			return new SizeRequest(new Size(widthConstraint, heightConstraint));
		}

		public void SetElement(VisualElement element)
		{
			CarouselPage oldElement = _page;
			_page = (CarouselPage)element;

			DataContext = _page;
            
			Loaded += (sender, args) => ((IPageController)_page).SendAppearing();
			Unloaded += (sender, args) => ((IPageController)_page).SendDisappearing();

			OnElementChanged(new VisualElementChangedEventArgs(oldElement, element));
		}

		protected virtual void OnElementChanged(VisualElementChangedEventArgs e)
		{
			EventHandler<VisualElementChangedEventArgs> changed = ElementChanged;
			if (changed != null)
				changed(this, e);
		}
        
	}
}