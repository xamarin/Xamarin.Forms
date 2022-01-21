using System;
using UIKit;
using Xamarin.Forms.Internals;

namespace Xamarin.Forms.Platform.iOS
{
	internal static class TemplateHelpers
	{
		public readonly static DataTemplate DefaultTemplate =
			new Lazy<DataTemplate>(() => new DataTemplate(() =>
			                                              {
				                                              var l = new Label();
				                                              l.SetBinding(Label.TextProperty, ".");
				                                              return l;
			                                              })).Value;

		public static IVisualElementRenderer CreateRenderer(View view)
		{
			if (view == null)
			{
				throw new ArgumentNullException(nameof(view));
			}

			Platform.GetRenderer(view)?.DisposeRendererAndChildren();
			var renderer = Platform.CreateRenderer(view);
			Platform.SetRenderer(view, renderer);

			renderer.NativeView.Bounds = view.Bounds.ToRectangleF();

			return renderer;
		}

		public static (UIView NativeView, VisualElement FormsElement) RealizeView(object view, DataTemplate viewTemplate, ItemsView itemsView)
		{
			// Run this through the extension method in case it's really a DataTemplateSelector
			var itemTemplate = viewTemplate?.SelectDataTemplate(view, itemsView) ??
			                   TemplateHelpers.DefaultTemplate;

			if (itemTemplate != null)
			{
				// We have a template; turn it into a Forms view 
				var templateElement = itemTemplate.CreateContent() as View;

				// Make sure the Visual property is available when the renderer is created
				PropertyPropagationExtensions.PropagatePropertyChanged(null, templateElement, itemsView);

				var renderer = CreateRenderer(templateElement);

				// and set the view as its BindingContext
				renderer.Element.BindingContext = view;

				return (renderer.NativeView, renderer.Element);
			}

			if (view is View formsView)
			{
				// Make sure the Visual property is available when the renderer is created
				PropertyPropagationExtensions.PropagatePropertyChanged(null, formsView, itemsView);

				// No template, and the EmptyView is a Forms view; use that
				var renderer = CreateRenderer(formsView);

				return (renderer.NativeView, renderer.Element);
			}

			return (new UILabel { TextAlignment = UITextAlignment.Center, Text = $"{view}" }, null);
		}
	}
}