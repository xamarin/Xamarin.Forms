using System;
using System.ComponentModel;
using Android.Content;
using Android.Graphics;
using Android.Util;
using Xamarin.Forms.PlatformConfiguration.AndroidSpecific;
using static System.String;
using AView = Android.Views.View;
using AFloatingButton = global::Android.Support.Design.Widget.FloatingActionButton;
using AMotionEvent = Android.Views.MotionEvent;
using AMotionEventActions = Android.Views.MotionEventActions;
using Object = Java.Lang.Object;
using Xamarin.Forms.Internals;
using System.Threading.Tasks;
using Android.Graphics.Drawables;

namespace Xamarin.Forms.Platform.Android
{
	public class FloatingActionButtonRenderer : ViewRenderer<FloatingActionButton, AFloatingButton>, AView.IOnAttachStateChangeListener
	{
		const float smallSize = 44f;
		const float normalSize = 56f;

		bool _isDisposed;

		public FloatingActionButtonRenderer(Context context) : base(context)
		{
			AutoPackage = false;
		}

		AFloatingButton NativeButton
		{
			get { return Control; }
		}

		public void OnViewAttachedToWindow(AView attachedView)
		{
		}

		public void OnViewDetachedFromWindow(AView detachedView)
		{
		}

		public override SizeRequest GetDesiredSize(int widthConstraint, int heightConstraint)
		{
			return base.GetDesiredSize(widthConstraint, heightConstraint);
		}

		protected override void OnLayout(bool changed, int l, int t, int r, int b)
		{
			var size = Element.Size == FloatingActionButtonSize.Mini ? smallSize : normalSize;
			size = Context.ToPixels(size);

			base.OnLayout(changed, l, t, l + (int)size, t + (int)size);
		}

		protected override void Dispose(bool disposing)
		{
			if (_isDisposed)
				return;

			_isDisposed = true;

			if (disposing)
			{
			}

			base.Dispose(disposing);
		}

		protected override AFloatingButton CreateNativeControl()
		{
			return new AFloatingButton(Context);
		}

		protected override async void OnElementChanged(ElementChangedEventArgs<FloatingActionButton> e)
		{
			base.OnElementChanged(e);

			if (e.OldElement == null)
			{
				AFloatingButton button = Control;

				if (button == null)
				{
					button = CreateNativeControl();
					button.SetOnClickListener(ButtonClickListener.Instance.Value);
					button.SetOnTouchListener(ButtonTouchListener.Instance.Value);
					button.Tag = this;

					SetNativeControl(button);

					//var useLegacyColorManagement = e.NewElement.UseLegacyColorManagement();
					//_textColorSwitcher = new TextColorSwitcher(button.TextColors, useLegacyColorManagement);

					button.AddOnAttachStateChangeListener(this);
				}
			}

			//if (_backgroundTracker == null)
			//	_backgroundTracker = new ButtonBackgroundTracker(Element, Control);
			//else
				//_backgroundTracker.Button = e.NewElement;

			await UpdateAll();
		}

		protected override async void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			base.OnElementPropertyChanged(sender, e);

			if (e.PropertyName == FloatingActionButton.ColorProperty.PropertyName)
				UpdateColor();
			else if (e.PropertyName == FloatingActionButton.SizeProperty.PropertyName)
				this.UpdateLayout();
			else if (e.PropertyName == Image.SourceProperty.PropertyName)
				await TryUpdateBitmap();
			else if (e.PropertyName == VisualElement.IsEnabledProperty.PropertyName)
				UpdateEnabled();
		}

		protected override void UpdateBackgroundColor()
		{
			if (Element == null || Control == null)
				return;
		}

		async Task UpdateAll()
		{
			UpdateColor();
			UpdateEnabled();
			await TryUpdateBitmap();
		}

		void UpdateColor()
		{
			// lets check if we can do that on droid actually
		}

		void UpdateEnabled()
		{
			Control.Enabled = Element.IsEnabled;
		}

		protected virtual async Task TryUpdateBitmap()
		{
			try
			{
				await UpdateBitmap();
			}
			catch (Exception ex)
			{
				Internals.Log.Warning(nameof(FloatingActionButtonRenderer), "Error loading image: {0}", ex);
			}
			finally
			{
			}
		}

		protected async Task UpdateBitmap()
		{
			if (Element == null || Control == null || Control.IsDisposed())
			{
				return;
			}

			if (NativeButton == null || NativeButton.IsDisposed())
				return;

			if (Device.IsInvokeRequired)
				throw new InvalidOperationException("Image Bitmap must not be updated from background thread");

			var source = Element.ImageSource;

			Bitmap bitmap = null;
			Drawable drawable = null;

			IImageSourceHandler handler;

			if (source != null && (handler = Internals.Registrar.Registered.GetHandlerForObject<IImageSourceHandler>(source)) != null)
			{
				if (handler is FileImageSourceHandler)
				{
					drawable = NativeButton.Context.GetDrawable((FileImageSource)source);
				}

				if (drawable == null)
				{
					try
					{
						bitmap = await handler.LoadImageAsync(source, NativeButton.Context);
					}
					catch (TaskCanceledException)
					{						
					}
				}
			}

			if (!NativeButton.IsDisposed())
			{
				if (bitmap == null && drawable != null)
				{
					NativeButton.SetImageDrawable(drawable);
				}
				else
				{
					NativeButton.SetImageBitmap(bitmap);
				}
			}

			bitmap?.Dispose();
		}

		class ButtonClickListener : Object, IOnClickListener
		{
			public static readonly Lazy<ButtonClickListener> Instance = new Lazy<ButtonClickListener>(() => new ButtonClickListener());

			public void OnClick(AView v)
			{
				var renderer = v.Tag as FloatingActionButtonRenderer;
				if (renderer != null)
					((IButtonController)renderer.Element).SendClicked();
			}
		}

		class ButtonTouchListener : Object, IOnTouchListener
		{
			public static readonly Lazy<ButtonTouchListener> Instance = new Lazy<ButtonTouchListener>(() => new ButtonTouchListener());

			public bool OnTouch(AView v, AMotionEvent e)
			{
				var renderer = v.Tag as FloatingActionButtonRenderer;
				if (renderer != null)
				{
					var buttonController = renderer.Element as IButtonController;
					if (e.Action == AMotionEventActions.Down)
					{
						buttonController?.SendPressed();
					}
					else if (e.Action == AMotionEventActions.Up)
					{
						buttonController?.SendReleased();
					}
				}
				return false;
			}
		}
	}
}