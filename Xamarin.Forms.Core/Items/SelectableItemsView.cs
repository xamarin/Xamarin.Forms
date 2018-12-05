using System;
using System.Windows.Input;

namespace Xamarin.Forms
{
	public class SelectableItemsView : ItemsView
	{
		public static readonly BindableProperty SelectionModeProperty =
			BindableProperty.Create(nameof(SelectionMode), typeof(SelectionMode), typeof(SelectableItemsView),
				SelectionMode.None);

		public static readonly BindableProperty SelectedItemProperty =
			BindableProperty.Create(nameof(SelectedItem), typeof(object), typeof(SelectableItemsView),
				propertyChanged: SelectedItemPropertyChanged);

		public static readonly BindableProperty SelectionChangedCommandProperty =
			BindableProperty.Create(nameof(SelectionChangedCommand), typeof(ICommand), typeof(SelectableItemsView));

		public static readonly BindableProperty SelectionChangedCommandParameterProperty =
			BindableProperty.Create(nameof(SelectionChangedCommandParameter), typeof(object),
				typeof(SelectableItemsView));

		public object SelectedItem
		{
			get => GetValue(SelectedItemProperty);
			set => SetValue(SelectedItemProperty, value);
		}

		public ICommand SelectionChangedCommand
		{
			get => (ICommand)GetValue(SelectionChangedCommandProperty);
			set => SetValue(SelectionChangedCommandProperty, value);
		}

		public object SelectionChangedCommandParameter
		{
			get => GetValue(SelectionChangedCommandParameterProperty);
			set => SetValue(SelectionChangedCommandParameterProperty, value);
		}

		public SelectionMode SelectionMode
		{
			get => (SelectionMode)GetValue(SelectionModeProperty);
			set => SetValue(SelectionModeProperty, value);
		}

		public event EventHandler<SelectionChangedEventArgs> SelectionChanged;

		protected virtual void OnSelectionChanged(SelectionChangedEventArgs args)
		{
			var command = SelectionChangedCommand;

			if (command != null)
			{
				var commandParameter = SelectionChangedCommandParameter;

				if (command.CanExecute(commandParameter))
				{
					command.Execute(commandParameter);
				}
			}
			
			SelectionChanged?.Invoke(this, args);
		}

		static void SelectedItemPropertyChanged(BindableObject bindable, object oldValue, object newValue)
		{
			var args = new SelectionChangedEventArgs(oldValue, newValue);
			((SelectableItemsView)bindable).OnSelectionChanged(args);
		}
	}
}