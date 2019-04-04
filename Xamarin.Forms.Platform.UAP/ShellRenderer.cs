using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Xamarin.Forms.Platform.UWP;
using UGrid= Windows.UI.Xaml.Controls.Grid;
using UFrame = Windows.UI.Xaml.Controls.Frame;
using URowDefinition = Windows.UI.Xaml.Controls.RowDefinition;

namespace Xamarin.Forms.Platform.UWP
{
	public class ShellRenderer : SplitView, IVisualElementRenderer, /*IShellContext, */IAppearanceObserver
	{
		event EventHandler<VisualElementChangedEventArgs> _elementChanged;

		public ShellRenderer()
		{
			//Everything in this constructor is an early hack to render something
			//To be replaced with renderers for each item

			//Flyout:
			this.IsPaneOpen = true;
			this.PaneBackground = new Windows.UI.Xaml.Media.SolidColorBrush(Windows.UI.Colors.Gray);
			//Content area
			var content = new UGrid();
			content.Children.Add(new UFrame());
			content.RowDefinitions.Add(new URowDefinition() { Height = new Windows.UI.Xaml.GridLength(1, Windows.UI.Xaml.GridUnitType.Star) });
			content.RowDefinitions.Add(new URowDefinition() { Height = new Windows.UI.Xaml.GridLength() });
			//Bottom bar
			var bottomBar = new UGrid() { Background = new SolidColorBrush(Colors.CornflowerBlue), MinHeight = 40 };
			UGrid.SetRow(bottomBar, 1);
			content.Children.Add(bottomBar);
			this.Content = content;
		}

		#region IVisualElementRenderer

		event EventHandler<VisualElementChangedEventArgs> IVisualElementRenderer.ElementChanged
		{
			add { _elementChanged += value; }
			remove { _elementChanged -= value; }
		}

		FrameworkElement IVisualElementRenderer.ContainerElement => this;

		VisualElement IVisualElementRenderer.Element => Element;

		//VisualElementTracker IVisualElementRenderer.Tracker => null;

		//AView IVisualElementRenderer.View => _flyoutRenderer.AndroidView;
		//ViewGroup IVisualElementRenderer.ViewGroup => _flyoutRenderer.AndroidView as ViewGroup;

		SizeRequest IVisualElementRenderer.GetDesiredSize(double widthConstraint, double heightConstraint)
		{
			var constraint = new Windows.Foundation.Size(widthConstraint, heightConstraint);

			double oldWidth = Width;
			double oldHeight = Height;

			Height = double.NaN;
			Width = double.NaN;

			Measure(constraint);
			var result = new Size(Math.Ceiling(DesiredSize.Width), Math.Ceiling(DesiredSize.Height));

			Width = oldWidth;
			Height = oldHeight;

			return new SizeRequest(result);
		}

		public UIElement GetNativeElement() => null;

		public void Dispose()
		{
			SetElement(null);
		}

		public void SetElement(VisualElement element)
		{
			if (Element != null)
				throw new NotSupportedException("Reuse of the Shell Renderer is not supported");
			Element = (Shell)element;
			Element.SizeChanged += OnElementSizeChanged;
			OnElementSet(Element);

			_elementChanged?.Invoke(this, new VisualElementChangedEventArgs(null, Element));
		}

		#endregion IVisualElementRenderer

		protected Shell Element { get; private set; }

		void OnElementSizeChanged(object sender, EventArgs e)
		{
			//int width = (int)AndroidContext.ToPixels(Element.Width);
			//int height = (int)AndroidContext.ToPixels(Element.Height);
			//_flyoutRenderer.AndroidView.Measure(MeasureSpecFactory.MakeMeasureSpec(width, MeasureSpecMode.Exactly),
			//	MeasureSpecFactory.MakeMeasureSpec(height, MeasureSpecMode.Exactly));
			//_flyoutRenderer.AndroidView.Layout(0, 0, width, height);
		}

		protected virtual void OnElementSet(Shell shell)
		{
			//_flyoutRenderer = CreateShellFlyoutRenderer();
			//_frameLayout = new CustomFrameLayout(AndroidContext)
			//{
			//	LayoutParameters = new LP(LP.MatchParent, LP.MatchParent),
			//	Id = Platform.GenerateViewId(),
			//};
			//_frameLayout.SetFitsSystemWindows(true);
			//
			//_flyoutRenderer.AttachFlyout(this, _frameLayout);
			//
			//((IShellController)shell).AddAppearanceObserver(this, shell);
			//
			//SwitchFragment(FragmentManager, _frameLayout, shell.CurrentItem, false);
		}

		#region IAppearanceObserver

		void IAppearanceObserver.OnAppearanceChanged(ShellAppearance appearance)
		{
			//TODO
			//UpdateStatusBarColor(appearance);
		}

		#endregion IAppearanceObserver
	}
}
