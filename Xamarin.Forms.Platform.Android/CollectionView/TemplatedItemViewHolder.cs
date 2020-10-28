using System;
using System.Collections.Generic;
using System.Linq;
using Xamarin.Forms.Internals;

namespace Xamarin.Forms.Platform.Android
{
	public class TemplatedItemViewHolder : SelectableViewHolder
	{
		bool? _useAndroidDefaultsDrawableSelectionColors;
		readonly ItemContentView _itemContentView;
		readonly DataTemplate _template;
		DataTemplate _selectedTemplate;

		public View View { get; private set; }

		public TemplatedItemViewHolder(ItemContentView itemContentView, DataTemplate template,
			bool isSelectionEnabled = true) : base(itemContentView, isSelectionEnabled)
		{
			_itemContentView = itemContentView;
			_template = template;
		}

		protected override void OnSelectedChanged()
		{
			base.OnSelectedChanged();

			if (View == null)
			{
				return;
			}

			VisualStateManager.GoToState(View, IsSelected
				? VisualStateManager.CommonStates.Selected
				: VisualStateManager.CommonStates.Normal);
		}

		public void Recycle(ItemsView itemsView)
		{
			if (View == null)
			{
				return;
			}

			itemsView.RemoveLogicalChild(View);
			View.BindingContext = null;
		}

		public void Bind(object itemBindingContext, ItemsView itemsView,
			Action<Size> reportMeasure = null, Size? size = null)
		{
			var template = _template.SelectDataTemplate(itemBindingContext, itemsView);

			var templateChanging = template != _selectedTemplate;

			if (templateChanging)
			{
				// Clean up any content we're still holding on to
				_itemContentView.Recycle();

				// Create the new content
				View = (View)template.CreateContent();

				// Set the binding context _before_ we create the renderer; that way, the bound data will be 
				// available during OnElementChanged
				View.BindingContext = itemBindingContext;

				// Make sure the Visual property is available when the renderer is created
				PropertyPropagationExtensions.PropagatePropertyChanged(null, View, itemsView);

				// Prevents the use of Android default color for selected item
				UseAndroidDefaultColorsForSelection = UseAndroidDefaultsSelectionColors(itemsView);

				// Actually create the native renderer
				_itemContentView.RealizeContent(View);

				_selectedTemplate = template;
			}

			_itemContentView.HandleItemSizingStrategy(reportMeasure, size);

			if (!templateChanging)
			{
				// Same template, new data
				View.BindingContext = itemBindingContext;
			}

			itemsView.AddLogicalChild(View);
		}


		bool UseAndroidDefaultsSelectionColors(ItemsView itemsView)
		{
			// Walks through the resource dictionary to know
			// if there are a resource dictionary that have VisualStateGroups
			// with Selected State, overriding the Android default
			// selection color

			if (_useAndroidDefaultsDrawableSelectionColors.HasValue)
            {
                return _useAndroidDefaultsDrawableSelectionColors.Value;
            }

            if (itemsView.FindParentOfType<Page>() is Page page)
            {
                foreach (var resourceDictionary in page.Resources)
                {
                    if (resourceDictionary.Value is Style style
						&& style.TargetType.IsAssignableFrom(View.GetType()))
                    {
                        foreach (var setter in style.Setters)
                        {
                            if (setter.Property == VisualStateManager.VisualStateGroupsProperty
								&& setter.Value is VisualStateGroupList visualStateGroups)
                            {
                                foreach (var group in visualStateGroups)
                                {
                                    foreach (var state in group.States)
                                    {
                                        if (state.Name == "Selected")
                                        {
                                            foreach (var visualStateSetter in state.Setters)
                                            {
                                                if (visualStateSetter.Property == VisualElement.BackgroundColorProperty)
                                                {
                                                    _useAndroidDefaultsDrawableSelectionColors = false;
                                                    return false;
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }

			_useAndroidDefaultsDrawableSelectionColors = true;
            return true;
		}
	}
}
