using System;
using System.ComponentModel;
using System.Threading.Tasks;
using Android.Content;
using Android.Graphics;
using AImageView = Android.Widget.ImageView;
using AView = Android.Views.View;
using Android.Views;
using Xamarin.Forms.Internals;
using Android.Support.V4.View;

// TODO GIF
namespace Xamarin.Forms.Platform.Android.FastRenderers
{
	public class ImageRenderer : AImageView, IVisualElementRenderer, IImageRendererController, IViewRenderer, ITabStop,
		ILayoutChanges
	{
		bool _disposed;
		Image _element;
		bool _skipInvalidate;
		int? _defaultLabelFor;
		VisualElementTracker _visualElementTracker;
		VisualElementRenderer _visualElementRenderer;
		readonly MotionEventHelper _motionEventHelper = new MotionEventHelper();

		bool IImageRendererController.IsDisposed => _disposed;
		protected override void Dispose(bool disposing)
		{
			if (_disposed)
				return;

			_disposed = true;

			if (disposing)
			{
				if (_element != null)
				{
					_element.PropertyChanged -= OnElementPropertyChanged;
				}

				ImageElementManager.Dispose(this);
				BackgroundManager.Dispose(this);

				if (_visualElementTracker != null)
				{
					_visualElementTracker.Dispose();
					_visualElementTracker = null;
				}

				if (_visualElementRenderer != null)
				{
					_visualElementRenderer.Dispose();
					_visualElementRenderer = null;
				}

				if (Control != null)
				{
					if (Control.Drawable is IFormsAnimationDrawable animation)
						animation.AnimationStopped -= OnAnimationStopped;

					Control.Reset();
				}

				if (_element != null)
				{
					if (Platform.GetRenderer(_element) == this)
						_element.ClearValue(Platform.RendererProperty);
				}
			}

			base.Dispose(disposing);
		}

		public override void Invalidate()
		{
			if (_skipInvalidate)
			{
				_skipInvalidate = false;
				return;
			}

			base.Invalidate();
		}

		protected virtual void OnElementChanged(ElementChangedEventArgs<Image> e)
		{
			this.EnsureId();
			ElementChanged?.Invoke(this, new VisualElementChangedEventArgs(e.OldElement, e.NewElement));
		}

		public override bool OnTouchEvent(MotionEvent e)
		{
			if (_visualElementRenderer.OnTouchEvent(e) || base.OnTouchEvent(e))
			{
				return true;
			}

			return _motionEventHelper.HandleMotionEvent(Parent, e);
		}

		Size MinimumSize()
		{
			return new Size();
		}

		SizeRequest IVisualElementRenderer.GetDesiredSize(int widthConstraint, int heightConstraint)
		{
			if (_disposed)
			{
				return new SizeRequest();
			}

			Measure(widthConstraint, heightConstraint);
			return new SizeRequest(new Size(MeasuredWidth, MeasuredHeight), MinimumSize());
		}

		void IVisualElementRenderer.SetElement(VisualElement element)
		{
			if (element == null)
				throw new ArgumentNullException(nameof(element));

			var image = element as Image;
			if (image == null)
				throw new ArgumentException("Element is not of type " + typeof(Image), nameof(element));

			Image oldElement = _element;
			_element = image;

			Performance.Start(out string reference);

			if (oldElement != null)
				oldElement.PropertyChanged -= OnElementPropertyChanged;

			element.PropertyChanged += OnElementPropertyChanged;

			if (_visualElementTracker == null)
			{
				_visualElementTracker = new VisualElementTracker(this);
			}

			if (_visualElementRenderer == null)
			{
				_visualElementRenderer = new VisualElementRenderer(this);
				BackgroundManager.Init(this);
				ImageElementManager.Init(this);
			}

			Performance.Stop(reference);
			_motionEventHelper.UpdateElement(element);
			OnElementChanged(new ElementChangedEventArgs<Image>(oldElement, _element));

			_element?.SendViewInitialized(Control);
		}

		void IVisualElementRenderer.SetLabelFor(int? id)
		{
			if (_defaultLabelFor == null)
				_defaultLabelFor = ViewCompat.GetLabelFor(this);

			ViewCompat.SetLabelFor(this, (int)(id ?? _defaultLabelFor));
		}

		void IVisualElementRenderer.UpdateLayout() => _visualElementTracker?.UpdateLayout();

		void IViewRenderer.MeasureExactly()
		{
			ViewRenderer.MeasureExactly(this, ((IVisualElementRenderer)this).Element, Context);
		}

		VisualElement IVisualElementRenderer.Element => _element;

		VisualElementTracker IVisualElementRenderer.Tracker => _visualElementTracker;

		AView IVisualElementRenderer.View => this;

		AView ITabStop.TabStop => this;

		ViewGroup IVisualElementRenderer.ViewGroup => null;

		void IImageRendererController.SkipInvalidate() => _skipInvalidate = true;

		protected AImageView Control => this;
		protected Image Element => _element;

		public event EventHandler<VisualElementChangedEventArgs> ElementChanged;
		public event EventHandler<PropertyChangedEventArgs> ElementPropertyChanged;

		public ImageRenderer(Context context) : base(context)
		{
		}

		[Obsolete("This constructor is obsolete as of version 2.5. Please use ImageRenderer(Context) instead.")]
		[EditorBrowsable(EditorBrowsableState.Never)]
		public ImageRenderer() : base(Forms.Context)
		{
		}

		protected virtual void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			if (e.PropertyName == Image.SourceProperty.PropertyName)
				await TryUpdateBitmap();
			else if (e.PropertyName == Image.AspectProperty.PropertyName)
				UpdateAspect();
			else if (e.PropertyName == Image.IsAnimationPlayingProperty.PropertyName)
				await StartStopAnimation();

			ElementPropertyChanged?.Invoke(this, e);
		}

		async Task TryUpdateBitmap(Image previous = null)
		{
			// By default we'll just catch and log any exceptions thrown by UpdateBitmap so they don't bring down
			// the application; a custom renderer can override this method and handle exceptions from
			// UpdateBitmap differently if it wants to

			try
			{
				await UpdateBitmap(previous);
			}
			catch (Exception ex)
			{
				Log.Warning(nameof(ImageRenderer), "Error loading image: {0}", ex);
			}
			finally
			{
				((IImageController)_element)?.SetIsLoading(false);
			}
		}

		async Task UpdateBitmap(Image previous = null)
		{
			if (_element == null || _disposed)
			{
				return;
			}

			if (Control.Drawable is IFormsAnimationDrawable currentAnimation)
			{
				currentAnimation.Stop();
				currentAnimation.AnimationStopped -= OnAnimationStopped;
			}

			await Control.UpdateBitmap(_element, previous);

			if (Control.Drawable is IFormsAnimationDrawable updatedAnimation)
			{
				updatedAnimation.AnimationStopped += OnAnimationStopped;
				if (_element.IsAnimationAutoPlay)
					updatedAnimation.Start();
			}
		}

		void UpdateAspect()
		{
			if (_element == null || _disposed)
			{
				return;
			}

			ScaleType type = _element.Aspect.ToScaleType();
			SetScaleType(type);
		}

		void OnAnimationStopped(object sender, FormsAnimationDrawableStateEventArgs e)
		{
			if (_element != null && !_disposed && e.Finished)
				_element.OnAnimationFinishedPlaying();
		}

		async Task StartStopAnimation()
		{
			if (_disposed || _element == null || Control == null)
			{
				return;
			}

			if (_element.IsLoading)
				return;

			if (!(Control.Drawable is IFormsAnimationDrawable) && _element.IsAnimationPlaying)
				await TryUpdateBitmap();

			if (Control.Drawable is IFormsAnimationDrawable animation)
			{
				if (_element.IsAnimationPlaying && !animation.IsRunning)
					animation.Start();
				else if (!_element.IsAnimationPlaying && animation.IsRunning)
					animation.Stop();
			}
		}
	}
}
