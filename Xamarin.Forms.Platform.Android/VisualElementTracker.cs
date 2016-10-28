using System;
using System.Collections.Generic;
using System.ComponentModel;
using Android.Content;
using Android.OS;
using Android.Views;
using AView = Android.Views.View;
using Object = Java.Lang.Object;

namespace Xamarin.Forms.Platform.Android
{
	public class VisualElementTracker : IDisposable
	{
		readonly EventHandler<EventArg<VisualElement>> _batchCommittedHandler;
		readonly IList<string> _batchedProperties = new List<string>();
		readonly PropertyChangedEventHandler _propertyChangedHandler;
		Context _context;

		bool _disposed;

		VisualElement _element;
		bool _initialUpdateNeeded = true;
		bool _layoutNeeded;
		IVisualElementRenderer _renderer;

		public VisualElementTracker(IVisualElementRenderer renderer)
		{
			if (renderer == null)
				throw new ArgumentNullException("renderer");

			_batchCommittedHandler = HandleRedrawNeeded;
			_propertyChangedHandler = HandlePropertyChanged;

			_renderer = renderer;
			_context = renderer.ViewGroup.Context;
			_renderer.ElementChanged += RendererOnElementChanged;

			VisualElement view = renderer.Element;
			SetElement(null, view);

			renderer.ViewGroup.SetCameraDistance(3600);

			renderer.ViewGroup.AddOnAttachStateChangeListener(AttachTracker.Instance);
		}

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		protected virtual void Dispose(bool disposing)
		{
			if (_disposed)
				return;

			_disposed = true;

			if (disposing)
			{
				SetElement(_element, null);

				if (_renderer != null)
				{
					_renderer.ElementChanged -= RendererOnElementChanged;
					_renderer.ViewGroup.RemoveOnAttachStateChangeListener(AttachTracker.Instance);
					_renderer = null;
					_context = null;
				}
			}
		}

		public void UpdateLayout()
		{
			Performance.Start();

			VisualElement view = _renderer.Element;
			AView aview = _renderer.ViewGroup;

			var x = (int)_context.ToPixels(view.X);
			var y = (int)_context.ToPixels(view.Y);
			var width = (int)_context.ToPixels(view.Width);
			var height = (int)_context.ToPixels(view.Height);

			var formsViewGroup = aview as FormsViewGroup;
			if (formsViewGroup == null)
			{
				Performance.Start("Measure");
				aview.Measure(MeasureSpecFactory.MakeMeasureSpec(width, MeasureSpecMode.Exactly), MeasureSpecFactory.MakeMeasureSpec(height, MeasureSpecMode.Exactly));
				Performance.Stop("Measure");

				Performance.Start("Layout");
				aview.Layout(x, y, x + width, y + height);
				Performance.Stop("Layout");
			}
			else
			{
				Performance.Start("MeasureAndLayout");
				formsViewGroup.MeasureAndLayout(MeasureSpecFactory.MakeMeasureSpec(width, MeasureSpecMode.Exactly), MeasureSpecFactory.MakeMeasureSpec(height, MeasureSpecMode.Exactly), x, y, x + width, y + height);
				Performance.Stop("MeasureAndLayout");
			}

			Performance.Stop();

			//On Width or Height changes, the anchors needs to be updated
			UpdateAnchorX();
			UpdateAnchorY();
		}

		void HandlePropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			if (e.PropertyName == Layout.IsClippedToBoundsProperty.PropertyName)
			{
				UpdateClipToBounds();
				return;
			}

			if (_renderer.Element.Batched)
			{
				if (e.PropertyName == VisualElement.XProperty.PropertyName || e.PropertyName == VisualElement.YProperty.PropertyName || e.PropertyName == VisualElement.WidthProperty.PropertyName ||
					e.PropertyName == VisualElement.HeightProperty.PropertyName)
					_layoutNeeded = true;
				else if (e.PropertyName == VisualElement.AnchorXProperty.PropertyName || e.PropertyName == VisualElement.AnchorYProperty.PropertyName || e.PropertyName == VisualElement.ScaleProperty.PropertyName ||
						 e.PropertyName == VisualElement.RotationProperty.PropertyName || e.PropertyName == VisualElement.RotationXProperty.PropertyName || e.PropertyName == VisualElement.RotationYProperty.PropertyName ||
						 e.PropertyName == VisualElement.IsVisibleProperty.PropertyName || e.PropertyName == VisualElement.OpacityProperty.PropertyName ||
						 e.PropertyName == VisualElement.TranslationXProperty.PropertyName || e.PropertyName == VisualElement.TranslationYProperty.PropertyName)
				{
					if (!_batchedProperties.Contains(e.PropertyName))
						_batchedProperties.Add(e.PropertyName);
				}
				return;
			}

			if (e.PropertyName == VisualElement.XProperty.PropertyName || e.PropertyName == VisualElement.YProperty.PropertyName || e.PropertyName == VisualElement.WidthProperty.PropertyName ||
				e.PropertyName == VisualElement.HeightProperty.PropertyName)
				MaybeRequestLayout();
			else if (e.PropertyName == VisualElement.AnchorXProperty.PropertyName)
				UpdateAnchorX();
			else if (e.PropertyName == VisualElement.AnchorYProperty.PropertyName)
				UpdateAnchorY();
			else if (e.PropertyName == VisualElement.ScaleProperty.PropertyName)
				UpdateScale();
			else if (e.PropertyName == VisualElement.RotationProperty.PropertyName)
				UpdateRotation();
			else if (e.PropertyName == VisualElement.RotationXProperty.PropertyName)
				UpdateRotationX();
			else if (e.PropertyName == VisualElement.RotationYProperty.PropertyName)
				UpdateRotationY();
			else if (e.PropertyName == VisualElement.IsVisibleProperty.PropertyName)
				UpdateIsVisible();
			else if (e.PropertyName == VisualElement.OpacityProperty.PropertyName)
				UpdateOpacity();
			else if (e.PropertyName == VisualElement.TranslationXProperty.PropertyName)
				UpdateTranslationX();
			else if (e.PropertyName == VisualElement.TranslationYProperty.PropertyName)
				UpdateTranslationY();
		}

		void HandleRedrawNeeded(object sender, EventArg<VisualElement> e)
		{
			foreach (string propertyName in _batchedProperties)
				HandlePropertyChanged(this, new PropertyChangedEventArgs(propertyName));
			_batchedProperties.Clear();

			if (_layoutNeeded)
				MaybeRequestLayout();
			_layoutNeeded = false;
		}

		void HandleViewAttachedToWindow()
		{
			if (_initialUpdateNeeded)
			{
				UpdateNativeView(this, EventArgs.Empty);
				_initialUpdateNeeded = false;
			}

			UpdateClipToBounds();
		}

		void MaybeRequestLayout()
		{
			var isInLayout = false;
			if ((int)Build.VERSION.SdkInt >= 18)
				isInLayout = _renderer.ViewGroup.IsInLayout;

			if (!isInLayout && !_renderer.ViewGroup.IsLayoutRequested)
				_renderer.ViewGroup.RequestLayout();
		}

		void RendererOnElementChanged(object sender, VisualElementChangedEventArgs args)
		{
			SetElement(args.OldElement, args.NewElement);
		}

		void SetElement(VisualElement oldElement, VisualElement newElement)
		{
			if (oldElement != null)
			{
				oldElement.BatchCommitted -= _batchCommittedHandler;
				oldElement.PropertyChanged -= _propertyChangedHandler;
				_context = null;
			}

			_element = newElement;
			if (newElement != null)
			{
				newElement.BatchCommitted += _batchCommittedHandler;
				newElement.PropertyChanged += _propertyChangedHandler;
				_context = _renderer.ViewGroup.Context;

				if (oldElement != null)
				{
					AView view = _renderer.ViewGroup;

					// ReSharper disable CompareOfFloatsByEqualityOperator
					if (oldElement.AnchorX != newElement.AnchorX)
						UpdateAnchorX();
					if (oldElement.AnchorY != newElement.AnchorY)
						UpdateAnchorY();
					if (oldElement.IsVisible != newElement.IsVisible)
						UpdateIsVisible();
					if (oldElement.IsEnabled != newElement.IsEnabled)
						view.Enabled = newElement.IsEnabled;
					if (oldElement.Opacity != newElement.Opacity)
						UpdateOpacity();
					if (oldElement.Rotation != newElement.Rotation)
						UpdateRotation();
					if (oldElement.RotationX != newElement.RotationX)
						UpdateRotationX();
					if (oldElement.RotationY != newElement.RotationY)
						UpdateRotationY();
					if (oldElement.Scale != newElement.Scale)
						UpdateScale();
					// ReSharper restore CompareOfFloatsByEqualityOperator

					_initialUpdateNeeded = false;
				}
			}
		}

		void UpdateAnchorX()
		{
			VisualElement view = _renderer.Element;
			AView aview = _renderer.ViewGroup;

			float currentPivot = aview.PivotX;
			var target = (float)(view.AnchorX * _context.ToPixels(view.Width));
			if (currentPivot != target)
				aview.PivotX = target;
		}

		void UpdateAnchorY()
		{
			VisualElement view = _renderer.Element;
			AView aview = _renderer.ViewGroup;

			float currentPivot = aview.PivotY;
			var target = (float)(view.AnchorY * _context.ToPixels(view.Height));
			if (currentPivot != target)
				aview.PivotY = target;
		}

		void UpdateClipToBounds()
		{
			var layout = _renderer.Element as Layout;
			var parent = _renderer.ViewGroup.Parent as ViewGroup;

			if (parent == null || layout == null)
				return;

			bool shouldClip = layout.IsClippedToBounds;

			if ((int)Build.VERSION.SdkInt >= 18 && parent.ClipChildren == shouldClip)
				return;

			parent.SetClipChildren(shouldClip);
			parent.Invalidate();
		}

		void UpdateIsVisible()
		{
			VisualElement view = _renderer.Element;
			AView aview = _renderer.ViewGroup;

			if (view.IsVisible && aview.Visibility != ViewStates.Visible)
				aview.Visibility = ViewStates.Visible;
			if (!view.IsVisible && aview.Visibility != ViewStates.Gone)
				aview.Visibility = ViewStates.Gone;
		}

		void UpdateNativeView(object sender, EventArgs e)
		{
			Performance.Start();

			VisualElement view = _renderer.Element;
			AView aview = _renderer.ViewGroup;

			if (aview is FormsViewGroup)
			{
				var formsViewGroup = (FormsViewGroup)aview;
				formsViewGroup.SendBatchUpdate((float)(view.AnchorX * _context.ToPixels(view.Width)), (float)(view.AnchorY * _context.ToPixels(view.Height)),
					(int)(view.IsVisible ? ViewStates.Visible : ViewStates.Invisible), view.IsEnabled, (float)view.Opacity, (float)view.Rotation, (float)view.RotationX, (float)view.RotationY, (float)view.Scale,
					_context.ToPixels(view.TranslationX), _context.ToPixels(view.TranslationY));
			}
			else
			{
				UpdateAnchorX();
				UpdateAnchorY();
				UpdateIsVisible();

				if (view.IsEnabled != aview.Enabled)
					aview.Enabled = view.IsEnabled;

				UpdateOpacity();
				UpdateRotation();
				UpdateRotationX();
				UpdateRotationY();
				UpdateScale();
				UpdateTranslationX();
				UpdateTranslationY();
			}

			Performance.Stop();
		}

		void UpdateOpacity()
		{
			Performance.Start();

			VisualElement view = _renderer.Element;
			AView aview = _renderer.ViewGroup;

			aview.Alpha = (float)view.Opacity;

			Performance.Stop();
		}

		void UpdateRotation()
		{
			VisualElement view = _renderer.Element;
			AView aview = _renderer.ViewGroup;

			aview.Rotation = (float)view.Rotation;
		}

		void UpdateRotationX()
		{
			VisualElement view = _renderer.Element;
			AView aview = _renderer.ViewGroup;

			aview.RotationX = (float)view.RotationX;
		}

		void UpdateRotationY()
		{
			VisualElement view = _renderer.Element;
			AView aview = _renderer.ViewGroup;

			aview.RotationY = (float)view.RotationY;
		}

		void UpdateScale()
		{
			VisualElement view = _renderer.Element;
			AView aview = _renderer.ViewGroup;

			aview.ScaleX = (float)view.Scale;
			aview.ScaleY = (float)view.Scale;
		}

		void UpdateTranslationX()
		{
			VisualElement view = _renderer.Element;
			AView aview = _renderer.ViewGroup;

			aview.TranslationX = _context.ToPixels(view.TranslationX);
		}

		void UpdateTranslationY()
		{
			VisualElement view = _renderer.Element;
			AView aview = _renderer.ViewGroup;

			aview.TranslationY = _context.ToPixels(view.TranslationY);
		}

		class AttachTracker : Object, AView.IOnAttachStateChangeListener
		{
			public static readonly AttachTracker Instance = new AttachTracker();

			public void OnViewAttachedToWindow(AView attachedView)
			{
				var renderer = attachedView as IVisualElementRenderer;
				if (renderer == null || renderer.Tracker == null)
					return;

				renderer.Tracker.HandleViewAttachedToWindow();
			}

			public void OnViewDetachedFromWindow(AView detachedView)
			{
			}
		}
	}
}