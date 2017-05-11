using Xamarin.Forms.CustomAttributes;
using Xamarin.Forms.Internals;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Collections.ObjectModel;
using System;

#if UITEST
using Xamarin.UITest;
using NUnit.Framework;
#endif

// Apply the default category of "Issues" to all of the tests in this assembly
// We use this as a catch-all for tests which haven't been individually categorized
#if UITEST
[assembly: NUnit.Framework.Category("Issues")]
#endif

namespace Xamarin.Forms.Controls.Issues
{
	[Preserve(AllMembers = true)]
	[Issue(IssueTracker.Bugzilla, 55365, "~VisualElement crashes with System.Runtime.InteropServices.COMException", PlatformAffected.UWP)]
	public class Bugzilla55365 : TestContentPage // or TestMasterDetailPage, etc ...
	{
		protected override void Init()
		{
			var viewModel = new ObservableCollection<Item>()
			{
				//new Item() { Subject = 65 },
				//new Item() { Subject = 66 },
				new Item() { Subject = 65 },
			};

			viewModel.CollectionChanged += OnCollectionChanged;

			_itemsPanel.BindingContext = viewModel;

			foreach (var item in viewModel)
			{
				_itemTemplate.SetValue(BindingContextProperty, item);
				var view = (View)_itemTemplate.CreateContent();
				_itemsPanel.Children.Add(view);
			}


			var clearButton = new Button() { Text = "Clear", Command = new Command((o) => viewModel.Clear()) };
			var addButton = new Button()
			{
				Text = "Add",
				Command = new Command((o) =>
				{
					for (int i = 0; i < 10; i++)
					{
						viewModel.Add(new Item { Subject = 66 + i });
					}
					
					foreach (var item in viewModel)
					{
						_itemTemplate.SetValue(BindingContextProperty, item);
						var view = (View)_itemTemplate.CreateContent();
						_itemsPanel.Children.Add(view);
					}
				})
			};
			_layout.Children.Add(clearButton);
			_layout.Children.Add(addButton);

			var collectButton = new Button() { Text = "Garbage", Command = new Command((o) => GC.Collect()) };
			_layout.Children.Add(collectButton);
			_layout.Children.Add(_itemsPanel);
			Content = _layout;
		}

		void OnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
		{
			if (e.Action == NotifyCollectionChangedAction.Reset)
			{
				// reset the list
				_itemsPanel.Children.Clear();
			}
		}

		private static object CreateBoxView()
		{
			var boxView1 = new BoxView() { HeightRequest = 100, Color = new Color(0.55, 0.23, 0.147) };
			var setter1 = new Setter() { Property = BoxView.ColorProperty, Value = "#FF2879DD" };
			var trigger1 = new DataTrigger(typeof(BoxView)) { Binding = new Binding("Subject"), Value = 65 };
			trigger1.Setters.Add(setter1);
			boxView1.Triggers.Add(trigger1);
			return boxView1;
		}

		public class Item
		{
			public int Subject { get; set; }
		}

		StackLayout _layout = new StackLayout();
		StackLayout _itemsPanel = new StackLayout();
		DataTemplate _itemTemplate = new DataTemplate(CreateBoxView);

		[ContentProperty("ItemsSource")]
		public class ItemsControl : ContentView
		{
			//#region ItemsPanel

			///// <Summary>
			///// ItemsPanel BindableProperty
			///// </Summary>
			//public static readonly BindableProperty ItemsPanelProperty = BindableProperty.Create(
			//  propertyName: nameof(ItemsPanel),
			//  returnType: typeof(Layout<View>),
			//  declaringType: typeof(ItemsControl),
			//  defaultBindingMode: BindingMode.OneWay,
			//  defaultValueCreator: (b) => new StackLayout(),
			//  propertyChanged: ItemsPanelChanged);

			///// <Summary>
			///// ItemsPanel CLR property
			///// </Summary>
			//public Layout<View> ItemsPanel
			//{
			//    get { return (Layout<View>)GetValue(ItemsPanelProperty); }
			//    set { SetValue(ItemsPanelProperty, value); }
			//}

			///// <Summary>
			///// ItemsPanel change event handler
			///// </Summary>
			///// <Param name="bindable">BindableObject</param>
			///// <Param name="oldValue">old value</param>
			///// <Param name="newValue">new value</param>
			//private static void ItemsPanelChanged(BindableObject bindable, object oldValue, object newValue)
			//{
			//    var control = (ItemsControl)bindable;
			//    control.LoadChildren();
			//    control.Content = control.ItemsPanel;
			//}

			//#endregion ItemsPanel

			private Layout<View> ItemsPanel = new StackLayout();

			#region ItemsSource

			/// <Summary>
			/// ItemsSource BindableProperty
			/// </Summary>
			public static readonly BindableProperty ItemsSourceProperty = BindableProperty.Create(
			  propertyName: nameof(ItemsSource),
			  returnType: typeof(IEnumerable),
			  declaringType: typeof(ItemsControl),
			  defaultValueCreator: (b) => new List<object>(),
			  defaultBindingMode: BindingMode.OneWay,
			  propertyChanged: ItemsSourceChanged);

			/// <Summary>
			/// ItemsSource CLR property
			/// </Summary>
			public IEnumerable ItemsSource
			{
				get { return (IEnumerable)GetValue(ItemsSourceProperty); }
				set { SetValue(ItemsSourceProperty, value); }
			}

			/// <Summary>
			/// ItemsSource change event handler
			/// </Summary>
			/// <Param name="bindable">BindableObject</param>
			/// <Param name="oldValue">old value</param>
			/// <Param name="newValue">new value</param>
			private static void ItemsSourceChanged(BindableObject bindable, object oldValue, object newValue)
			{
				var control = (ItemsControl)bindable;
				var oldCollection = oldValue as INotifyCollectionChanged;

				if (oldCollection != null)
				{
					oldCollection.CollectionChanged -= control.OnCollectionChanged;
				}

				var newCollection = newValue as INotifyCollectionChanged;

				if (newCollection != null)
				{
					newCollection.CollectionChanged += control.OnCollectionChanged;
				}

				control.LoadChildren();
			}

			#endregion // ItemsSource

			#region ItemTemplate

			/// <Summary>
			/// ItemTemplate BindableProperty
			/// </Summary>
			public static readonly BindableProperty ItemTemplateProperty = BindableProperty.Create(
			  propertyName: nameof(ItemTemplate),
			  returnType: typeof(DataTemplate),
			  declaringType: typeof(ItemsControl),
			  defaultValue: default(DataTemplate),
			  propertyChanged: ItemTemplateChanged);

			/// <Summary>
			/// ItemTemplate CLR property
			/// </Summary>
			public DataTemplate ItemTemplate
			{
				get { return (DataTemplate)GetValue(ItemTemplateProperty); }
				set { SetValue(ItemTemplateProperty, value); }
			}

			/// <Summary>
			/// ItemTemplate change event handler
			/// </Summary>
			/// <Param name="bindable">BindableObject</param>
			/// <Param name="oldValue">old value</param>
			/// <Param name="newValue">new value</param>
			private static void ItemTemplateChanged(BindableObject bindable, object oldValue, object newValue)
			{
				var control = (ItemsControl)bindable;
				control.LoadChildren();
			}

			#endregion // ItemTemplate

			/// <summary>
			/// Create an items control with a <see cref="ItemsPanel"/> container
			/// </summary>
			public ItemsControl()
			{
				Content = ItemsPanel;
			}

			/// <Summary>
			/// Items of change event handler
			/// </Summary>
			/// <Param name="sender">The observable collection</param>
			/// <Param name="e">The notify collection changed arguments</param>
			private void OnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
			{
				if (e.Action == NotifyCollectionChangedAction.Reset)
				{
					// reset the list
					ItemsPanel.Children.Clear();
					//System.GC.Collect();
				}
				else
				{
					// add, remove and replace 
					if (e.OldItems != null)
					{
						var index = e.OldStartingIndex;
						var count = 0;
						while (index < ItemsPanel.Children.Count && count < e.OldItems.Count)
						{
							ItemsPanel.Children.RemoveAt(index);
							++count;
						}
					}

					if (e.NewItems != null && ItemTemplate != null)
					{
						for (var i = 0; i < e.NewItems.Count; ++i)
						{
							var item = e.NewItems[i];
							var view = CreateChildViewFor(item);
							ItemsPanel.Children.Insert(i + e.NewStartingIndex, view);
						}
					}
				}
			}

			/// <summary>
			/// Load the ItemsPanel with the children from the ItemsSource
			/// </summary>
			private void LoadChildren()
			{
				ItemsPanel.Children.Clear();

				if (ItemTemplate != null && ItemsSource != null)
				{
					foreach (var item in ItemsSource)
					{
						var view = CreateChildViewFor(item);
						ItemsPanel.Children.Add(view);
					}
				}
			}

			/// <summary>
			/// Create a child view for the specified view model.
			/// This uses the ItemTemplate to create the view.
			/// Two scenarios exist:
			/// 1) ItemTemplate is a <see cref="DataTemplateSelector"/> and this is used to select the appropriate view for the view model.
			/// 2) ItemTemplate is a <see cref="DataTemplate"/> and this is used to create the view.
			/// </summary>
			/// <param name="viewModel">The view model</param>
			/// <returns>The view created</returns>
			private View CreateChildViewFor(object viewModel)
			{
				//            ItemTemplate.SetValue(BindingContextProperty, viewModel);
				//return (View)ItemTemplate.CreateContent();
				var view = (View)ItemTemplate.CreateContent();
				view.BindingContext = viewModel;
				return view;
			}

			//protected override void OnBindingContextChanged()
			//{
			//    var bindableObject = ItemsSource as BindableObject;
			//    if (bindableObject != null)
			//    {
			//        bindableObject.BindingContext = BindingContext;
			//    }

			//    base.OnBindingContextChanged();
			//}
		}

#if UITEST
		[Test]
		public void Issue1Test()
		{
			RunningApp.Screenshot("I am at Issue 1");
			RunningApp.WaitForElement(q => q.Marked("IssuePageLabel"));
			RunningApp.Screenshot("I see the Label");
		}
#endif
	}
}