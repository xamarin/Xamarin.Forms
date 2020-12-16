﻿using System;
using Android.Content;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Google.Android.Material.AppBar;
using AView = Android.Views.View;
using LP = Android.Views.ViewGroup.LayoutParams;

namespace Xamarin.Forms.Platform.Android
{
	// This is used to monitor an xplat View and apply layout changes
	internal class ShellViewRenderer
	{
		public IVisualElementRenderer Renderer { get; private set; }
		View _view;
		WeakReference<Context> _context;
		public double Width { get; private set; }
		public double Height { get; private set; }

		public ShellViewRenderer(Context context, View view)
		{
			_context = new WeakReference<Context>(context);
			View = view;
		}

		public View View
		{
			get { return _view; }
			set
			{
				OnViewSet(value);
			}
		}

		public void TearDown()
		{
			View = null;
			Renderer?.Dispose();
			Renderer = null;
			_view = null;
			_context = null;
		}

		public void LayoutView(double width, double height, double? maxWidth = null, double? maxHeight = null)
		{
			if (width == -1)
				width = double.PositiveInfinity;

			if (height == -1)
				height = double.PositiveInfinity;

			Width = width;
			Height = height;
			Context context;

			if (Renderer == null || !(_context.TryGetTarget(out context)) || !Renderer.View.IsAlive())
				return;

			if (View == null)
			{
				var empty = MeasureSpecFactory.GetSize(0);
				Renderer.View.Measure(empty, empty);
				return;
			}

			var request = View.Measure(width, height, MeasureFlags.None);

			var layoutParams = NativeView.LayoutParameters;
			if (height == -1 || double.IsInfinity(height))
				height = request.Request.Height;

			if (width == -1 || double.IsInfinity(width))
				width = request.Request.Width;

			if (height > maxHeight)
				height = maxHeight.Value;

			if (width > maxWidth)
				width = maxWidth.Value;

			if (layoutParams.Width != LP.MatchParent)
				layoutParams.Width = (int)context.ToPixels(width);

			if (layoutParams.Height != LP.MatchParent)
				layoutParams.Height = (int)context.ToPixels(height);

			NativeView.LayoutParameters = layoutParams;
			View.Layout(new Rectangle(0, 0, width, height));
			Renderer.UpdateLayout();
		}

		public virtual void OnViewSet(View view)
		{
			if (View != null)
				View.SizeChanged -= OnViewSizeChanged;

			if (View is VisualElement oldView)
				oldView.MeasureInvalidated -= OnViewSizeChanged;

			if (Renderer != null)
			{
				Renderer.View.RemoveFromParent();
				Renderer.Dispose();
				Renderer = null;
			}

			_view = view;
			if (view != null)
			{
				Context context;

				if (!(_context.TryGetTarget(out context)))
					return;

				Renderer = Platform.CreateRenderer(view, context);
				Platform.SetRenderer(view, Renderer);
				NativeView = Renderer.View;

				if (View is VisualElement ve)
					ve.MeasureInvalidated += OnViewSizeChanged;
				else
					View.SizeChanged += OnViewSizeChanged;
			}
			else
			{
				NativeView = null;
			}
		}

		void OnViewSizeChanged(object sender, EventArgs e) =>
			LayoutView(Width, Height);

		public AView NativeView
		{
			get;
			private set;
		}
	}
}