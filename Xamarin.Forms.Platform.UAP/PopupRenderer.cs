using System;
using System.ComponentModel;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Xamarin.Forms.Internals;
using UWPThickness = Windows.UI.Xaml.Thickness;
using XamlStyle = Windows.UI.Xaml.Style;

namespace Xamarin.Forms.Platform.UWP
{
	public class PopupRenderer : Flyout, IVisualElementRenderer
	{
		public BasePopup Element { get; private set; }
		internal ViewToRendererConverter.WrapperControl Control { get; private set; }
		FrameworkElement IVisualElementRenderer.ContainerElement { get => null; }
		VisualElement IVisualElementRenderer.Element { get => Element; }
		
		public event EventHandler<VisualElementChangedEventArgs> ElementChanged;
		public event EventHandler<PropertyChangedEventArgs> ElementPropertyChanged;

		const double _defaultBorderThickness = 2;
		const double _defaultSize = 600;
		bool _isDisposed = false;
		XamlStyle _flyoutStyle;
		XamlStyle _panelStyle;
		public PopupRenderer()
		{
		}

		void IVisualElementRenderer.SetElement(VisualElement element)
		{
			if (element == null)
				throw new ArgumentNullException(nameof(element));

			if (!(element is BasePopup popup))
				throw new ArgumentNullException("Element is not of type " + typeof(BasePopup), nameof(element));

			BasePopup oldElement = Element;
			Element = popup;
			CreateControl();

			Performance.Start(out string reference);

			if (oldElement != null)
				oldElement.PropertyChanged -= OnElementPropertyChanged;

			element.PropertyChanged += OnElementPropertyChanged;

			OnElementChanged(new ElementChangedEventArgs<BasePopup>(oldElement, Element));

			Performance.Stop(reference);
		}

		void CreateControl()
		{
			if (Control == null)
			{
				Control = new ViewToRendererConverter.WrapperControl(Element.View);
				Content = Control;
			}
		}

		void InitializeStyles()
		{
			_flyoutStyle = new XamlStyle { TargetType = typeof(FlyoutPresenter) };
			_panelStyle = new XamlStyle { TargetType = typeof(Panel) };
		}

		protected virtual void OnElementChanged(ElementChangedEventArgs<BasePopup> e)
		{
			if (e.NewElement != null && !_isDisposed)
			{
				InitializeStyles();
				SetEvents();
				SetColor();
				SetBorderColor();
				SetSize();
				SetLayout();
				ApplyStyles();
				Show();
			}

			ElementChanged?.Invoke(this, new VisualElementChangedEventArgs(e.OldElement, e.NewElement));
		}

		protected virtual void OnElementPropertyChanged(object sender, PropertyChangedEventArgs args)
		{
			if (args.PropertyName == BasePopup.VerticalOptionsProperty.PropertyName ||
				args.PropertyName == BasePopup.HorizontalOptionsProperty.PropertyName ||
				args.PropertyName == BasePopup.SizeProperty.PropertyName)
			{
				InitializeStyles();
				SetSize();
				SetLayout();
				ApplyStyles();
			}
			else if (args.PropertyName == BasePopup.ColorProperty.PropertyName)
			{
				InitializeStyles();
				SetColor();
				SetBorderColor();
				ApplyStyles();
			}

			ElementPropertyChanged?.Invoke(this, args);
		}

		void SetEvents()
		{
			Element.Dismissed += OnDismissed;
		}

		void SetSize()
		{
			var standardSize = new Size { Width = _defaultSize, Height = _defaultSize / 2 };

			Size currentSize = Element.Size != default(Size) ? Element.Size : standardSize;
			Control.Width = currentSize.Width;
			Control.Height = currentSize.Height;

			_flyoutStyle.Setters.Add(new Windows.UI.Xaml.Setter(FlyoutPresenter.MinHeightProperty, currentSize.Height + (_defaultBorderThickness * 2)));
			_flyoutStyle.Setters.Add(new Windows.UI.Xaml.Setter(FlyoutPresenter.MinWidthProperty, currentSize.Width + (_defaultBorderThickness * 2)));
		}

		void SetLayout()
		{
			LightDismissOverlayMode = LightDismissOverlayMode.On;

			_panelStyle.Setters.Add(new Windows.UI.Xaml.Setter(Panel.HorizontalAlignmentProperty, HorizontalAlignment.Center));
			_panelStyle.Setters.Add(new Windows.UI.Xaml.Setter(Panel.VerticalAlignmentProperty, VerticalAlignment.Center));
		}

		void SetBorderColor()
		{
			_flyoutStyle.Setters.Add(new Windows.UI.Xaml.Setter(FlyoutPresenter.PaddingProperty, 0));
			_flyoutStyle.Setters.Add(new Windows.UI.Xaml.Setter(FlyoutPresenter.BorderThicknessProperty, new UWPThickness(_defaultBorderThickness)));

			if (Element.BorderColor == default(Color))
				_flyoutStyle.Setters.Add(new Windows.UI.Xaml.Setter(FlyoutPresenter.BorderBrushProperty, Color.FromHex("#2e6da0").ToWindowsColor()));
			else
				_flyoutStyle.Setters.Add(new Windows.UI.Xaml.Setter(FlyoutPresenter.BorderBrushProperty, Element.BorderColor.ToWindowsColor()));
		}

		void SetColor()
		{
			if (Element.View.BackgroundColor == default(Color))
				_panelStyle.Setters.Add(new Windows.UI.Xaml.Setter(Panel.BackgroundProperty, Element.Color.ToWindowsColor()));

			_flyoutStyle.Setters.Add(new Windows.UI.Xaml.Setter(FlyoutPresenter.BackgroundProperty, Element.Color.ToWindowsColor()));
		}

		void ApplyStyles()
		{
			Control.Style = _panelStyle;
			FlyoutPresenterStyle = _flyoutStyle;
		}

		void Show()
		{
			if (Element.Anchor != null)
			{
				var anchor = Platform.GetRenderer(Element.Anchor);
				var anchorElement = anchor.ContainerElement;
				FlyoutBase.SetAttachedFlyout(anchorElement, this);
				FlyoutBase.ShowAttachedFlyout(anchorElement);
			}
			else
			{
				var frameworkElement = Platform.Current.NavigationStack[0].ToFrameworkElement();
				SetDialogPosition(Element.VerticalOptions, Element.HorizontalOptions);
				FlyoutBase.SetAttachedFlyout(frameworkElement, this);
				FlyoutBase.ShowAttachedFlyout(frameworkElement);
			}
		}

		void SetDialogPosition(LayoutOptions verticalOptions, LayoutOptions horizontalOptions)
		{
			Placement = FlyoutPlacementMode.Full;

			switch (verticalOptions.Alignment)
			{
				case LayoutAlignment.Start:
					Placement = FlyoutPlacementMode.Top;
					break;
				case LayoutAlignment.End:
					Placement = FlyoutPlacementMode.Bottom;
					break;
			}

			switch (horizontalOptions.Alignment)
			{
#if !UWP_14393
				case LayoutAlignment.Start when Placement == FlyoutPlacementMode.Top:
					Placement = FlyoutPlacementMode.TopEdgeAlignedLeft;
					break;
				case LayoutAlignment.End when Placement == FlyoutPlacementMode.Top:
					Placement = FlyoutPlacementMode.TopEdgeAlignedRight;
					break;
				case LayoutAlignment.Start when Placement == FlyoutPlacementMode.Bottom:
					Placement = FlyoutPlacementMode.BottomEdgeAlignedLeft;
					break;
				case LayoutAlignment.End when Placement == FlyoutPlacementMode.Bottom:
					Placement = FlyoutPlacementMode.BottomEdgeAlignedRight;
					break;
#endif
				case LayoutAlignment.Start:
					Placement = FlyoutPlacementMode.Left;
					break;
				case LayoutAlignment.End:
					Placement = FlyoutPlacementMode.Right;
					break;
			}
		}

		SizeRequest IVisualElementRenderer.GetDesiredSize(double widthConstraint, double heightConstraint)
		{
			if (_isDisposed || Control == null)
				return new SizeRequest();

			var constraint = new Windows.Foundation.Size(widthConstraint, heightConstraint);
			Control.Measure(constraint);

			var size = new Size(Math.Ceiling(Control.DesiredSize.Width), Math.Ceiling(Control.DesiredSize.Height));
			return new SizeRequest(size);
		}

		UIElement IVisualElementRenderer.GetNativeElement() => Control;

		void OnDismissed(object sender, PopupDismissedEventArgs e)
		{
			Hide();
		}

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		protected virtual void Dispose(bool disposing)
		{
			if (!_isDisposed && disposing)
			{
				Element.Dismissed -= OnDismissed;
				Element = null;
			}

			_isDisposed = true;
		}
	}
}
