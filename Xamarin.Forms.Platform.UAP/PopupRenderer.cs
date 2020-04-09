using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using UWPThickness = Windows.UI.Xaml.Thickness;
using XamlStyle = Windows.UI.Xaml.Style;

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
			flyoutStyle.Setters.Add(new Windows.UI.Xaml.Setter(FlyoutPresenter.BorderThicknessProperty, new UWPThickness(_defaultBorderThickness)));

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
				var frameworkElement = Platform.Current.NavigationStack[0].ToFrameworkElement();
				SetDialogPosition(newPopup.VerticalOptions, newPopup.HorizontalOptions);
				FlyoutBase.SetAttachedFlyout(frameworkElement, _flyout);
				FlyoutBase.ShowAttachedFlyout(frameworkElement);
			}

			newPopup.Dismissed += OnDismissed;
			OnElementChanged(new ElementChangedEventArgs<BasePopup>(oldPopup, newPopup));
		}

		void SetDialogPosition(LayoutOptions verticalOptions, LayoutOptions horizontalOptions)
		{
			_flyout.Placement = FlyoutPlacementMode.Full;

			switch (verticalOptions.Alignment)
			{
				case LayoutAlignment.Start:
					_flyout.Placement = FlyoutPlacementMode.Top;
					break;
				case LayoutAlignment.End:
					_flyout.Placement = FlyoutPlacementMode.Bottom;
					break;
			}

			switch (horizontalOptions.Alignment)
			{
#if !UWP_14393
				case LayoutAlignment.Start when _flyout.Placement == FlyoutPlacementMode.Top:
					_flyout.Placement = FlyoutPlacementMode.TopEdgeAlignedLeft;
					break;
				case LayoutAlignment.End when _flyout.Placement == FlyoutPlacementMode.Top:
					_flyout.Placement = FlyoutPlacementMode.TopEdgeAlignedRight;
					break;
				case LayoutAlignment.Start when _flyout.Placement == FlyoutPlacementMode.Bottom:
					_flyout.Placement = FlyoutPlacementMode.BottomEdgeAlignedLeft;
					break;
				case LayoutAlignment.End when _flyout.Placement == FlyoutPlacementMode.Bottom:
					_flyout.Placement = FlyoutPlacementMode.BottomEdgeAlignedRight;
					break;
#endif
				case LayoutAlignment.Start:
					_flyout.Placement = FlyoutPlacementMode.Left;
					break;
				case LayoutAlignment.End:
					_flyout.Placement = FlyoutPlacementMode.Right;
					break;
			}
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
