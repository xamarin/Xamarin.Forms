using System;
using System.Collections.Generic;
using System.Reflection;

namespace Xamarin.Forms.Markup
{
	public static class DefaultBindableProperties
	{
		static Dictionary<string, BindableProperty> elementTypeDefaultProperty = new Dictionary<string, BindableProperty>
		{ // Key: full type name of element, Value: the default BindableProperty
			{ "Xamarin.Forms.ActivityIndicator", ActivityIndicator.IsRunningProperty },
			{ "Xamarin.Forms.BoxView", BoxView.ColorProperty },
			{ "Xamarin.Forms.Button", Button.CommandProperty },
			{ "Xamarin.Forms.DatePicker", DatePicker.DateProperty },
			{ "Xamarin.Forms.Editor", Editor.TextProperty },
			{ "Xamarin.Forms.Entry", Entry.TextProperty },
			{ "Xamarin.Forms.Image", Image.SourceProperty },
			{ "Xamarin.Forms.Label", Label.TextProperty },
			{ "Xamarin.Forms.ListView", ListView.ItemsSourceProperty },
			{ "Xamarin.Forms.MasterDetailPage", Page.TitleProperty },
			{ "Xamarin.Forms.MultiPage", Page.TitleProperty },
			{ "Xamarin.Forms.NavigationPage", Page.TitleProperty },
			{ "Xamarin.Forms.CarouselPage", Page.TitleProperty },
			{ "Xamarin.Forms.ContentPage", Page.TitleProperty },
			{ "Xamarin.Forms.Page", Page.TitleProperty },
			{ "Xamarin.Forms.TabbedPage", Page.TitleProperty },
			{ "Xamarin.Forms.TemplatedPage", Page.TitleProperty },
			{ "Xamarin.Forms.Picker", Picker.SelectedIndexProperty },
			{ "Xamarin.Forms.ProgressBar", ProgressBar.ProgressProperty },
			{ "Xamarin.Forms.SearchBar", SearchBar.SearchCommandProperty },
			{ "Xamarin.Forms.Slider", Slider.ValueProperty },
			{ "Xamarin.Forms.Stepper", Stepper.ValueProperty },
			{ "Xamarin.Forms.Switch", Switch.IsToggledProperty },
			{ "Xamarin.Forms.TableView", TableView.BindingContextProperty },
			{ "Xamarin.Forms.TimePicker", TimePicker.TimeProperty },
			{ "Xamarin.Forms.WebView", WebView.SourceProperty },
			{ "Xamarin.Forms.TextCell", TextCell.TextProperty },
			{ "Xamarin.Forms.ToolbarItem", ToolbarItem.CommandProperty },
			{ "Xamarin.Forms.TapGestureRecognizer", TapGestureRecognizer.CommandProperty },
			{ "Xamarin.Forms.Span", Span.TextProperty }
		};

		public static void Register(params BindableProperty[] properties)
		{
			foreach (var property in properties)
				elementTypeDefaultProperty.Add(property.DeclaringType.FullName, property);
		}

		// We use Element because we want to bind to Cell types as well as View types
		internal static BindableProperty GetFor(Element element)
		{
			BindableProperty defaultProperty;
			var elementType = element.GetType();
			string elementTypeName;

			do
			{
				elementTypeName = elementType.FullName;
				if (elementTypeDefaultProperty.TryGetValue(elementTypeName, out defaultProperty))
					break;
				if (elementTypeName.StartsWith("Xamarin.Forms.", StringComparison.Ordinal))
					throw new NotImplementedException(
						"No default bindable property is defined for element type." + elementTypeName +
						"\r\nEither specify a property when calling Bind() or register a default bindable property for this element type");

				elementType = elementType.GetTypeInfo().BaseType;

				if (elementType == null)
					return null;
			} while (true);

			return defaultProperty;
		}
	}
}
