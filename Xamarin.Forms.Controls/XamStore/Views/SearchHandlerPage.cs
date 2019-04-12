using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Xamarin.Forms;
using Xamarin.Forms.Internals;
using Xamarin.Forms.PlatformConfiguration;
using Xamarin.Forms.PlatformConfiguration.iOSSpecific;
using static Xamarin.Forms.Controls.XamStore.BasePage;

namespace Xamarin.Forms.Controls.XamStore
{
	[Preserve(AllMembers = true)]
	public class SearchHandlerPage : ContentPage
	{
		HashSet<string> _exceptProperties = new HashSet<string>
		{
			AutomationIdProperty.PropertyName,
			ClassIdProperty.PropertyName,
			"StyleId",
		};
		CustomSearchHandler _searchHandler;
		StackLayout _propertyLayout;
		public SearchHandlerPage()
		{
			On<iOS>().SetUseSafeArea(true);
			_searchHandler = AddSearchHandler();
			_propertyLayout = new StackLayout
			{
				Spacing = 10,
				Padding = 10
			};
			Content = new StackLayout
			{
				Children = {
					new Label { Text = "Test SearchHandler", HorizontalTextAlignment = TextAlignment.Center },
					new ScrollView { Content = new StackLayout {
														Children = {
																	new Button { Text = "Show/Hide SearchHandler", Command = new Command(()=> ShowHideSearchHandler()) },
																	new Button { Text = "Focus/Unfocus SearchHandler", Command = new Command(()=> FocusUnfocusSearchHandler()) },

																	_propertyLayout
																	}
															}
									}
				}
			};


			var publicProperties = typeof(SearchHandler)
				.GetProperties(BindingFlags.Public | BindingFlags.Instance)
				.Where(p => p.CanRead && p.CanWrite && !_exceptProperties.Contains(p.Name));

			foreach (var property in publicProperties)
			{
				if (property.PropertyType == typeof(Color))
				{
					var colorPicker = new ColorPicker
					{
						Title = property.Name,
						Color = (Color)property.GetValue(_searchHandler)
					};
					colorPicker.ColorPicked += (_, e) => property.SetValue(_searchHandler, e.Color);
					_propertyLayout.Children.Add(colorPicker);
				}
				if (property.PropertyType == typeof(TextAlignment))
				{
					var alignmentPicker = new Picker
					{
						Title = property.Name,
						Items = { TextAlignment.Center.ToString(), TextAlignment.End.ToString(), TextAlignment.Start.ToString() }
					};
					alignmentPicker.SelectedIndexChanged += (_, e) => property.SetValue(_searchHandler, Enum.Parse(typeof(TextAlignment), alignmentPicker.SelectedItem.ToString()));
					_propertyLayout.Children.Add(alignmentPicker);
				}
			}
		}

		void FocusUnfocusSearchHandler()
		{
			if (_searchHandler == null)
				return;

			if (_searchHandler.IsFocused)
				_searchHandler.Unfocus();
			else
				_searchHandler.Focus();
		}

		void ShowHideSearchHandler()
		{
			if (_searchHandler == null)
			{
				_searchHandler = AddSearchHandler();
			}
			else
			{
				_searchHandler = null;

				Shell.SetSearchHandler(this, _searchHandler);
			}
		}

		internal CustomSearchHandler AddSearchHandler()
		{
			var searchHandler = new CustomSearchHandler();

			searchHandler.BackgroundColor = Color.Orange;
			searchHandler.CancelButtonColor = Color.Pink;
			searchHandler.TextColor = Color.White;
			searchHandler.PlaceholderColor = Color.Yellow;
			searchHandler.HorizontalTextAlignment = TextAlignment.Center;
			searchHandler.ShowsResults = true;

			searchHandler.Keyboard = Keyboard.Numeric;

			searchHandler.FontFamily = "ChalkboardSE-Regular";
			searchHandler.FontAttributes = FontAttributes.Bold;

			searchHandler.ClearIconName = "Clear";
			searchHandler.ClearIconHelpText = "Clears the search field text";

			searchHandler.ClearPlaceholderName = "Voice Search";
			searchHandler.ClearPlaceholderHelpText = "Start voice search";

			searchHandler.QueryIconName = "Search";
			searchHandler.QueryIconHelpText = "Press to search app";

			searchHandler.Placeholder = "Type to search";
			searchHandler.ClearPlaceholderEnabled = true;
			searchHandler.ClearPlaceholderIcon = "mic.png";

			Shell.SetSearchHandler(this, searchHandler);
			return searchHandler;
		}

	}
}

