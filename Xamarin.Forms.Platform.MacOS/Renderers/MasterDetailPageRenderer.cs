﻿using System;
using System.ComponentModel;
using System.Linq;
using AppKit;
using CoreGraphics;

namespace Xamarin.Forms.Platform.MacOS
{
	public class MasterDetailPageRenderer : NSSplitViewController, IVisualElementRenderer, IEffectControlProvider
	{
		bool _disposed;
		EventTracker _events;
		VisualElementTracker _tracker;
		MasterDetailPage _masterDetailPage;

		IPageController PageController => Element as IPageController;

		void IEffectControlProvider.RegisterEffect(Effect effect)
		{
			var platformEffect = effect as PlatformEffect;
			if (platformEffect != null)
				platformEffect.Container = View;
		}

		protected MasterDetailPage MasterDetailPage => _masterDetailPage ?? (_masterDetailPage = (MasterDetailPage)Element);

		protected override void Dispose(bool disposing)
		{
			if (!_disposed && disposing)
			{
				PageController?.SendDisappearing();

				if (Element != null)
				{
					Element.PropertyChanged -= HandlePropertyChanged;
					Element = null;
				}

				ClearControllers();

				_tracker?.Dispose();
				_tracker = null;
				_events?.Dispose();
				_events = null;

				_disposed = true;
			}
			base.Dispose(disposing);
		}

		public NSViewController ViewController => this;

		public NSView NativeView => View;

		public VisualElement Element { get; private set; }

		public event EventHandler<VisualElementChangedEventArgs> ElementChanged;

		public SizeRequest GetDesiredSize(double widthConstraint, double heightConstraint)
		{
			return NativeView.GetSizeRequest(widthConstraint, heightConstraint);
		}

		public void SetElement(VisualElement element)
		{
			var oldElement = Element;
			Element = element;

			UpdateControllers();

			OnElementChanged(new VisualElementChangedEventArgs(oldElement, element));

			EffectUtilities.RegisterEffectControlProvider(this, oldElement, element);
		}

		public void SetElementSize(Size size)
		{
			Element.Layout(new Rectangle(Element.X, Element.Y, size.Width, size.Height));
			UpdateChildrenLayout();
		}

		protected virtual void OnElementChanged(VisualElementChangedEventArgs e)
		{
			if (e.OldElement != null)
				e.OldElement.PropertyChanged -= HandlePropertyChanged;

			if (e.NewElement != null)
				e.NewElement.PropertyChanged += HandlePropertyChanged;

			ElementChanged?.Invoke(this, e);
		}

		protected virtual double MasterWidthPercentage => 0.3;

		public override void ViewWillAppear()
		{
			UpdateBackground();
			_tracker = new VisualElementTracker(this);
			_events = new EventTracker(this);
			_events.LoadEvents(NativeView);
			UpdateChildrenLayout();

			base.ViewWillAppear();
		}

		public override CGRect GetEffectiveRect(NSSplitView splitView, CGRect proposedEffectiveRect, CGRect drawnRect,
			nint dividerIndex)
		{
			return CGRect.Empty;
		}

		void UpdateChildrenLayout()
		{
			if (View.Frame.Width == -1)
				return;
			var width = View.Frame.Width;
			var masterWidth = MasterWidthPercentage * width;
			if (SplitViewItems.Length > 0)
				SplitViewItems[0].MaximumThickness = SplitViewItems[0].MinimumThickness = (nfloat)masterWidth;
		}

		void HandlePropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			if (_tracker == null)
				return;

			if (e.PropertyName == "Master" || e.PropertyName == "Detail")
				UpdateControllers();
		}

		void UpdateControllers()
		{
			ClearControllers();

			if (Platform.GetRenderer(MasterDetailPage.Master) == null)
				Platform.SetRenderer(MasterDetailPage.Master, Platform.CreateRenderer(MasterDetailPage.Master));
			if (Platform.GetRenderer(MasterDetailPage.Detail) == null)
				Platform.SetRenderer(MasterDetailPage.Detail, Platform.CreateRenderer(MasterDetailPage.Detail));

			AddSplitViewItem(new NSSplitViewItem
			{
				ViewController = new ViewControllerWrapper(Platform.GetRenderer(MasterDetailPage.Master))
			});
			AddSplitViewItem(new NSSplitViewItem
			{
				ViewController = new ViewControllerWrapper(Platform.GetRenderer(MasterDetailPage.Detail))
			});

			UpdateChildrenLayout();
		}

		void ClearControllers()
		{
			while (SplitViewItems.Length > 0)
			{
				var splitItem = SplitViewItems.Last();
				var childVisualRenderer = splitItem.ViewController as ViewControllerWrapper;
				RemoveSplitViewItem(splitItem);
				IVisualElementRenderer render = null;
				if (childVisualRenderer.RendererWeakRef.TryGetTarget(out render))
				{
					render.Dispose();
				}
				childVisualRenderer.Dispose();
				childVisualRenderer = null;
			}
		}

		//TODO: Implement Background color on MDP
		void UpdateBackground()
		{
		}

		sealed class ViewControllerWrapper : NSViewController
		{
			internal WeakReference<IVisualElementRenderer> RendererWeakRef;

			public ViewControllerWrapper(IVisualElementRenderer renderer)
			{
				RendererWeakRef = new WeakReference<IVisualElementRenderer>(renderer);
				View = new NSView { WantsLayer = true };
				AddChildViewController(renderer.ViewController);
				View.AddSubview(renderer.NativeView);
			}

			public override void ViewWillLayout()
			{
				IVisualElementRenderer renderer;
				if (RendererWeakRef.TryGetTarget(out renderer))
					renderer?.Element?.Layout(new Rectangle(0, 0, View.Bounds.Width, View.Bounds.Height));
				base.ViewWillLayout();
			}

			protected override void Dispose(bool disposing)
			{
				if (disposing && RendererWeakRef != null)
				{
					RendererWeakRef = null;
				}
				base.Dispose(disposing);
			}
		}
	}
}