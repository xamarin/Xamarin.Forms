using System;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Xamarin.Forms.Internals;

namespace Xamarin.Forms.Platform.UWP
{
	// TODO hartez 2018/06/28 09:08:55 This should really be named something like FormsItemControl/FormsItemsContentControl? Not sure yet.
	// ItemContentControl right now on Android
	public class FormsContentControl : ContentControl
	{
		public FormsContentControl()
		{
			DefaultStyleKey = typeof(FormsContentControl);
		}

		public static readonly DependencyProperty FormsDataTemplateProperty = DependencyProperty.Register(
			"FormsDataTemplate", typeof(DataTemplate), typeof(FormsContentControl), 
			new PropertyMetadata(default(DataTemplate), FormsDataTemplateChanged));

		static void FormsDataTemplateChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			if (e.NewValue == null)
			{
				return;
			}

			var formsContentControl = (FormsContentControl)d;
			formsContentControl.RealizeFormsDataTemplate((DataTemplate)e.NewValue);
		}

		public DataTemplate FormsDataTemplate
		{
			get => (DataTemplate)GetValue(FormsDataTemplateProperty);
			set => SetValue(FormsDataTemplateProperty, value);
		}

		// TODO hartez 2018/07/24 11:02:39 Use nameof	
		public static readonly DependencyProperty FormsDataContextProperty = DependencyProperty.Register(
			"FormsDataContext", typeof(object), typeof(FormsContentControl), 
			new PropertyMetadata(default(object), FormsDataContextChanged));

		static void FormsDataContextChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			var formsContentControl = (FormsContentControl)d;
			formsContentControl.SetFormsDataContext(e.NewValue);
		}

		public object FormsDataContext
		{
			get => GetValue(FormsDataContextProperty);
			set => SetValue(FormsDataContextProperty, value);
		}

		VisualElement _rootElement;

		internal void RealizeFormsDataTemplate(DataTemplate template)
		{
			var content = FormsDataTemplate.CreateContent();

			if (content is VisualElement visualElement)
			{
				if (_rootElement != null)
				{
					_rootElement.MeasureInvalidated -= RootElementOnMeasureInvalidated;
				}

				_rootElement = visualElement;
				_rootElement.MeasureInvalidated += RootElementOnMeasureInvalidated;

				// TODO hartez 2018/07/24 11:22:05 Using GetOrCreate might be a waste here, since we're creating the element in this method	
				// The "Get" part of GetOrCreate is checking a value that's certain to not be set. Change this to just create the renderer
				Content = visualElement.GetOrCreateRenderer().ContainerElement;
			}

			if (FormsDataContext != null)
			{
				SetFormsDataContext(FormsDataContext);
			}
		}

		void RootElementOnMeasureInvalidated(object sender, EventArgs e)
		{
			InvalidateMeasure();
		}

		internal void SetFormsDataContext(object context)
		{
			if (_rootElement == null)
			{
				return;
			}

			BindableObject.SetInheritedBindingContext(_rootElement, context);
		}

		protected override Windows.Foundation.Size MeasureOverride(Windows.Foundation.Size availableSize)
		{
			if (_rootElement == null)
			{
				return base.MeasureOverride(availableSize);
			}

			Size request = _rootElement.Measure(availableSize.Width, availableSize.Height, 
				MeasureFlags.IncludeMargins).Request;

			// TODO hartez 2018/07/24 11:01:41 With GetNativeSize working, I don't think this next set of ifs is entirely necessary 
			if (request.Width < 0)
			{
				request.Width = 100;
			}

			if (request.Height < 0)
			{
				request.Height = 100;
			}

			// TODO hartez 2018/07/25 08:51:52 To make this look nicer, how about a LayoutOrigin extension method on VisualElement that just takes a size	
			_rootElement.Layout(new Rectangle(0, 0, request.Width, request.Height));

			return new Windows.Foundation.Size(request.Width, request.Height); 
		}

		protected override Windows.Foundation.Size ArrangeOverride(Windows.Foundation.Size finalSize)
		{
			if (!(Content is FrameworkElement frameworkElement))
			{
				return finalSize;
			}
		
			frameworkElement.Arrange(new Rect(_rootElement.X, _rootElement.Y, _rootElement.Width, _rootElement.Height));
			return base.ArrangeOverride(finalSize);
		}
	}
}