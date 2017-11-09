using FormsFlexLayoutDemo;
using System;
using System.Collections.Generic;

using Xamarin.Forms;

namespace FormsFlexLayoutDemo
{
    public partial class FlexDemoPage : ContentPage
    {
        int _initialItems = 2;
        public FlexDemoPage()
        {
            InitializeComponent();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            for (int i = 0; i < _initialItems; i++)
                AddClicked(this, null);
        }

        void ClearClicked(object sender, EventArgs e)
        {
            for (int i = flexBox.Children.Count - 1; i >= 0; i--)
                Remove(flexBox.Children[i] as FlexItemView);
        }

        void AddClicked(object sender, EventArgs e)
        {
            flexBox.Children.Add(CreateItem());
        }

        void RemoveClicked(object sender, EventArgs e)
        {
            if (flexBox.Children.Count >= 1)
                Remove(flexBox.Children[flexBox.Children.Count - 1] as FlexItemView);
        }

        View CreateItem()
        {
            var item = new FlexItemView
            {
                Style = Resources["DemoItemStyle"] as Style
            };
            (item.BindingContext as FlexItemViewModel).Index = flexBox.Children.Count + 1;
            item.RemoveClicked += ItemRemoveClicked;
            return item;
        }

        void ItemRemoveClicked(object sender, EventArgs e)
        {
            var item = (sender as FlexItemView);
            Remove(item);
        }

        void Remove(FlexItemView item)
        {
            if (item == null)
                return;
            item.RemoveClicked -= ItemRemoveClicked;
            flexBox.Children.Remove(item);
        }
    }
}
