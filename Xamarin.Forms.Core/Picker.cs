using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Reflection;
using Xamarin.Forms.Platform;

namespace Xamarin.Forms
{
    [RenderWith(typeof(_PickerRenderer))]
    public class Picker : View
    {
        public static readonly BindableProperty TextColorProperty =
            BindableProperty.Create(
                nameof(TextColor),
                typeof(Color),
                typeof(Picker),
                Color.Default);

        public static readonly BindableProperty TitleProperty =
            BindableProperty.Create(
                nameof(Title),
                typeof(string),
                typeof(Picker),
                default(string));

        public static readonly BindableProperty SelectedIndexProperty =
            BindableProperty.Create(
                nameof(SelectedIndex),
                typeof(int),
                typeof(Picker),
                -1,
                BindingMode.TwoWay,
                propertyChanged: OnSelectedIndexChanged,
                coerceValue: CoerceSelectedIndex);

        public static readonly BindableProperty SelectedValueMemberPathProperty =
            BindableProperty.Create(
                nameof(SelectedValueMemberPath),
                typeof(string),
                typeof(Picker));

        public static readonly BindableProperty ItemsSourceProperty =
            BindableProperty.Create(
                nameof(ItemsSource),
                typeof(IList),
                typeof(Picker),
                default(IList),
                propertyChanged: OnItemsSourceChanged);

        public static readonly BindableProperty DisplayMemberPathProperty =
            BindableProperty.Create(
                nameof(DisplayMemberPath),
                typeof(string),
                typeof(Picker),
                propertyChanged: OnDisplayMemberPathChanged);

        public static readonly BindableProperty SelectedItemProperty =
            BindableProperty.Create(
                nameof(SelectedItem),
                typeof(object),
                typeof(Picker),
                null,
                BindingMode.TwoWay,
                propertyChanged: OnSelectedItemChanged);

        public Picker()
        {
            ((ObservableList<string>)Items).CollectionChanged += OnItemsCollectionChanged;
        }

        /// <summary>
        /// Set the name of the property that should be used to display the objects in <see cref="ItemsSource" />.
        /// If left blank the <see cref="object.ToString" /> will be called
        /// </summary>
        /// <remarks>
        /// Setting a value that does not exists on the object cause exception
        /// </remarks>
        /// <para>
        /// Settings this value only affect objects that are not primitive types.
        /// </para>
        /// <exception cref="ArgumentException">Setting a value that does not exists on the object cause exception</exception>
        public string DisplayMemberPath
        {
            get { return (string)GetValue(DisplayMemberPathProperty); }
            set { SetValue(DisplayMemberPathProperty, value); }
        }

        public IList<string> Items { get; } = new ObservableList<string>();

        public IList ItemsSource
        {
            get { return (IList)GetValue(ItemsSourceProperty); }
            set { SetValue(ItemsSourceProperty, value); }
        }

        public int SelectedIndex
        {
            get { return (int)GetValue(SelectedIndexProperty); }
            set { SetValue(SelectedIndexProperty, value); }
        }

        public object SelectedItem
        {
            get { return GetValue(SelectedItemProperty); }
            set { SetValue(SelectedItemProperty, value); }
        }

        public string SelectedValueMemberPath
        {
            get { return (string)GetValue(SelectedValueMemberPathProperty); }
            set { SetValue(SelectedValueMemberPathProperty, value); }
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

        public event EventHandler SelectedIndexChanged;

        /// <summary>
        /// Get the value to display through reflection on <paramref name="item" /> using property <see cref="DisplayMemberPath" />
        /// </summary>
        /// <param name="item">Item to get value from.</param>
        /// <returns>Value of the property <see cref="DisplayMemberPath" /> if not <c>null</c>; otherwise ToString()</returns>
        /// <exception cref="ArgumentException">If no property with name <see cref="DisplayMemberPath" /> is found</exception>
        protected virtual string GetDisplayMember(object item)
        {
            return GetPropertyValue(item, DisplayMemberPath) as string;
        }

        protected virtual object GetSelectedValue(object item)
        {
            return GetPropertyValue(item, SelectedValueMemberPath);
        }

        void AddItems(NotifyCollectionChangedEventArgs e)
        {
            int index = e.NewStartingIndex < 0 ? Items.Count : e.NewStartingIndex;
            foreach (object newItem in e.NewItems)
            {
                Items.Insert(index++, GetDisplayMember(newItem));
            }
        }

        void BindItems()
        {
            Items.Clear();
            foreach (object item in ItemsSource)
            {
                Items.Add(GetDisplayMember(item));
            }
            UpdateSelectedItem();
        }

        static object CoerceSelectedIndex(BindableObject bindable, object value)
        {
            var picker = (Picker)bindable;
            return picker.Items == null ? -1 : ((int)value).Clamp(-1, picker.Items.Count - 1);
        }

        void CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Reset:
                    BindItems();
                    return;
                case NotifyCollectionChangedAction.Remove:
                    RemoveItems(e);
                    break;
                case NotifyCollectionChangedAction.Add:
                    AddItems(e);
                    break;
                case NotifyCollectionChangedAction.Move:
                    // TODO Make more intelligent decision
                    BindItems();
                    break;
                case NotifyCollectionChangedAction.Replace:
                    // TODO Make more intelligent decision
                    BindItems();
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        static object GetPropertyValue(object item, string memberPath)
        {
            if (item == null)
            {
                return null;
            }
            // TODO How to handle Nullable types
            if (IsPrimitive(item) || string.IsNullOrEmpty(memberPath))
            {
                return item.ToString();
            }
            // Find the property by walking the display member path to find any nested properties
            string[] propertyPathParts = memberPath.Split('.');
            object propertyValue = null;
            foreach (string propertyPathPart in propertyPathParts)
            {
                PropertyInfo propInfo = item.GetType().GetTypeInfo().GetDeclaredProperty(propertyPathPart);
                propertyValue = propInfo.GetValue(item);
            }

            if (propertyValue == null)
            {
                throw new ArgumentException(
                    $"No property with name '{memberPath}' was found on '{item.GetType().FullName}'");
            }
            return propertyValue;
        }

        static bool IsPrimitive(object item)
        {
            // TODO What is missing
            return item is string || item is int || item is double || item is decimal || item is Enum ||
                   item is DateTime;
        }

        static void OnDisplayMemberPathChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var picker = (Picker)bindable;
            if (picker.ItemsSource?.Count > 0)
            {
                picker.BindItems();
            }
        }

        void OnItemsCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Remove || e.Action == NotifyCollectionChangedAction.Reset)
            {
                SelectedIndex = SelectedIndex.Clamp(-1, Items.Count - 1);
                UpdateSelectedItem();
            }
        }

        static void OnItemsSourceChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var picker = (Picker)bindable;
            // Check if the ItemsSource value has changed and if so, unsubscribe from collection changed
            var observable = oldValue as INotifyCollectionChanged;
            if (observable != null)
            {
                observable.CollectionChanged -= picker.CollectionChanged;
            }
            observable = newValue as INotifyCollectionChanged;
            if (observable != null)
            {
                observable.CollectionChanged += picker.CollectionChanged;
                picker.BindItems();
            }
            else
            {
                // newValue is null so clear the items collection
                picker.Items.Clear();
            }
        }

        static void OnSelectedIndexChanged(object bindable, object oldValue, object newValue)
        {
            var picker = (Picker)bindable;
            EventHandler eh = picker.SelectedIndexChanged;
            eh?.Invoke(bindable, EventArgs.Empty);
            picker.UpdateSelectedItem();
        }

        static void OnSelectedItemChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var picker = (Picker)bindable;
            picker.UpdateSelectedIndex(newValue);
        }

        void RemoveItems(NotifyCollectionChangedEventArgs e)
        {
            int index = e.OldStartingIndex < Items.Count ? e.OldStartingIndex : Items.Count;
            // TODO: How do we determine the order of which the items were removed
            foreach (object _ in e.OldItems)
            {
                Items.RemoveAt(index--);
            }
        }

        void UpdateSelectedIndex(object selectedItem)
        {
            string displayMember = GetDisplayMember(selectedItem);
            int index = Items.IndexOf(displayMember);
            // TODO Should we prevent call to FindObject since the object is already known
            // by setting a flag, or otherwise indicate, that we, internally, forced a SelectedIndex changed
            SelectedIndex = index;
        }

        void UpdateSelectedItem()
        {
            if (SelectedIndex < 0 || SelectedIndex > Items.Count - 1)
            {
                SelectedItem = null;
            }
            else
            {
                SelectedItem = ItemsSource?[SelectedIndex];
            }
        }
    }
}