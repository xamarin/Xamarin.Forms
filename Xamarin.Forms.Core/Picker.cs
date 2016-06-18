using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using Xamarin.Forms.Platform;

namespace Xamarin.Forms
{
	[RenderWith(typeof(_PickerRenderer))]
	public class Picker : View
	{
		public static readonly BindableProperty TextColorProperty = BindableProperty.Create(nameof(TextColor), typeof(Color), typeof(Picker), Color.Default);

		public static readonly BindableProperty TitleProperty = BindableProperty.Create(nameof(Title), typeof(string), typeof(Picker), default(string));

		public static readonly BindableProperty SelectedIndexProperty = BindableProperty.Create(nameof(SelectedIndex), typeof(int), typeof(Picker), -1, BindingMode.TwoWay,
			propertyChanged: (bindable, oldvalue, newvalue) =>
			{
				EventHandler eh = ((Picker)bindable).SelectedIndexChanged;
				if (eh != null)
					eh(bindable, EventArgs.Empty);
			}, coerceValue: CoerceSelectedIndex);
		public static readonly BindableProperty ItemsSourceProperty = BindableProperty.Create (nameof (ItemsSource), typeof (IEnumerable), typeof (Picker), null, propertyChanged: OnItemsSourcePropertyChanged);

		public static readonly BindableProperty SelectedItemProperty = BindableProperty.Create (nameof (SelectedItem), typeof (object), typeof (Picker), null, BindingMode.TwoWay, propertyChanged: OnSelectedItemPropertyChanged);

		public Picker()
		{
			Items = new ObservableList<string>();
			((ObservableList<string>)Items).CollectionChanged += OnItemsCollectionChanged;
			SelectedIndexChanged += (o, e) =>
			{
				if (ItemsSource != null && SelectedIndex >= 0)
					SelectedItem = ItemsSource.Cast<object> ().ToList () [SelectedIndex];
			};
		}

		public IList<string> Items { get; }

		public int SelectedIndex
		{
			get { return (int)GetValue(SelectedIndexProperty); }
			set { SetValue(SelectedIndexProperty, value); }
		}

		public Color TextColor
		{
			get { return (Color)GetValue(TextColorProperty); }
			set { SetValue(TextColorProperty, value); }
		}

		public string Title
		{
			get { return (string)GetValue(TitleProperty); }
			set { SetValue(TitleProperty, value); }
		}

		public IEnumerable<object> ItemsSource {
			get { return (IEnumerable<object>)GetValue (ItemsSourceProperty); }
			set { SetValue (ItemsSourceProperty, value); }
		}

		public object SelectedItem {
			get { return GetValue (SelectedItemProperty); }
			set { SetValue (SelectedItemProperty, value); }
		}

		public event EventHandler SelectedIndexChanged;

		static object CoerceSelectedIndex(BindableObject bindable, object value)
		{
			var picker = (Picker)bindable;
			return picker.Items == null ? -1 : ((int)value).Clamp(-1, picker.Items.Count - 1);
		}

		void OnItemsCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
		{
			SelectedIndex = SelectedIndex.Clamp(-1, Items.Count - 1);
		}

		static void OnItemsSourcePropertyChanged (BindableObject bindable, object value, object newValue)
		{
			var picker = (Picker)bindable;
			var notifyCollection = newValue as INotifyCollectionChanged;
			if (notifyCollection != null) {
				notifyCollection.CollectionChanged += (sender, args) => {
					if (args.NewItems != null)
						foreach (var newItem in args.NewItems)
							picker.Items.Add ((newItem ?? "").ToString ());

					if (args.OldItems != null)
						foreach (var oldItem in args.OldItems)
							picker.Items.Remove ((oldItem ?? "").ToString ());
				};
			}

			var enumerable = newValue as IEnumerable;
			if (enumerable == null)
				return;

			picker.Items.Clear ();

			foreach (var item in enumerable)
				picker.Items.Add ((item ?? "").ToString ());
		}

		static void OnSelectedItemPropertyChanged (BindableObject bindable, object value, object newValue)
		{
			var picker = (Picker)bindable;
			if (picker.ItemsSource != null)
				picker.SelectedIndex = picker.ItemsSource.IndexOf(picker.SelectedItem);
		}

	}
}