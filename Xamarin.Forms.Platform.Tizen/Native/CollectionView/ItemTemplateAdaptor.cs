using System.Collections;
using System.Collections.Generic;
using ElmSharp;
using ESize = ElmSharp.Size;
using XLabel = Xamarin.Forms.Label;

namespace Xamarin.Forms.Platform.Tizen.Native
{
	public class ItemDefaultTemplateAdaptor : ItemTemplateAdaptor
	{
		public ItemDefaultTemplateAdaptor(ItemsView itemsView) : base(itemsView)
		{
			ItemTemplate = new DataTemplate(() =>
			{
				XLabel label;
				var view = new StackLayout
				{
					BackgroundColor = Color.White,
					Padding = 30,
					Children =
					{
						(label = new XLabel())
					}
				};
				label.SetBinding(XLabel.TextProperty, new Binding("."));
				return view;
			});
		}
	}

	public class ItemTemplateAdaptor : ItemAdaptor
	{
		Dictionary<EvasObject, View> _nativeFormsTable = new Dictionary<EvasObject, View>();
		Dictionary<object, View> _dataBindedViewTable = new Dictionary<object, View>();
		ItemsView _itemsView;

		public ItemTemplateAdaptor(ItemsView itemsView) : base(itemsView.ItemsSource)
		{
			ItemTemplate = itemsView.ItemTemplate;
			_itemsView = itemsView;
		}

		protected ItemTemplateAdaptor(ItemsView itemsView, IEnumerable items, DataTemplate template) : base(items)
		{
			ItemTemplate = template;
			_itemsView = itemsView;
		}

		protected DataTemplate ItemTemplate { get; set; }

		protected View GetTemplatedView(EvasObject evasObject)
		{
			return _nativeFormsTable[evasObject];
		}

		public override EvasObject CreateNativeView(EvasObject parent)
		{
			var view = ItemTemplate.CreateContent() as View;
			var renderer = Platform.GetOrCreateRenderer(view);
			var native = Platform.GetOrCreateRenderer(view).NativeView;
			view.Parent = _itemsView;
			(renderer as LayoutRenderer)?.RegisterOnLayoutUpdated();

			_nativeFormsTable[native] = view;
			return native;
		}

		public override void RemoveNativeView(EvasObject native)
		{
			if (_nativeFormsTable.TryGetValue(native, out View view))
			{
				ResetBindedView(view);
				Platform.GetRenderer(view)?.Dispose();
				_nativeFormsTable.Remove(native);
			}
		}

		public override void SetBinding(EvasObject native, int index)
		{
			if (_nativeFormsTable.TryGetValue(native, out View view))
			{
				ResetBindedView(view);
				view.BindingContext = this[index];
				_dataBindedViewTable[this[index]] = view;
			}
		}

		public override ESize MeasureItem(int widthConstraint, int heightConstraint)
		{
			return MeasureItem(0, widthConstraint, heightConstraint);
		}

		public override ESize MeasureItem(int index, int widthConstraint, int heightConstraint)
		{
			if (_dataBindedViewTable.TryGetValue(this[index], out View createdView) && createdView != null)
			{
				return createdView.Measure(Forms.ConvertToScaledDP(widthConstraint), Forms.ConvertToScaledDP(heightConstraint), MeasureFlags.IncludeMargins).Request.ToPixel();
			}

			var view = ItemTemplate.CreateContent() as View;
			using (var renderer = Platform.GetOrCreateRenderer(view))
			{
				view.Parent = _itemsView;
				if (Count > index)
					view.BindingContext = this[index];
				var request = view.Measure(Forms.ConvertToScaledDP(widthConstraint), Forms.ConvertToScaledDP(heightConstraint), MeasureFlags.IncludeMargins).Request;
				return request.ToPixel();
			}
		}

		void ResetBindedView(View view)
		{
			if (view.BindingContext != null && _dataBindedViewTable.ContainsKey(view.BindingContext))
			{
				_dataBindedViewTable[view.BindingContext] = null;
			}
		}
	}
}
