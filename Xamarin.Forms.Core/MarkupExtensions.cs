using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;

namespace Xamarin.Forms
{
	public static partial class MarkupExtensions
	{
		const string bindingContextPropertyName = ".";

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
		static BindableProperty GetDefaultProperty(Element view)
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
						"\r\nEither specify a property when calling Bind() or add a default property for this type " +
						"to the viewTypeDefaultProperty dictionary.");

				viewType = viewType.GetTypeInfo().BaseType;

				if (viewType == null)
					return null;
			} while (true);

			return defaultProperty;
		}

		public static void RegisterDefaultBindableProperties(params BindableProperty[] properties)
		{
			foreach (var property in properties)
				viewTypeDefaultProperty.Add(property.DeclaringType.FullName, property);
		}

		public static TView Bind<TView>(
			this TView view, 
			BindableProperty targetProperty, 
			string sourcePropertyName = bindingContextPropertyName, 
			BindingMode mode = BindingMode.Default, 
			IValueConverter converter = null, 
			object converterParameter = null, 
			string stringFormat = null, 
			object source = null
		) where TView : Element
		{
			if (source != null || converterParameter != null)
				view.SetBinding(targetProperty, new Binding(
					path: sourcePropertyName,
					mode: mode,
					converter: converter,
					converterParameter: converterParameter,
					stringFormat: stringFormat,
					source: source
				));
			else
				view.SetBinding(targetProperty, sourcePropertyName, mode, converter, stringFormat);
			return view;
		}

		public static TView Bind<TView, TSource, TDest>(
			this TView view, 
			BindableProperty targetProperty, 
			string sourcePropertyName = bindingContextPropertyName, 
			BindingMode mode = BindingMode.Default, 
			Func<TSource, TDest> convert = null, 
			Func<TDest, TSource> convertBack = null, 
			object converterParameter = null, 
			string stringFormat = null, 
			object source = null
		) where TView : Element
		{
			var converter = new FuncConverter<TSource, TDest>(convert, convertBack);
			if (source != null || converterParameter != null)
				view.SetBinding(targetProperty, new Binding(
					path: sourcePropertyName,
					mode: mode,
					converter: converter,
					converterParameter: converterParameter,
					stringFormat: stringFormat,
					source: source
				));
			else
				view.SetBinding(targetProperty, sourcePropertyName, mode, converter, stringFormat);
			return view;
		}

		public static TView Bind<TView>(
			this TView view, 
			string sourcePropertyName = bindingContextPropertyName, 
			BindingMode mode = BindingMode.Default, 
			IValueConverter converter = null, 
			object converterParameter = null, 
			string stringFormat = null, 
			object source = null
		) where TView : Element
		{
			view.Bind(
				targetProperty: GetDefaultProperty(view),
				sourcePropertyName: sourcePropertyName,
				mode: mode,
				converter: converter,
				converterParameter: converterParameter,
				stringFormat: stringFormat,
				source: source
			);
			return view;
		}

		public static TView Bind<TView, TSource, TDest>(
			this TView view, 
			string sourcePropertyName = bindingContextPropertyName, 
			BindingMode mode = BindingMode.Default, 
			Func<TSource, TDest> convert = null, 
			Func<TDest, TSource> convertBack = null, 
			object converterParameter = null, 
			string stringFormat = null, 
			object source = null
		) where TView : Element
		{
			var converter = new FuncConverter<TSource, TDest>(convert, convertBack);
			view.Bind(
				targetProperty: GetDefaultProperty(view),
				sourcePropertyName: sourcePropertyName,
				mode: mode,
				converter: converter,
				converterParameter: converterParameter,
				stringFormat: stringFormat,
				source: source
			);
			return view;
		}

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
				targetProperty = GetDefaultProperty(gestureRecognizer);

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

		public static TView Assign<TView, TAssignView>(this TView view, out TAssignView variable) 
			where TView : class, TAssignView
		{
			variable = view;
			return view;
		}

		public static TView Invoke<TView>(this TView view, Action<TView> action) where TView : Element
		{
			action?.Invoke(view);
			return view;
		}

		public static TView Row<TView>(this TView view, int row, int span = 1) where TView : View
		{
			if (row != 0)
				view.SetValue(Grid.RowProperty, row);
			if (span != 1)
				view.SetValue(Grid.RowSpanProperty, span);
			return view;
		}

		public static TView RowSpan<TView>(this TView view, int span) where TView : View
		{
			if (span != 1)
				view.SetValue(Grid.RowSpanProperty, span);
			return view;
		}

		public static TView Col<TView>(this TView view, int col, int span = 1) where TView : View
		{
			if (col != 0)
				view.SetValue(Grid.ColumnProperty, col);
			if (span != 1)
				view.SetValue(Grid.ColumnSpanProperty, span);
			return view;
		}

		public static TView ColSpan<TView>(this TView view, int span) where TView : View
		{
			if (span != 1)
				view.SetValue(Grid.ColumnSpanProperty, span);
			return view;
		}

		public static TView RowCol<TView>(this TView view, int row, int col, int rowSpan = 1, int colSpan = 1) 
			where TView : View
		{
			if (row != 0)
				view.SetValue(Grid.RowProperty, row);
			if (col != 0)
				view.SetValue(Grid.ColumnProperty, col);
			if (rowSpan != 1)
				view.SetValue(Grid.RowSpanProperty, rowSpan);
			if (colSpan != 1)
				view.SetValue(Grid.ColumnSpanProperty, colSpan);
			return view;
		}

		public static TView AlignSelf<TView>(this TView view, FlexAlignSelf value) where TView : View
		{
			FlexLayout.SetAlignSelf(view, value);
			return view;
		}

		public static TView Basis<TView>(this TView view, FlexBasis value) where TView : View
		{
			FlexLayout.SetBasis(view, value);
			return view;
		}

		public static TView Grow<TView>(this TView view, float value) where TView : View
		{
			FlexLayout.SetGrow(view, value);
			return view;
		}

		public static TView Menu<TView>(this TView view, Menu value) where TView : View
		{
			FlexLayout.SetMenu(view, value);
			return view;
		}

		public static TView Order<TView>(this TView view, int value) where TView : View
		{
			FlexLayout.SetOrder(view, value);
			return view;
		}

		public static TView Shrink<TView>(this TView view, float value) where TView : View
		{
			FlexLayout.SetShrink(view, value);
			return view;
		}


		#region Use enum for Row / Col for better readability + avoid manual renumbering
		public static TView Row<TView, TRow>(this TView view, TRow row) where TView : View where TRow : Enum
		{
			int rowIndex = row.ToInt();
			if (rowIndex != 0)
				view.SetValue(Grid.RowProperty, rowIndex);
			return view;
		}

		public static TView Row<TView, TRow>(this TView view, TRow first, TRow last) where TView : View where TRow : Enum
		{
			int rowIndex = first.ToInt();
			int span = last.ToInt() - rowIndex + 1;
			if (rowIndex != 0)
				view.SetValue(Grid.RowProperty, rowIndex);
			if (span != 1)
				view.SetValue(Grid.RowSpanProperty, span);
			return view;
		}

		public static TView Col<TView, TCol>(this TView view, TCol col) where TView : View where TCol : Enum
		{
			int colIndex = col.ToInt();
			if (colIndex != 0)
				view.SetValue(Grid.ColumnProperty, colIndex);
			return view;
		}

		public static TView Col<TView, TCol>(this TView view, TCol first, TCol last) where TView : View where TCol : Enum
		{
			int colIndex = first.ToInt();
			if (colIndex != 0)
				view.SetValue(Grid.ColumnProperty, colIndex);

			int span = last.ToInt() + 1 - colIndex;
			if (span != 1)
				view.SetValue(Grid.ColumnSpanProperty, span);

			return view;
		}

		public static TView RowCol<TView, TRow, TCol>(
			this TView view, 
			TRow? firstRow = null, 
			TCol? firstCol = null, 
			TRow? lastRow = null, 
			TCol? lastCol = null
		) where TView : View where TRow : struct, Enum where TCol : struct, Enum
		{
			int firstRowIndex = firstRow.ToInt();
			if (firstRowIndex != 0)
				view.SetValue(Grid.RowProperty, firstRowIndex);

			int firstColIndex = firstCol.ToInt();
			if (firstColIndex != 0)
				view.SetValue(Grid.ColumnProperty, firstColIndex);

			if (lastRow != null)
			{
				int lastRowIndex = lastRow.ToInt();
				int rowSpan = lastRowIndex + 1 - firstRowIndex;
				if (rowSpan != 1)
					view.SetValue(Grid.RowSpanProperty, rowSpan);
			}

			if (lastCol != null)
			{
				int lastColIndex = lastCol.ToInt();
				int colSpan = lastColIndex + 1 - firstColIndex;
				if (colSpan != 1)
					view.SetValue(Grid.ColumnSpanProperty, colSpan);
			}

			return view;
		}

		static int ToInt(this Enum enumValue) => Convert.ToInt32(enumValue, CultureInfo.InvariantCulture);

		public static class EnumsForGridRowsAndColumns
		{
			public static GridLength Auto => GridLength.Auto;
			public static GridLength Star => GridLength.Star;

			public static class Columns
			{
				public static ColumnDefinitionCollection Define<TEnum>(params (TEnum name, GridLength width)[] cols) where TEnum : Enum
				{
					var columnDefinitions = new ColumnDefinitionCollection();
					for (int i = 0; i < cols.Length; i++)
					{
						if (i != cols[i].name.ToInt())
							throw new ArgumentException(
								$"Value of column name { cols[i].name } is not { i }. " +
								"Columns must be defined with enum names whose values form the sequence 0,1,2,..."
							);
						columnDefinitions.Add(new ColumnDefinition { Width = cols[i].width });
					}
					return columnDefinitions;
				}
			}

			public static class Rows
			{
				public static RowDefinitionCollection Define<TEnum>(params (TEnum name, GridLength height)[] rows) where TEnum : Enum
				{
					var rowDefinitions = new RowDefinitionCollection();
					for (int i = 0; i < rows.Length; i++)
					{
						if (i != rows[i].name.ToInt())
							throw new ArgumentException(
								$"Value of row name { rows[i].name } is not { i }. " +
								"Rows must be defined with enum names whose values form the sequence 0,1,2,..."
							);
						rowDefinitions.Add(new RowDefinition { Height = rows[i].height });
					}
					return rowDefinitions;
				}
			}

			public static int All<TEnum>() where TEnum : Enum
			{
				var values = Enum.GetValues(typeof(TEnum));
				int span = (int)values.GetValue(values.Length - 1) + 1;
				return span;
			}

			public static int Last<TEnum>() where TEnum : Enum
			{
				var values = Enum.GetValues(typeof(TEnum));
				int last = (int)values.GetValue(values.Length - 1);
				return last;
			}
		}

		#endregion Use enum for Row / Col for better readability + avoid manual renumbering


		public static TView Left<TView>(this TView view) where TView : View
		{ view.HorizontalOptions = LayoutOptions.Start; return view; }
		public static TView CenterH<TView>(this TView view) where TView : View
		{ view.HorizontalOptions = LayoutOptions.Center; return view; }
		public static TView FillH<TView>(this TView view) where TView : View
		{ view.HorizontalOptions = LayoutOptions.Fill; return view; }
		public static TView Right<TView>(this TView view) where TView : View
		{ view.HorizontalOptions = LayoutOptions.End; return view; }

		public static TView LeftExpand<TView>(this TView view) where TView : View
		{ view.HorizontalOptions = LayoutOptions.StartAndExpand; return view; }
		public static TView CenterExpandH<TView>(this TView view) where TView : View
		{ view.HorizontalOptions = LayoutOptions.CenterAndExpand; return view; }
		public static TView FillExpandH<TView>(this TView view) where TView : View
		{ view.HorizontalOptions = LayoutOptions.FillAndExpand; return view; }
		public static TView RightExpand<TView>(this TView view) where TView : View
		{ view.HorizontalOptions = LayoutOptions.EndAndExpand; return view; }

		public static TView Top<TView>(this TView view) where TView : View
		{ view.VerticalOptions = LayoutOptions.Start; return view; }
		public static TView Bottom<TView>(this TView view) where TView : View
		{ view.VerticalOptions = LayoutOptions.End; return view; }
		public static TView CenterV<TView>(this TView view) where TView : View
		{ view.VerticalOptions = LayoutOptions.Center; return view; }
		public static TView FillV<TView>(this TView view) where TView : View
		{ view.VerticalOptions = LayoutOptions.Fill; return view; }

		public static TView TopExpand<TView>(this TView view) where TView : View
		{ view.VerticalOptions = LayoutOptions.StartAndExpand; return view; }
		public static TView BottomExpand<TView>(this TView view) where TView : View
		{ view.VerticalOptions = LayoutOptions.EndAndExpand; return view; }
		public static TView CenterExpandV<TView>(this TView view) where TView : View
		{ view.VerticalOptions = LayoutOptions.CenterAndExpand; return view; }
		public static TView FillExpandV<TView>(this TView view) where TView : View
		{ view.VerticalOptions = LayoutOptions.FillAndExpand; return view; }

		public static TView Center<TView>(this TView view) where TView : View 
			=> view.CenterH().CenterV();
		public static TView Fill<TView>(this TView view) where TView : View 
			=> view.FillH().FillV();
		public static TView CenterExpand<TView>(this TView view) where TView : View 
			=> view.CenterExpandH().CenterExpandV();
		public static TView FillExpand<TView>(this TView view) where TView : View 
			=> view.FillExpandH().FillExpandV();

		public static TView Margin<TView>(this TView view, Thickness margin) where TView : View
		{ view.Margin = margin; return view; }
		public static TView Margin<TView>(this TView view, double horizontal, double vertical) where TView : View
		{ view.Margin = new Thickness(horizontal, vertical); return view; }
		public static TView Margins<TView>(this TView view, double left = 0, double top = 0, double right = 0, double bottom = 0) where TView : View
		{ view.Margin = new Thickness(left, top, right, bottom); return view; }

		public static TLayout Padding<TLayout>(this TLayout layout, Thickness padding) where TLayout : Layout
		{ layout.Padding = padding; return layout; }
		public static TLayout Padding<TLayout>(this TLayout layout, double horizontalSize, double verticalSize) where TLayout : Layout
		{ layout.Padding = new Thickness(horizontalSize, verticalSize); return layout; }
		public static TLayout Paddings<TLayout>(this TLayout layout, double left = 0, double top = 0, double right = 0, double bottom = 0) where TLayout : Layout
		{ layout.Padding = new Thickness(left, top, right, bottom); return layout; }

		public static TLabel TextLeft<TLabel>(this TLabel label) where TLabel : Label
		{ label.HorizontalTextAlignment = TextAlignment.Start; return label; }
		public static TLabel TextCenterH<TLabel>(this TLabel label) where TLabel : Label
		{ label.HorizontalTextAlignment = TextAlignment.Center; return label; }
		public static TLabel TextRight<TLabel>(this TLabel label) where TLabel : Label
		{ label.HorizontalTextAlignment = TextAlignment.End; return label; }

		public static TLabel TextTop<TLabel>(this TLabel label) where TLabel : Label
		{ label.VerticalTextAlignment = TextAlignment.Start; return label; }
		public static TLabel TextCenterV<TLabel>(this TLabel label) where TLabel : Label
		{ label.VerticalTextAlignment = TextAlignment.Center; return label; }
		public static TLabel TextBottom<TLabel>(this TLabel label) where TLabel : Label
		{ label.VerticalTextAlignment = TextAlignment.End; return label; }

		public static TLabel TextCenter<TLabel>(this TLabel label) where TLabel : Label 
			=> label.TextCenterH().TextCenterV();

		public static TElement Height<TElement>(this TElement element, double request) where TElement : VisualElement
		{ element.HeightRequest = request; return element; }
		public static TElement Width<TElement>(this TElement element, double request) where TElement : VisualElement
		{ element.WidthRequest = request; return element; }
		public static TElement MinHeight<TElement>(this TElement element, double request) where TElement : VisualElement
		{ element.MinimumHeightRequest = request; return element; }
		public static TElement MinWidth<TElement>(this TElement element, double request) where TElement : VisualElement
		{ element.MinimumWidthRequest = request; return element; }

		public static TElement Size<TElement>(this TElement element, double widthRequest, double heightRequest) where TElement : VisualElement 
			=> element.Width(widthRequest).Height(heightRequest);
		public static TElement Size<TElement>(this TElement element, double sizeRequest) where TElement : VisualElement 
			=> element.Width(sizeRequest).Height(sizeRequest);
		public static TElement MinSize<TElement>(this TElement element, double widthRequest, double heightRequest) where TElement : VisualElement 
			=> element.MinWidth(widthRequest).MinHeight(heightRequest);
		public static TElement MinSize<TElement>(this TElement element, double sizeRequest) where TElement : VisualElement 
			=> element.MinWidth(sizeRequest).MinHeight(sizeRequest);

		public static Label FontSize(this Label label, double fontSize) { label.FontSize = fontSize; return label; }
		public static Label Bold(this Label label) { label.FontAttributes = FontAttributes.Bold; return label; }
		public static Label Italic(this Label label) { label.FontAttributes = FontAttributes.Italic; return label; }

		public static TLabel FormattedText<TLabel>(this TLabel label, params Span[] spans) where TLabel : Label
		{
			label.FormattedText = new FormattedString();
			foreach (var span in spans) label.FormattedText.Spans.Add(span);
			return label;
		}

		public static TView Font<TView>(
			this TView view, 
			double? fontSize = null, 
			bool? bold = null, 
			bool? italic = null, 
			string family = null
		) where TView : View
		{
			var attributes = bold == true ? FontAttributes.Bold :
							 italic == true ? FontAttributes.Italic :
							 bold.HasValue || italic.HasValue ? FontAttributes.None :
							 (FontAttributes?)null;

			switch (view)
			{
				case Button button:
					if (fontSize.HasValue)
						button.FontSize = fontSize.Value;
					if (attributes.HasValue)
						button.FontAttributes = attributes.Value;
					if (family != null)
						button.FontFamily = family;
					break;
				case Label label:
					if (fontSize.HasValue)
						label.FontSize = fontSize.Value;
					if (attributes.HasValue)
						label.FontAttributes = attributes.Value;
					if (family != null)
						label.FontFamily = family;
					break;
				case Entry entry:
					if (fontSize.HasValue)
						entry.FontSize = fontSize.Value;
					if (attributes.HasValue)
						entry.FontAttributes = attributes.Value;
					if (family != null)
						entry.FontFamily = family;
					break;
				case Picker picker:
					if (fontSize.HasValue)
						picker.FontSize = fontSize.Value;
					if (attributes.HasValue)
						picker.FontAttributes = attributes.Value;
					if (family != null)
						picker.FontFamily = family;
					break;
			}
			return view;
		}

		static FontAttributes GetFontAttributes(bool bold = false, bool italic = false) 
			=> bold ? FontAttributes.Bold : italic ? FontAttributes.Italic : FontAttributes.None;

		public static T Style<T>(this T view, Style<T> style) where T : VisualElement
		{ view.Style = style; return view; }

		public static TElement Effects<TElement>(this TElement element, params Effect[] effects) where TElement : Element
		{
			for (int i = 0; i < effects.Length; i++)
				element.Effects.Add(effects[i]);
			return element;
		}

		#region Platform-specifics

		public static T iOSSetDefaultBackgroundColor<T>(this T cell, Color color) where T : Cell
		{
			Xamarin.Forms.PlatformConfiguration.iOSSpecific.Cell.SetDefaultBackgroundColor(cell, color);
			return cell;
		}

		public static T iOSSetGroupHeaderStyleGrouped<T>(this T listView) where T : ListView
		{
			Xamarin.Forms.PlatformConfiguration.iOSSpecific.ListView.SetGroupHeaderStyle(
				listView, 
				Xamarin.Forms.PlatformConfiguration.iOSSpecific.GroupHeaderStyle.Grouped
			);
			return listView;
		}

		#endregion Platform-specifics
	}

	public class FuncConverter<TSource, TDest> : IValueConverter
	{
		readonly Func<TSource, TDest> convert;
		readonly Func<TDest, TSource> convertBack;

		public FuncConverter(Func<TSource, TDest> convert = null, Func<TDest, TSource> convertBack = null)
		{ this.convert = convert; this.convertBack = convertBack; }

		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) 
			=> convert != null ? convert.Invoke(value != null ? (TSource)value : default(TSource)) : default(TDest);

		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) 
			=> convertBack != null ? convertBack.Invoke(value != null ? (TDest)value : default(TDest)) : default(TSource);
	}

	public class FuncConverter<TSource> : FuncConverter<TSource, object>
	{
		public FuncConverter(Func<TSource, object> convert = null, Func<object, TSource> convertBack = null) 
			: base(convert, convertBack) { }
	}

	public class FuncConverter : FuncConverter<object, object>
	{
		public FuncConverter(Func<object, object> convert = null, Func<object, object> convertBack = null) 
			: base(convert, convertBack) { }
	}

	public class ToStringConverter : FuncConverter {
		public ToStringConverter(string format = "{0}") 
			: base(o => string.Format(CultureInfo.InvariantCulture, format, o)) { }
	}

	public class BoolNotConverter : FuncConverter<bool>
	{
		static readonly Lazy<BoolNotConverter> instance = new Lazy<BoolNotConverter>(() => new BoolNotConverter());
		public static BoolNotConverter Instance => instance.Value;
		public BoolNotConverter() : base(t => !t) { }
	}

	public class Style<T> where T : Element // Should be IStyleElement but can't use that because it's internal
	{
		public static implicit operator Style(Style<T> style) => style?.FormsStyle;

		public Style FormsStyle { get; }

		public Style(params (BindableProperty Property, object Value)[] setters)
		{
			FormsStyle = new Style(typeof(T)) { };
			Add(setters);
		}

		public Style<T> ApplyToDerivedTypes(bool value)
		{
			FormsStyle.ApplyToDerivedTypes = value;
			return this;
		}

		public Style<T> BasedOn(Style value)
		{
			FormsStyle.BasedOn = value;
			return this;
		}

		public Style<T> Add(params (BindableProperty Property, object Value)[] setters)
		{
			foreach (var setter in setters)
				FormsStyle.Setters.Add(setter.Property, setter.Value);
			return this;
		}

		public Style<T> Add(params Behavior[] behaviors)
		{
			foreach (var behavior in behaviors)
				FormsStyle.Behaviors.Add(behavior);
			return this;
		}

		public Style<T> Add(params TriggerBase[] triggers)
		{
			foreach (var trigger in triggers)
				FormsStyle.Triggers.Add(trigger);
			return this;
		}

		public Style<T> CanCascade(bool value)
		{
			FormsStyle.CanCascade = value;
			return this;
		}
	}
}