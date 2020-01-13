using System;
using System.Collections.Generic;
using System.Reflection;

namespace Xamarin.Forms.Markup
{
	public static class DefaultBindableProperties
	{
		static Dictionary<string, BindableProperty> bindableObjectTypeDefaultProperty = new Dictionary<string, BindableProperty>
		{ // Key: full type name of BindableObject, Value: the default BindableProperty
		  // Note that we don't specify default properties for unconstructed generic types
			{ "Xamarin.Forms.ActivityIndicator", ActivityIndicator.IsRunningProperty },
			{ "Xamarin.Forms.BackButtonBehavior", BackButtonBehavior.CommandProperty },
			{ "Xamarin.Forms.BoxView", BoxView.ColorProperty },
			{ "Xamarin.Forms.Button", Button.CommandProperty },
			{ "Xamarin.Forms.CarouselPage", Page.TitleProperty },
			{ "Xamarin.Forms.CheckBox", CheckBox.IsCheckedProperty },
			{ "Xamarin.Forms.ClickGestureRecognizer", ClickGestureRecognizer.CommandProperty },
			{ "Xamarin.Forms.CollectionView", CollectionView.ItemsSourceProperty },
			{ "Xamarin.Forms.ContentPage", Page.TitleProperty },
			{ "Xamarin.Forms.DatePicker", DatePicker.DateProperty },
			{ "Xamarin.Forms.Editor", Editor.TextProperty },
			{ "Xamarin.Forms.Entry", Entry.TextProperty },
			{ "Xamarin.Forms.EntryCell", EntryCell.TextProperty },
			{ "Xamarin.Forms.FileImageSource", FileImageSource.FileProperty },
			{ "Xamarin.Forms.HtmlWebViewSource", HtmlWebViewSource.HtmlProperty },
			{ "Xamarin.Forms.Image", Image.SourceProperty },
			{ "Xamarin.Forms.ImageButton", ImageButton.CommandProperty },
			{ "Xamarin.Forms.ImageCell", ImageCell.ImageSourceProperty },
			{ "Xamarin.Forms.ItemsView", ItemsView.ItemsSourceProperty },
			{ "Xamarin.Forms.Label", Label.TextProperty },
			{ "Xamarin.Forms.ListView", ListView.ItemsSourceProperty },
			{ "Xamarin.Forms.MasterDetailPage", Page.TitleProperty },
			{ "Xamarin.Forms.MenuItem", MenuItem.CommandProperty },
			{ "Xamarin.Forms.MultiPage", Page.TitleProperty },
			{ "Xamarin.Forms.NavigationPage", Page.TitleProperty },
			{ "Xamarin.Forms.Page", Page.TitleProperty },
			{ "Xamarin.Forms.Picker", Picker.SelectedIndexProperty },
			{ "Xamarin.Forms.ProgressBar", ProgressBar.ProgressProperty },
			{ "Xamarin.Forms.SearchBar", SearchBar.SearchCommandProperty },
			{ "Xamarin.Forms.SearchHandler", SearchHandler.CommandProperty },
			{ "Xamarin.Forms.Slider", Slider.ValueProperty },
			{ "Xamarin.Forms.Span", Span.TextProperty },
			{ "Xamarin.Forms.Stepper", Stepper.ValueProperty },
			{ "Xamarin.Forms.StreamImageSource", StreamImageSource.StreamProperty },
			{ "Xamarin.Forms.SwipeGestureRecognizer", SwipeGestureRecognizer.CommandProperty },
			{ "Xamarin.Forms.SwipeItem", SwipeItem.CommandProperty },
			{ "Xamarin.Forms.Switch", Switch.IsToggledProperty },
			{ "Xamarin.Forms.SwitchCell", SwitchCell.OnProperty },
			{ "Xamarin.Forms.TabbedPage", Page.TitleProperty },
			{ "Xamarin.Forms.TableRoot", TableRoot.TitleProperty },
			{ "Xamarin.Forms.TableSection", TableSection.TitleProperty },
			{ "Xamarin.Forms.TableSectionBase", TableSectionBase.TitleProperty },
			{ "Xamarin.Forms.TapGestureRecognizer", TapGestureRecognizer.CommandProperty },
			{ "Xamarin.Forms.TemplatedPage", Page.TitleProperty },
			{ "Xamarin.Forms.TextCell", TextCell.TextProperty },
			{ "Xamarin.Forms.TimePicker", TimePicker.TimeProperty },
			{ "Xamarin.Forms.ToolbarItem", ToolbarItem.CommandProperty },
			{ "Xamarin.Forms.UriImageSource", UriImageSource.UriProperty },
			{ "Xamarin.Forms.UrlWebViewSource", UrlWebViewSource.UrlProperty },
			{ "Xamarin.Forms.WebView", WebView.SourceProperty }
		};

		public static void Register(params BindableProperty[] properties)
		{
			foreach (var property in properties)
				bindableObjectTypeDefaultProperty.Add(property.DeclaringType.FullName, property);
		}

		internal static BindableProperty GetFor(BindableObject bindableObject)
		{
			var type = bindableObject.GetType();
			var defaultProperty = GetFor(type);
			if (defaultProperty == null)
				throw new NotImplementedException(
					"No default bindable property is defined for BindableObject type " + type.FullName +
					"\r\nEither specify a property when calling Bind() or register a default bindable property for this BindableObject type");
			return defaultProperty;
		}

		internal static BindableProperty GetFor(Type bindableObjectType)
		{
			BindableProperty defaultProperty = null;

			do
			{
				string bindableObjectTypeName = bindableObjectType.FullName;
				if (bindableObjectTypeDefaultProperty.TryGetValue(bindableObjectTypeName, out defaultProperty))
					break;
				if (bindableObjectTypeName.StartsWith("Xamarin.Forms.", StringComparison.Ordinal))
					break;

				bindableObjectType = bindableObjectType.GetTypeInfo().BaseType;

				if (bindableObjectType == null)
					break;
			} while (true);

			return defaultProperty;
		}
	}
}
