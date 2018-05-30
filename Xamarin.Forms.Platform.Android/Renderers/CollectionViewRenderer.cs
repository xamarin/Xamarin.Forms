using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Android.Content;
using Android.Runtime;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using Xamarin.Forms.Internals;
using Xamarin.Forms.Platform.Android.FastRenderers;
using AView = Android.Views.View;

namespace Xamarin.Forms.Platform.Android
{
	internal class CollectionViewAdapter : RecyclerView.Adapter
	{
		readonly ItemsView _itemsView;
		readonly Context _context;

		// TODO hartez 2018/05/29 17:06:45 Reconcile the type/name mismatch here	
		// This class should probably be something like ItemsViewAdapter
		internal CollectionViewAdapter(ItemsView itemsView, Context context)
		{
			_itemsView = itemsView;
			_context = context;
		}

		public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
		{
			// TODO hartez 2018/05/30 08:41:05 This needs a wrapper which can interpret the ItemsSource as an IList for us	
			if (holder is ViewHolder textViewHolder && _itemsView.ItemsSource is IList list)
			{
				textViewHolder.TextView.Text = list[position].ToString();
			}
		}

		internal class ViewHolder : RecyclerView.ViewHolder
		{
			public TextView TextView { get; set; }

			public ViewHolder(TextView itemView) : base(itemView)
			{
				TextView = itemView;
			}
		}

		public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
		{
			var view = new TextView(_context);
			return new ViewHolder(view);
		}

		// TODO hartez 2018/05/29 17:08:30 Very naive implementation, obviously we'll need something better	
		public override int ItemCount => (_itemsView.ItemsSource as IList).Count;
	}

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
				SetLayoutManager(new LinearLayoutManager(Context));
			}

			ElementChanged?.Invoke(this, new VisualElementChangedEventArgs(args.OldElement, args.NewElement));;
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