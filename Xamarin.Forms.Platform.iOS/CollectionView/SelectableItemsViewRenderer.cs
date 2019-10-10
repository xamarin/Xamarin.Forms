using System;
using System.ComponentModel;

namespace Xamarin.Forms.Platform.iOS
{
	public class SelectableItemsViewRenderer<TItemsView, TViewController> : StructuredItemsViewRenderer<TItemsView, TViewController>
		where TItemsView : SelectableItemsView
		where TViewController : SelectableItemsViewController
	{
		protected override TViewController CreateController(TItemsView itemsView, ItemsViewLayout layout)
		{
			return new SelectableItemsViewController(itemsView, layout) as TViewController;
		}

		protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs changedProperty)
		{
			base.OnElementPropertyChanged(sender, changedProperty);

			if (changedProperty.IsOneOf(SelectableItemsView.SelectedItemProperty, SelectableItemsView.SelectedItemsProperty))
			{
				UpdateNativeSelection();
			}
			else if (changedProperty.Is(SelectableItemsView.SelectionModeProperty))
			{
				UpdateSelectionMode();
			}
		}

		protected override void SetUpNewElement(TItemsView newElement)
		{
			base.SetUpNewElement(newElement);

			if (newElement == null)
			{
				return;
			}

			UpdateSelectionMode();
			UpdateNativeSelection();
		}

		protected virtual void UpdateNativeSelection()
		{
			ItemsViewController.UpdateNativeSelection();
		}

		protected virtual void UpdateSelectionMode()
		{
			ItemsViewController.UpdateSelectionMode();
		}

		protected override void UpdateItemsSource()
		{
			base.UpdateItemsSource();
			UpdateNativeSelection();
		}
	}
}