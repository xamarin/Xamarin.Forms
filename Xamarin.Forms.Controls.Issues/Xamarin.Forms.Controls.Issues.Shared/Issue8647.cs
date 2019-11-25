using System;
using System.Collections.Generic;
using Xamarin.Forms.CustomAttributes;
using Xamarin.Forms.Internals;

namespace Xamarin.Forms.Controls.Issues
{
	[Preserve(AllMembers = true)]
	[Issue(IssueTracker.Github, 8647, "Crash on iOS when using DataTemplateSelector as a GroupingHeaderTemplate in a CollectionView", PlatformAffected.iOS)]
	public class Issue8647 : TestContentPage
	{
        CollectionView _view;

        protected override void Init()
		{
            var layout = new StackLayout { Margin = 50 };
            layout.Children.Add(new Button { HorizontalOptions = LayoutOptions.Center, Text = "Fill Data", Command = new Command(_ => FillData()) });
            _view = new CollectionView
            {
                IsGrouped = true,
                GroupHeaderTemplate = new DummySelector(),
                ItemTemplate = new DataTemplate(() =>
                {
                    var item = new Label();
                    item.SetBinding(Label.TextProperty, nameof(Item.Value));
                    return item;
                })
            };
            layout.Children.Add(_view);
            Content = layout;
        }

        void FillData()
        {
            if (_view.ItemsSource == null)
            {
                _view.ItemsSource = new List<List<Item>>
                {
                    new List<Item> { "1", "2", "3" },
                    new List<Item> { "4", "5" }
                };
            }
        }

        class DummySelector : DataTemplateSelector
        {
            protected override DataTemplate OnSelectTemplate(object item, BindableObject container)
            {
                return new DataTemplate(() => new Label { Text = "GroupHeader" });
            }
        }

        class Item
        {
            public string Value { get; set; }

            public static implicit operator Item(string value) => new Item { Value = value };
        }
    }
}
