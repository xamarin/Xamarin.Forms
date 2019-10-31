using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;

namespace Xamarin.Forms.Markup
{
	public static class ViewGesturesExtensions
	{
		const string bindingContextPropertyName = ".";

		public static TView Bind<TView, TGestureRecognizer>(
			this TView view,
			BindableProperty targetProperty,
			string sourcePropertyName = bindingContextPropertyName,
			BindingMode mode = BindingMode.Default,
			IValueConverter converter = null,
			object converterParameter = null,
			string stringFormat = null,
			object source = null
		) where TView : View where TGestureRecognizer : GestureRecognizer, new()
		{
			Bind<TGestureRecognizer>(view.GestureRecognizers, targetProperty, sourcePropertyName, mode, converter,
				converterParameter, stringFormat, source);
			return view;
		}

		public static TView Bind<TView, TGestureRecognizer>(
			this TView view,
			string sourcePropertyName = bindingContextPropertyName,
			BindingMode mode = BindingMode.Default,
			IValueConverter converter = null,
			object converterParameter = null,
			string stringFormat = null,
			object source = null
		) where TView : View where TGestureRecognizer : GestureRecognizer, new()
		{
			Bind<TGestureRecognizer>(view.GestureRecognizers, null, sourcePropertyName, mode, converter,
				converterParameter, stringFormat, source);
			return view;
		}

		public static TGestureElement BindGesture<TGestureElement, TGestureRecognizer>(
			this TGestureElement gestureElement,
			BindableProperty targetProperty,
			string sourcePropertyName = bindingContextPropertyName,
			BindingMode mode = BindingMode.Default,
			IValueConverter converter = null,
			object converterParameter = null,
			string stringFormat = null,
			object source = null
		) where TGestureElement : GestureElement where TGestureRecognizer : GestureRecognizer, new()
		{
			Bind<TGestureRecognizer>(gestureElement.GestureRecognizers, targetProperty, sourcePropertyName, mode,
				converter, converterParameter, stringFormat, source);
			return gestureElement;
		}

		public static TGestureElement BindGesture<TGestureElement, TGestureRecognizer>(
			this TGestureElement gestureElement,
			string sourcePropertyName = bindingContextPropertyName,
			BindingMode mode = BindingMode.Default,
			IValueConverter converter = null,
			object converterParameter = null,
			string stringFormat = null,
			object source = null
		) where TGestureElement : GestureElement where TGestureRecognizer : GestureRecognizer, new()
		{
			Bind<TGestureRecognizer>(gestureElement.GestureRecognizers, null, sourcePropertyName, mode, converter,
				converterParameter, stringFormat, source);
			return gestureElement;
		}

		static void Bind<TGestureRecognizer>(
			IList<IGestureRecognizer> gestureRecognizers,
			BindableProperty targetProperty,
			string sourcePropertyName,
			BindingMode mode,
			IValueConverter converter,
			object converterParameter,
			string stringFormat,
			object source
		) where TGestureRecognizer : GestureRecognizer, new()
		{
			var gestureRecognizer = (TGestureRecognizer)gestureRecognizers.FirstOrDefault(r => r is TGestureRecognizer);
			if (gestureRecognizer == null)
				gestureRecognizers.Add(gestureRecognizer = new TGestureRecognizer());

			if (targetProperty == null)
				targetProperty = DefaultBindableProperties.GetFor(gestureRecognizer);

			if (source != null || converterParameter != null)
				gestureRecognizer.SetBinding(targetProperty, new Binding(
					path: sourcePropertyName,
					mode: mode,
					converter: converter,
					converterParameter: converterParameter,
					stringFormat: stringFormat,
					source: source
				));
			else
				gestureRecognizer.SetBinding(targetProperty, sourcePropertyName, mode, converter, stringFormat);
		}

		public static TView BindTapGesture<TView>(
			this TView view,
			string sourcePropertyName = bindingContextPropertyName,
			string commandParameterPropertyName = null,
			object commandParameter = null,
			BindingMode mode = BindingMode.Default,
			IValueConverter converter = null,
			object converterParameter = null,
			string stringFormat = null,
			object source = null,
			object commandParameterSource = null
		) where TView : View
		{
			BindTap(view.GestureRecognizers, sourcePropertyName, commandParameterPropertyName, commandParameter, mode,
				converter, converterParameter, stringFormat, source, commandParameterSource);
			return view;
		}

		public static TGestureElement BindTap<TGestureElement>(
			this TGestureElement gestureElement,
			string sourcePropertyName = bindingContextPropertyName,
			string commandParameterPropertyName = null,
			object commandParameter = null,
			BindingMode mode = BindingMode.Default,
			IValueConverter converter = null,
			object converterParameter = null,
			string stringFormat = null,
			object source = null,
			object commandParameterSource = null
		) where TGestureElement : GestureElement
		{
			BindTap(gestureElement.GestureRecognizers, sourcePropertyName, commandParameterPropertyName,
				commandParameter, mode, converter, converterParameter, stringFormat, source, commandParameterSource);
			return gestureElement;
		}

		static void BindTap(
			IList<IGestureRecognizer> gestureRecognizers,
			string sourcePropertyName,
			string commandParameterPropertyName,
			object commandParameter,
			BindingMode mode,
			IValueConverter converter,
			object converterParameter,
			string stringFormat,
			object source,
			object commandParameterSource
		)
		{
			var gestureRecognizer = (TapGestureRecognizer)gestureRecognizers
				.FirstOrDefault(r => r is TapGestureRecognizer);
			if (gestureRecognizer == null)
				gestureRecognizers.Add(gestureRecognizer = new TapGestureRecognizer());

			if (commandParameterPropertyName != null)
				gestureRecognizer.SetBinding(TapGestureRecognizer.CommandParameterProperty,
					new Binding(path: commandParameterPropertyName, source: commandParameterSource));
			else if (commandParameter != null)
				gestureRecognizer.CommandParameter = commandParameter;

			var targetProperty = TapGestureRecognizer.CommandProperty;

			if (source != null || converterParameter != null)
				gestureRecognizer.SetBinding(targetProperty, new Binding(
					path: sourcePropertyName,
					mode: mode,
					converter: converter,
					converterParameter: converterParameter,
					stringFormat: stringFormat,
					source: source
				));
			else
				gestureRecognizer.SetBinding(targetProperty, sourcePropertyName, mode, converter, stringFormat);
		}
	}
}
