using System;
using System.Windows.Input;

namespace Xamarin.Forms
{
	public class IndexableSelectableItemsView : SelectableItemsView
	{
		public static readonly BindableProperty PositionProperty =
		BindableProperty.Create(nameof(SelectedItem), typeof(int), typeof(IndexableSelectableItemsView), default(int),
			propertyChanged: PositionPropertyChanged);


		public static readonly BindableProperty PositionChangedCommandProperty =
			BindableProperty.Create(nameof(PositionChangedCommand), typeof(ICommand), typeof(IndexableSelectableItemsView));

		public static readonly BindableProperty PositionChangedCommandPropertyProperty =
			BindableProperty.Create(nameof(PositionChangedCommandParameter), typeof(object),
				typeof(IndexableSelectableItemsView));

		public int Position
		{
			get => (int)GetValue(PositionProperty);
			set => SetValue(PositionProperty, value);
		}

		public ICommand PositionChangedCommand
		{
			get => (ICommand)GetValue(SelectionChangedCommandProperty);
			set => SetValue(SelectionChangedCommandProperty, value);
		}

		public object PositionChangedCommandParameter
		{
			get => GetValue(SelectionChangedCommandParameterProperty);
			set => SetValue(SelectionChangedCommandParameterProperty, value);
		}

		public event EventHandler<PositionChangedEventArgs> PositionChanged;

		protected virtual void OnPositionChanged(PositionChangedEventArgs args)
		{
		}

		static void PositionPropertyChanged(BindableObject bindable, object oldValue, object newValue)
		{
			var indexableSelectableItemsView = (IndexableSelectableItemsView)bindable;

			var args = new PositionChangedEventArgs((int)oldValue, (int)newValue);

			var command = indexableSelectableItemsView.SelectionChangedCommand;

			if (command != null)
			{
				var commandParameter = indexableSelectableItemsView.SelectionChangedCommandParameter;

				if (command.CanExecute(commandParameter))
				{
					command.Execute(commandParameter);
				}
			}

			indexableSelectableItemsView.PositionChanged?.Invoke(indexableSelectableItemsView, args);

			indexableSelectableItemsView.OnPositionChanged(args);
		}
	}
}