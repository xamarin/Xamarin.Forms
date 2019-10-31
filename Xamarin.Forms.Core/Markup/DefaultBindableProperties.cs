using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;

namespace Xamarin.Forms.Markup
{
	public static class DefaultBindableProperties
	{
		static Dictionary<string, BindableProperty> viewTypeDefaultProperty = new Dictionary<string, BindableProperty>
		{ // Key: full type name of view, Value: the default BindableProperty
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

		// Note that we use Element type for the view variable in bind functions because we want to bind to 
		// Cell types as well as View types
		internal static BindableProperty GetFor(Element view)
		{
			BindableProperty defaultProperty;
			var viewType = view.GetType();
			string viewTypeName;

			do
			{
				viewTypeName = viewType.FullName;
				if (viewTypeDefaultProperty.TryGetValue(viewTypeName, out defaultProperty))
					break;
				if (viewTypeName.StartsWith("Xamarin.Forms.", StringComparison.Ordinal))
					throw new NotImplementedException(
						"No default bindable property is defined for view type." + viewTypeName +
						"\r\nEither specify a property when calling Bind() or register a default bindable property for this view type");

				viewType = viewType.GetTypeInfo().BaseType;

				if (viewType == null)
					return null;
			} while (true);

			return defaultProperty;
		}

		public static void Register(params BindableProperty[] properties)
		{
			foreach (var property in properties)
				viewTypeDefaultProperty.Add(property.DeclaringType.FullName, property);
		}
	}
}
