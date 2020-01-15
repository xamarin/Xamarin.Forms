using System.Collections.Generic;
using System.Linq;

namespace Xamarin.Forms.Markup
{
	public static class ViewGesturesExtensions
	{
		const string bindingContextPath = Binding.SelfPath;

		/// <summary>Bind a specified property of this <typeparamref name="TView"/>'s <typeparamref name="TGestureRecognizer"/></summary>
		/// <remarks>Creates / updates the <typeparamref name="TGestureRecognizer"/> as needed</remarks>
		public static TView Bind<TView, TGestureRecognizer>(
			this TView view,
			BindableProperty targetProperty,
			string path = bindingContextPath,
			BindingMode mode = BindingMode.Default,
			IValueConverter converter = null,
			object converterParameter = null,
			string stringFormat = null,
			object source = null,
			object targetNullValue = null,
			object fallbackValue = null
		) where TView : View where TGestureRecognizer : GestureRecognizer, new()
		{
			Bind<TGestureRecognizer>(view.GestureRecognizers, targetProperty, path, mode, converter,
				converterParameter, stringFormat, source, targetNullValue, fallbackValue);
			return view;
		}

		/// <summary>Bind the default property of this <typeparamref name="TView"/>'s <typeparamref name="TGestureRecognizer"/></summary>
		/// <remarks>Creates / updates the <typeparamref name="TGestureRecognizer"/> as needed</remarks>
		public static TView Bind<TView, TGestureRecognizer>(
			this TView view,
			string path = bindingContextPath,
			BindingMode mode = BindingMode.Default,
			IValueConverter converter = null,
			object converterParameter = null,
			string stringFormat = null,
			object source = null,
			object targetNullValue = null,
			object fallbackValue = null
		) where TView : View where TGestureRecognizer : GestureRecognizer, new()
		{
			Bind<TGestureRecognizer>(view.GestureRecognizers, null, path, mode, converter,
				converterParameter, stringFormat, source, targetNullValue, fallbackValue);
			return view;
		}

		/// <summary>Bind a specified property of this <typeparamref name="TGestureElement"/>'s <typeparamref name="TGestureRecognizer"/></summary>
		/// <remarks>Creates / updates the <typeparamref name="TGestureRecognizer"/> as needed</remarks>
		public static TGestureElement BindGesture<TGestureElement, TGestureRecognizer>(
			this TGestureElement gestureElement,
			BindableProperty targetProperty,
			string path = bindingContextPath,
			BindingMode mode = BindingMode.Default,
			IValueConverter converter = null,
			object converterParameter = null,
			string stringFormat = null,
			object source = null,
			object targetNullValue = null,
			object fallbackValue = null
		) where TGestureElement : GestureElement where TGestureRecognizer : GestureRecognizer, new()
		{
			Bind<TGestureRecognizer>(gestureElement.GestureRecognizers, targetProperty, path, mode,
				converter, converterParameter, stringFormat, source, targetNullValue, fallbackValue);
			return gestureElement;
		}

		/// <summary>Bind the default property of this <typeparamref name="TGestureElement"/>'s <typeparamref name="TGestureRecognizer"/></summary>
		/// <remarks>Creates / updates the <typeparamref name="TGestureRecognizer"/> as needed</remarks>
		public static TGestureElement BindGesture<TGestureElement, TGestureRecognizer>(
			this TGestureElement gestureElement,
			string path = bindingContextPath,
			BindingMode mode = BindingMode.Default,
			IValueConverter converter = null,
			object converterParameter = null,
			string stringFormat = null,
			object source = null,
			object targetNullValue = null,
			object fallbackValue = null
		) where TGestureElement : GestureElement where TGestureRecognizer : GestureRecognizer, new()
		{
			Bind<TGestureRecognizer>(gestureElement.GestureRecognizers, null, path, mode, converter,
				converterParameter, stringFormat, source, targetNullValue, fallbackValue);
			return gestureElement;
		}

		/// <summary>Bind the <see cref="TapGestureRecognizer.Command"/> property of this <typeparamref name="TView"/>'s <see cref="TapGestureRecognizer"/></summary>
		/// <remarks>Creates / updates the <see cref="TapGestureRecognizer"/> as needed</remarks>
		public static TView BindTapGesture<TView>(
			this TView view,
			string path = bindingContextPath,
			string commandParameterPropertyName = null,
			object commandParameter = null,
			BindingMode mode = BindingMode.Default,
			IValueConverter converter = null,
			object converterParameter = null,
			string stringFormat = null,
			object source = null,
			object commandParameterSource = null,
			object targetNullValue = null,
			object fallbackValue = null
		) where TView : View
		{
			BindTap(view.GestureRecognizers, path, commandParameterPropertyName, commandParameter, mode,
				converter, converterParameter, stringFormat, source, commandParameterSource,
				targetNullValue, fallbackValue);
			return view;
		}

		/// <summary>Bind the <see cref="TapGestureRecognizer.Command"/> property of this <typeparamref name="TGestureElement"/>'s <see cref="TapGestureRecognizer"/></summary>
		/// <remarks>Creates / updates the <see cref="TapGestureRecognizer"/> as needed</remarks>
		public static TGestureElement BindTap<TGestureElement>(
			this TGestureElement gestureElement,
			string path = bindingContextPath,
			string commandParameterPropertyName = null,
			object commandParameter = null,
			BindingMode mode = BindingMode.Default,
			IValueConverter converter = null,
			object converterParameter = null,
			string stringFormat = null,
			object source = null,
			object commandParameterSource = null,
			object targetNullValue = null,
			object fallbackValue = null
		) where TGestureElement : GestureElement
		{
			BindTap(gestureElement.GestureRecognizers, path, commandParameterPropertyName,
				commandParameter, mode, converter, converterParameter, stringFormat, source, commandParameterSource,
				targetNullValue, fallbackValue);
			return gestureElement;
		}

		static void Bind<TGestureRecognizer>(
			IList<IGestureRecognizer> gestureRecognizers,
			BindableProperty targetProperty,
			string path,
			BindingMode mode,
			IValueConverter converter,
			object converterParameter,
			string stringFormat,
			object source,
			object targetNullValue = null,
			object fallbackValue = null
		) where TGestureRecognizer : GestureRecognizer, new()
		{
			var gestureRecognizer = (TGestureRecognizer)gestureRecognizers.FirstOrDefault(r => r is TGestureRecognizer);
			if (gestureRecognizer == null)
				gestureRecognizers.Add(gestureRecognizer = new TGestureRecognizer());

			if (targetProperty == null)
				targetProperty = DefaultBindableProperties.GetFor(gestureRecognizer);

			if (source != null || converterParameter != null || targetNullValue != null || fallbackValue != null)
				gestureRecognizer.SetBinding(targetProperty, new Binding(
					path: path,
					mode: mode,
					converter: converter,
					converterParameter: converterParameter,
					stringFormat: stringFormat,
					source: source
				)
				{
					TargetNullValue = targetNullValue,
					FallbackValue = fallbackValue
				});
			else
				gestureRecognizer.SetBinding(targetProperty, path, mode, converter, stringFormat);
		}

		static void BindTap(
			IList<IGestureRecognizer> gestureRecognizers,
			string path,
			string commandParameterPropertyName,
			object commandParameter,
			BindingMode mode,
			IValueConverter converter,
			object converterParameter,
			string stringFormat,
			object source,
			object commandParameterSource,
			object targetNullValue = null,
			object fallbackValue = null
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

			if (source != null || converterParameter != null || targetNullValue != null || fallbackValue != null)
				gestureRecognizer.SetBinding(targetProperty, new Binding(
					path: path,
					mode: mode,
					converter: converter,
					converterParameter: converterParameter,
					stringFormat: stringFormat,
					source: source
				)
				{
					TargetNullValue = targetNullValue,
					FallbackValue = fallbackValue
				});
			else
				gestureRecognizer.SetBinding(targetProperty, path, mode, converter, stringFormat);
		}
	}
}
