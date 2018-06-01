using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Android.Content;
using Android.Runtime;
using Android.Support.V7.Widget;
using Android.Views;
using Xamarin.Forms.Internals;
using Xamarin.Forms.Platform.Android.FastRenderers;
using AView = Android.Views.View;

namespace Xamarin.Forms.Platform.Android
{
	public class CollectionViewRenderer : RecyclerView, IVisualElementRenderer, IEffectControlProvider
	{
		ItemsView ItemsView { get; set; }

		RecyclerView.Adapter _adapter;

		readonly AutomationPropertiesProvider _automationPropertiesProvider;
		readonly EffectControlProvider _effectControlProvider;
		int? _defaultLabelFor;
		bool _isDisposed;

		public CollectionViewRenderer(Context context) : base(context)
		{
			_automationPropertiesProvider = new AutomationPropertiesProvider(this);
			_effectControlProvider = new EffectControlProvider(this);
		}

		public VisualElement Element => ItemsView;
		public VisualElementTracker Tracker { get; private set; }
		public ViewGroup ViewGroup => null;
		public AView View => this;

		public event EventHandler<VisualElementChangedEventArgs> ElementChanged;
		public event EventHandler<PropertyChangedEventArgs> ElementPropertyChanged;

		SizeRequest IVisualElementRenderer.GetDesiredSize(int widthConstraint, int heightConstraint)
		{
			Measure(widthConstraint, heightConstraint);
			return new SizeRequest(new Size(MeasuredWidth, MeasuredHeight), new Size());
		}

		void IVisualElementRenderer.SetElement(VisualElement element)
		{
			if (element == null)
			{
				throw new ArgumentNullException(nameof(element));
			}

			if (!(element is ItemsView))
			{
				throw new ArgumentException($"{nameof(element)} must be of type {nameof(ItemsView)}");
			}

			VisualElement oldElement = ItemsView;
			ItemsView = (ItemsView)element;

			Performance.Start(out string perfRef);

			if (oldElement != null)
			{
				oldElement.PropertyChanged -= OnElementPropertyChanged;
			}

			element.PropertyChanged += OnElementPropertyChanged;

			if (Tracker == null)
			{
				Tracker = new VisualElementTracker(this);
			}

			OnElementChanged(new ElementChangedEventArgs<ItemsView>(oldElement as ItemsView, ItemsView));

			element.SendViewInitialized(this);

			EffectUtilities.RegisterEffectControlProvider(this, oldElement, element);

			Performance.Stop(perfRef);
		}

		void OnElementChanged(ElementChangedEventArgs<ItemsView> args)
		{
			if (args.OldElement != null)
			{
				if (_adapter != null)
				{
					_adapter.Dispose();
					_adapter = null;
				}
			}

			if (args.NewElement != null)
			{
				this.EnsureId();

				// TODO hartez 2018/05/29 20:28:14 Review whether we really need to keep a ref to adapter	
				_adapter = new CollectionViewAdapter(args.NewElement, Context);
				SetAdapter(_adapter);
				SetLayoutManager(SelectLayoutManager(args.NewElement.ItemsLayout));
			}

			ElementChanged?.Invoke(this, new VisualElementChangedEventArgs(args.OldElement, args.NewElement));;
		}

		protected virtual RecyclerView.LayoutManager SelectLayoutManager(IItemsLayout layoutSpecification)
		{
			if (layoutSpecification is ListItemsLayout listItemsLayout)
			{
				var orientation = listItemsLayout.Orientation == ItemsLayoutOrientation.Horizontal
					? LinearLayoutManager.Horizontal
					: LinearLayoutManager.Vertical;

				return new LinearLayoutManager(Context, orientation, false);
			}

			// TODO hartez 2018/06/01 09:28:16 Handle grid	

			// Fall back to plain old vertical list
			return new LinearLayoutManager(Context);
		}

		void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			ElementPropertyChanged?.Invoke(this, e);
		}

		void IVisualElementRenderer.SetLabelFor(int? id)
		{
			if (_defaultLabelFor == null)
			{
				_defaultLabelFor = LabelFor;
			}

			LabelFor = (int)(id ?? _defaultLabelFor);
		}

		void IVisualElementRenderer.UpdateLayout()
		{
			Tracker?.UpdateLayout();
		}

		void IEffectControlProvider.RegisterEffect(Effect effect)
		{
			_effectControlProvider.RegisterEffect(effect);
		}

		protected override void Dispose(bool disposing)
		{
			if (_isDisposed)
			{
				return;
			}

			_isDisposed = true;

			if (disposing)
			{
				_automationPropertiesProvider?.Dispose();
				Tracker?.Dispose();

				if (_adapter != null)
				{
					_adapter.Dispose();
					_adapter = null;
				}

				if (Element != null)
				{
					Element.PropertyChanged -= OnElementPropertyChanged;

					if (Platform.GetRenderer(Element) == this)
						Element.ClearValue(Platform.RendererProperty);
				}
			}

			base.Dispose(disposing);
		}
	}
}