using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using XamlStyle = Windows.UI.Xaml.Style;
using UWPThickness = Windows.UI.Xaml.Thickness;

namespace Xamarin.Forms.Platform.UWP
{
	public class PopupRenderer : IVisualElementRenderer
	{
		private const double _defaultBorderThickness = 2;
		bool _disposed = false;
		Flyout _flyout;
		public FrameworkElement ContainerElement { get; protected set; }

		public BasePopup Element { get; protected set; }
		VisualElement IVisualElementRenderer.Element
		{
			get { return Element; }
		}

		public event EventHandler<VisualElementChangedEventArgs> ElementChanged;

		public PopupRenderer()
		{
		}

		protected virtual void OnElementChanged(ElementChangedEventArgs<BasePopup> e)
		{
			Element = e.NewElement;
			ElementChanged?.Invoke(this, new VisualElementChangedEventArgs(e.OldElement, e.NewElement));
		}

		public SizeRequest GetDesiredSize(double widthConstraint, double heightConstraint)
		{
			throw new NotImplementedException();
		}

		public UIElement GetNativeElement()
		{
			return null;
		}

		public void SetElement(VisualElement element)
		{
			var newPopup = (BasePopup)element;
			if (newPopup == null)
				return;

			BasePopup oldPopup = Element;

			var standardSize = new Size { Width = 600, Height = 300 };
			_flyout = new Flyout
			{
				LightDismissOverlayMode = LightDismissOverlayMode.On
			};

			var content = new ViewToRendererConverter.WrapperControl(newPopup.View);
			if (newPopup.View.BackgroundColor == default(Color))
			{
				var panelStyle = new XamlStyle { TargetType = typeof(Panel) };
				panelStyle.Setters.Add(new Windows.UI.Xaml.Setter(Panel.BackgroundProperty, newPopup.Color.ToWindowsColor()));
				panelStyle.Setters.Add(new Windows.UI.Xaml.Setter(Panel.HorizontalAlignmentProperty, HorizontalAlignment.Center));
				panelStyle.Setters.Add(new Windows.UI.Xaml.Setter(Panel.VerticalAlignmentProperty, VerticalAlignment.Center));
				content.Style = panelStyle;
			}

			Size currentSize = newPopup.Size != default(Size) ? newPopup.Size : standardSize;
			content.Width = currentSize.Width;
			content.Height = currentSize.Height;

			var flyoutStyle = new XamlStyle { TargetType = typeof(FlyoutPresenter) };

			if (newPopup.BorderColor == default(Color))
			{
				flyoutStyle.Setters.Add(new Windows.UI.Xaml.Setter(FlyoutPresenter.BorderBrushProperty, Color.FromHex("#2e6da0").ToWindowsColor()));
			}
			else
			{
				flyoutStyle.Setters.Add(new Windows.UI.Xaml.Setter(FlyoutPresenter.BorderBrushProperty, newPopup.BorderColor.ToWindowsColor()));
			}

			flyoutStyle.Setters.Add(new Windows.UI.Xaml.Setter(FlyoutPresenter.MinHeightProperty, currentSize.Height + (_defaultBorderThickness * 2)));
			flyoutStyle.Setters.Add(new Windows.UI.Xaml.Setter(FlyoutPresenter.MinWidthProperty, currentSize.Width + (_defaultBorderThickness * 2)));
			flyoutStyle.Setters.Add(new Windows.UI.Xaml.Setter(FlyoutPresenter.BackgroundProperty, newPopup.Color.ToWindowsColor()));
			flyoutStyle.Setters.Add(new Windows.UI.Xaml.Setter(FlyoutPresenter.PaddingProperty, 0));
			flyoutStyle.Setters.Add(new Windows.UI.Xaml.Setter(FlyoutPresenter.BorderThicknessProperty, new UWPThickness(_defaultBorderThickness) ));

			_flyout.FlyoutPresenterStyle = flyoutStyle;
			_flyout.Content = content;

			if (newPopup.Anchor != null)
			{
				var anchor = Platform.GetRenderer(newPopup.Anchor);
				var anchorElement = anchor.ContainerElement;
				FlyoutBase.SetAttachedFlyout(anchorElement, _flyout);
				FlyoutBase.ShowAttachedFlyout(anchorElement);
			}
			else
			{
				FlyoutBase.SetAttachedFlyout(Platform.Current.NavigationStack[0].ToFrameworkElement(), _flyout);
				FlyoutBase.ShowAttachedFlyout(Platform.Current.NavigationStack[0].ToFrameworkElement());
			}

			newPopup.Dismissed += OnDismissed;
			OnElementChanged(new ElementChangedEventArgs<BasePopup>(oldPopup, newPopup));
		}

		private void OnDismissed(object sender, PopupDismissedEventArgs e)
		{
			_flyout.Hide();
		}

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		protected virtual void Dispose(bool disposing)
		{
			if (!_disposed && disposing)
			{
				Element.Dismissed -= OnDismissed;
				Element = null;
				_flyout = null;
			}

			_disposed = true;
		}
	}
}
