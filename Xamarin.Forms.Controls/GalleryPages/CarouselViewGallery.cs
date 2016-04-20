using System;
using System.Linq;
using Xamarin.Forms.CustomAttributes;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Collections.ObjectModel;

#if UITEST
using Xamarin.UITest;
using NUnit.Framework;
#endif

namespace Xamarin.Forms.Controls
{
	[Preserve(AllMembers = true)]
	public sealed class CarouselViewGallaryLaunchPage : ContentPage
	{
		public CarouselViewGallaryLaunchPage()
		{
			var button = new Button {
				Text = "Launch",
				AutomationId = "Launch"
			};

			button.Clicked += (s, e) => this.Navigation.PushAsync(new CarouselViewGallaryPage());

			Content = new StackLayout {
				Children = {
					button
				}
			};
		}
	}

	[Preserve(AllMembers = true)]
	public sealed class CarouselViewGallaryPage : ContentPage
	{
		public abstract class Item
		{
			static int s_id = 0;

			int id;

			internal Item()
			{
				id = s_id++;
			}

			public int Id => id;
			public string TypeName => GetType().Name;
		}
		public sealed class Foo : Item { }
		public sealed class Bar : Item { }
		public sealed class Baz : Item { }
		public sealed class Poo : Item { }
		public sealed class Moo : Item { }

		[Preserve(AllMembers = true)]
		public sealed class ItemView : ContentView
		{
			public static readonly BindableProperty TextColorProperty = BindableProperty.Create(
				propertyName: nameof(TextColor),
				returnType: typeof(Color),
				declaringType: typeof(ItemView),
				defaultValue: Color.White,
				defaultBindingMode: BindingMode.TwoWay
			);
			public static readonly BindableProperty ContextProperty = BindableProperty.Create(
				propertyName: nameof(Context),
				returnType: typeof(CarouselView),
				declaringType: typeof(ItemView),
				defaultBindingMode: BindingMode.TwoWay
			);

			public ItemView()
			{
				var change = CreateButton("Change", "Change", (items, index) => items[index] = new Moo());

				var removeBar = new StackLayout
				{
					Orientation = StackOrientation.Horizontal,
					HorizontalOptions = LayoutOptions.FillAndExpand,
					Children = {
						CreateButton ("- Left", "RemoveLeft", (items, index) => items.RemoveAt (index - 1)),
						CreateButton ("Remove", "Remove", (items, index) => items.RemoveAt (index)),
						CreateButton ("- Right", "RemoveRight", (items, index) => items.RemoveAt (index + 1)),
					}
				};

				var addBar = new StackLayout
				{
					Orientation = StackOrientation.Horizontal,
					HorizontalOptions = LayoutOptions.FillAndExpand,
					Children = {
						CreateButton ("+ Left", "AddLeft", (items, index) => items.Insert (index, new Moo ())),
						CreateButton ("+ Right", "AddRight", (items, index) => {
							if (index == items.Count - 1)
								items.Add (new Moo ());
							else
								items.Insert (index + 1, new Moo ());
						}),
					}
				};

				var typeNameLabel = new Label() { StyleId = "typename" };
				typeNameLabel.SetBinding(Label.TextProperty, nameof(Item.TypeName));

				var idLabel = new Label()
				{
					AutomationId = "ItemId",
					StyleId = "id",
					TextColor = Color.White
				};
				idLabel.SetBinding(Label.TextProperty, nameof(Item.Id));

				Content = new StackLayout
				{
					Children = {
						typeNameLabel,
						idLabel,
						change,
						removeBar,
						addBar,
					}
				};

				PropertyChanged += (s, e) =>
				{
					if (e.PropertyName == "TextColor")
						typeNameLabel.TextColor = TextColor;
				};
			}

			Button CreateButton(string text, string automationId, Action<IList<Item>, int> clicked)
			{
				var button = new Button();
				button.AutomationId = automationId;
				button.Text = text;
				button.TextColor = Color.White;
				button.Clicked += (s, e) =>
				{
					var items = (IList<Item>)Context.ItemsSource;
					var index = items.IndexOf(BindingContext);
					clicked(items, index);
				};
				return button;
			}

			public CarouselView Context
			{
				get
				{
					return (CarouselView)GetValue(ContextProperty);
				}
				set
				{
					SetValue(ContextProperty, value);
				}
			}
			public Color TextColor
			{
				get
				{
					return (Color)GetValue(TextColorProperty);
				}
				set
				{
					SetValue(TextColorProperty, value);
				}
			}
		}
		public sealed class MyDataTemplateSelector : DataTemplateSelector
		{
			Dictionary<Type, Color> m_colorByType = new Dictionary<Type, Color>();
			Dictionary<Type, DataTemplate> m_dataTemplateByType = new Dictionary<Type, DataTemplate>();

			public MyDataTemplateSelector()
			{
				m_colorByType[typeof(Foo)] = Color.Green;
				m_colorByType[typeof(Bar)] = Color.Red;
			}

			protected override DataTemplate OnSelectTemplate(object item, BindableObject container)
			{
				return OnSelectTemplate(item.GetType(), container);
			}

			DataTemplate OnSelectTemplate(Type itemType, BindableObject container)
			{
				DataTemplate dataTemplate;
				if (!m_dataTemplateByType.TryGetValue(itemType, out dataTemplate))
					m_dataTemplateByType[itemType] = dataTemplate = CreateTemplate(itemType, container);
				return dataTemplate;
			}

			DataTemplate CreateTemplate(Type itemType, BindableObject container)
			{
				var dataTemplate = new DataTemplate(typeof(ItemView));

				Color color;
				if (!m_colorByType.TryGetValue(itemType, out color))
				{
					color = Color.Pink;
					dataTemplate.SetValue(BackgroundColorProperty, Color.Black);
				}
				else
				{
					dataTemplate.SetValue(BackgroundColorProperty, Color.Blue);
				}

				dataTemplate.SetValue(ItemView.TextColorProperty, color);
				dataTemplate.SetValue(ItemView.ContextProperty, container);
				return dataTemplate;
			}
		}

		static Button CreateButton(string text, string automationId, Action onClicked = null)
		{
			var button = new Button 
			{
				Text = text, 
				AutomationId = automationId,
				WidthRequest = 50
			};

			if (onClicked != null)
				button.Clicked += (s, e) => onClicked();

			return button;
		}
		static Label CreateValue(string text, string automationId = "") => 
			CreateLabel(text, Color.White, automationId);
		static Label CreateCopy(string text, string automationId = "") =>
			CreateLabel(text, Color.Olive, automationId);
		static Label CreateLabel(string text, Color color, string automationId)
		{
			return new Label()
			{
				TextColor = color,
				Text = text,
				AutomationId = automationId
			};
		}

		const int StartPosition = 1;
		const int EventQueueLength = 7;
		static readonly ObservableCollection<Item> EmptyItems = new ObservableCollection<Item>();

		readonly CarouselView _carouselView;
		readonly MyDataTemplateSelector _selector;
		readonly Label _position;
		readonly Label _selectedItem;
		readonly Label _selectedPosition;
		readonly Queue<string> _events;
		readonly Label _eventLog;
		int _eventId;
		ObservableCollection<Item> _items;

		void OnEvent(string name)
		{
			_events.Enqueue($"{name}/{_eventId++}");

			if (_events.Count == EventQueueLength)
				_events.Dequeue();
			_eventLog.Text = string.Join(", ", _events.ToArray().Reverse());

			UpdatePosition();
		}

		void UpdatePosition()
		{
			_position.Text = $"{_carouselView.Position}";
		}

		public CarouselViewGallaryPage()
		{
			_items = EmptyItems;

			_selector = new MyDataTemplateSelector ();
			Func<ObservableCollection<Item>> itemsFactory = () => 
				_items = new ObservableCollection<Item>()
				{
					new Baz(),
					new Poo(),
					new Foo(),
					new Bar(),
				};

			_carouselView = new CarouselView
			{
				BackgroundColor = Color.Purple,
				ItemTemplate = _selector,
				Position = StartPosition
				//,ItemsSource = _items = new ObservableCollection<Item>()
				//{
				//	new Baz(),
				//	new Poo(),
				//	new Foo(),
				//	new Bar(),
				//}
			};

			_events = new Queue<string>();
			_eventId = 0;
			_position = CreateValue($"{_carouselView.Position}", "Position");
			_selectedItem = CreateValue("null", "SelectedItem");
			_selectedPosition = CreateValue("null", "SelectedPosition");
			_eventLog = CreateValue("", "EventLog");

			_carouselView.ItemSelected += (s, o) =>
			{
				var selectedItem = (Item)o.SelectedItem;
				var selectedItemId = selectedItem?.Id.ToString() ?? "null";
				if (selectedItem != _carouselView.Item)
					throw new Exception("CarouselView.Item != ItemSelected");
				_selectedItem.Text = $"{selectedItemId}";
				OnEvent("i");
			};

			_carouselView.PositionSelected += (s, o) =>
			{
				var selectedPosition = (int)o.SelectedPosition;
				if (_items != null && _items.Any() && _items[selectedPosition] != _carouselView.Item)
					throw new Exception("CarouselView.Item != Items[selectedPosition]");
				_selectedPosition.Text = $"{selectedPosition}";
				OnEvent("p");
			};

			BackgroundColor = Color.Blue;

			var moveBar = new StackLayout {
				Orientation = StackOrientation.Horizontal,
				HorizontalOptions = LayoutOptions.FillAndExpand,
				Children = {
					CreateButton ("<<", "First", () => _carouselView.Position = 0),
					CreateButton ("<", "Previous", () => {
						try {
							_carouselView.Position--;
							if (_items == EmptyItems)
								UpdatePosition();
						} catch (ArgumentException) { OnEvent("e"); }
					}),
					CreateButton (">", "Next", () => {
						try {
							_carouselView.Position++;
							if (_items == EmptyItems)
								UpdatePosition();
						} catch (ArgumentException) { OnEvent("e"); }
				}),
					CreateButton (">>", "Last", () => _carouselView.Position = _items.Count - 1)
				}
			};
			var loadBar = new StackLayout {
				Orientation = StackOrientation.Horizontal,
				HorizontalOptions = LayoutOptions.FillAndExpand,
				Children = {
					CreateButton("Load", "Load", () => {
						try {
							_carouselView.ItemsSource = _items = itemsFactory();
						} catch (ArgumentException) {
							OnEvent("e");
						}
					}),
					CreateButton("Load0", "Load0", () => {
						try {
							_carouselView.ItemsSource = _items = EmptyItems;
						} catch (ArgumentException) {
							OnEvent("e");
						}
					}),
					CreateButton("Clear", "Clear", () => {
						_items = null;
						_carouselView.ItemsSource = null;
					})
				}
			};

			var statusBar = new StackLayout 
			{
				Orientation = StackOrientation.Horizontal,
				Children = {
					CreateCopy("Pos:"), _position,
					CreateCopy("OnItemSel:"), _selectedItem,
					CreateCopy("OnPosSel:"), _selectedPosition,
				}
			};

			var logBar = new StackLayout
			{
				Orientation = StackOrientation.Horizontal,
				Children = { _eventLog }
			};

			Content = new StackLayout
			{
				Children = {
					_carouselView,
					moveBar,
					loadBar,
					statusBar,
					logBar
				}
			};
		}
	}
}
