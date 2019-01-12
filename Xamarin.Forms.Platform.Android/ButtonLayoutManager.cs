using System;
using System.ComponentModel;
using System.Linq;
using Android.Content;
using Android.Graphics;
using Android.Graphics.Drawables;
using Android.Support.V4.View;
using Android.Support.V4.Widget;
using Android.Support.V7.Widget;
using Xamarin.Forms.Internals;
using Xamarin.Forms.PlatformConfiguration.AndroidSpecific;
using Specifics = Xamarin.Forms.PlatformConfiguration.AndroidSpecific;
using AView = Android.Views.View;

namespace Xamarin.Forms.Platform.Android
{
	// TODO: Currently the drawable is reloaded if the text or the layout changes.
	//       This is obviously not great, but it works. An optimization should
	//       be made to find the drawable in the view and just re-position.
	//       If we do this, we must remember to undo the offset in OnLayout.

	// TODO: The padding options here need investigation. See UpdatePadding().

	// TODO: Sometimes the border/padding does not affect the bounds of the 
	//       button due to minimum size constraints. See UpdateBounds(bool).

	public class ButtonLayoutManager : IDisposable
	{
		// we use left/right as this does not resize the button when there is no text
		Button.ButtonContentLayout _imageOnlyLayout = new Button.ButtonContentLayout(Button.ButtonContentLayout.ImagePosition.Left, 0);

		// reuse this instance to save on allocations
		Rect _drawableBounds = new Rect();

		bool _disposed;
		IButtonLayoutRenderer _renderer;
		Thickness? _defaultPaddingPix;
		Button _element;
		bool _alignIconWithText;
		bool _preserveInitialPadding;

		public ButtonLayoutManager(IButtonLayoutRenderer renderer, bool alignIconWithText = false, bool preserveInitialPadding = true)
		{
			_renderer = renderer;
			_renderer.ElementChanged += OnElementChanged;
			_alignIconWithText = alignIconWithText;
			_preserveInitialPadding = preserveInitialPadding;
		}

		AppCompatButton View => _renderer?.View;

		Context Context => _renderer?.View?.Context;

		public void Dispose()
		{
			Dispose(true);
		}

		protected virtual void Dispose(bool disposing)
		{
			if (!_disposed)
			{
				if (disposing)
				{
					if (_renderer != null)
					{
						_renderer.ElementChanged -= OnElementChanged;
						_renderer = null;
					}
				}
				_disposed = true;
			}
		}

		public void OnLayout(bool changed, int left, int top, int right, int bottom)
		{
			if (_disposed || _renderer == null || _element == null)
				return;

			AppCompatButton view = View;
			if (view == null || view.Layout == null)
				return;

			if (TextViewCompat.GetCompoundDrawablesRelative(view)?.FirstOrDefault(d => d != null) is Drawable drawable)
			{
				int iconWidth = drawable.IntrinsicWidth;
				drawable.CopyBounds(_drawableBounds);

				// Center the drawable in the button if there is no text.
				// We do not need to undo this as when we get some text, the drawable recreated
				if (string.IsNullOrEmpty(_element.Text))
				{
					var newLeft = (right - left - iconWidth) / 2 - view.PaddingLeft;

					_drawableBounds.Set(newLeft, _drawableBounds.Top, newLeft + iconWidth, _drawableBounds.Bottom);
					drawable.Bounds = _drawableBounds;
				}
				else
				{
					if (_alignIconWithText && _element.ContentLayout.IsHorizontal)
					{
						var buttonText = view.TextFormatted;

						// if text is transformed, add that transformation to to ensure correct calculation of icon padding
						if (view.TransformationMethod != null)
							buttonText = view.TransformationMethod.GetTransformationFormatted(buttonText, view);

						var measuredTextWidth = view.Paint.MeasureText(buttonText, 0, buttonText.Length());
						var textWidth = Math.Min((int)measuredTextWidth, view.Layout.Width);
						var contentsWidth = ViewCompat.GetPaddingStart(view) + iconWidth + view.CompoundDrawablePadding + textWidth + ViewCompat.GetPaddingEnd(view);

						var newLeft = (view.MeasuredWidth - contentsWidth) / 2;
						if (_element.ContentLayout.Position == Button.ButtonContentLayout.ImagePosition.Right)
							newLeft = -newLeft;
						if (ViewCompat.GetLayoutDirection(view) == ViewCompat.LayoutDirectionRtl)
							newLeft = -newLeft;

						_drawableBounds.Set(newLeft, _drawableBounds.Top, newLeft + iconWidth, _drawableBounds.Bottom);
						drawable.Bounds = _drawableBounds;
					}
				}
			}
		}

		public void OnViewAttachedToWindow(AView attachedView)
		{
			Update();
		}

		public void OnViewDetachedFromWindow(AView detachedView)
		{
		}

		public void Update()
		{
			if (!UpdateTextAndImage())
				UpdateImage();
			UpdatePadding();
		}

		void OnElementChanged(object sender, VisualElementChangedEventArgs e)
		{
			if (_element != null)
			{
				_element.PropertyChanged -= OnElementPropertyChanged;
				_element = null;
			}

			if (e.NewElement is Button button)
			{
				_element = button;
				_element.PropertyChanged += OnElementPropertyChanged;
			}

			Update();
		}

		void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			if (_disposed || _renderer == null || _element == null)
				return;

			if (e.PropertyName == Button.PaddingProperty.PropertyName)
				UpdatePadding();
			else if (e.PropertyName == Button.ImageProperty.PropertyName || e.PropertyName == Button.ContentLayoutProperty.PropertyName)
				UpdateImage();
			else if (e.PropertyName == Button.TextProperty.PropertyName || e.PropertyName == VisualElement.IsVisibleProperty.PropertyName)
				UpdateTextAndImage();
			else if (e.PropertyName == Button.BorderWidthProperty.PropertyName || e.PropertyName == Specifics.Button.BorderAdjustsPaddingProperty.PropertyName)
				_element.InvalidateMeasureNonVirtual(InvalidationTrigger.MeasureChanged);
		}

		void UpdatePadding()
		{
			if (_disposed || _renderer == null || _element == null)
				return;

			AppCompatButton view = View;
			if (view == null)
				return;

			if (!_defaultPaddingPix.HasValue)
				_defaultPaddingPix = new Thickness(view.PaddingLeft, view.PaddingTop, view.PaddingRight, view.PaddingBottom);

			var padding = _element.Padding;

			var adjustment = 0.0;
			if (_element.IsSet(Specifics.Button.BorderAdjustsPaddingProperty) && _element.OnThisPlatform().GetBorderAdjustsPadding() &&
				_element is IBorderElement borderElement && borderElement.IsBorderWidthSet() && borderElement.BorderWidth != borderElement.BorderWidthDefaultValue)
				adjustment = borderElement.BorderWidth;

			var defaultPadding = _preserveInitialPadding
				? _defaultPaddingPix.Value
				: default(Thickness);

			// TODO: The padding options here are both wrong. Need to sort this out.

			view.SetPadding(
				(int)(Context.ToPixels(padding.Left + adjustment) + defaultPadding.Left),
				(int)(Context.ToPixels(padding.Top + adjustment) + defaultPadding.Top),
				(int)(Context.ToPixels(padding.Right + adjustment) + defaultPadding.Right),
				(int)(Context.ToPixels(padding.Bottom + adjustment) + defaultPadding.Bottom));

			//if (_element.IsSet(Button.PaddingProperty))
			//{
			//	view.SetPadding(
			//		(int)(Context.ToPixels(padding.Left)),
			//		(int)(Context.ToPixels(padding.Top)),
			//		(int)(Context.ToPixels(padding.Right)),
			//		(int)(Context.ToPixels(padding.Bottom)));
			//}
			//else
			//{
			//	view.SetPadding(
			//		(int)_defaultPaddingPix.Value.Left,
			//		(int)_defaultPaddingPix.Value.Top,
			//		(int)_defaultPaddingPix.Value.Right,
			//		(int)_defaultPaddingPix.Value.Bottom);
			//}
		}

		//Size? _defaultMinSizePix;

		//void UpdateBounds(bool invalidateMeasure)
		//{
		//	if (_disposed || _renderer == null || _element == null)
		//		return;

		//	AppCompatButton view = View;
		//	if (view == null)
		//		return;

		//	// the defaults
		//	if (!_defaultMinSizePix.HasValue)
		//		_defaultMinSizePix = new Size(view.MinWidth, view.MinHeight);
		//	if (!_defaultPaddingPix.HasValue)
		//		_defaultPaddingPix = new Thickness(view.PaddingLeft, view.PaddingTop, view.PaddingRight, view.PaddingBottom);

		//	// get the border width, if we are going to adjust the bounds
		//	var adjustment = 0;
		//	if (_element.IsSet(Specifics.Button.BorderAdjustsPaddingProperty) && _element.OnThisPlatform().GetBorderAdjustsPadding() &&
		//		_element is IBorderElement borderElement && borderElement.IsBorderWidthSet() && borderElement.BorderWidth != borderElement.BorderWidthDefaultValue)
		//		adjustment = (int)Context.ToPixels(borderElement.BorderWidth);

		//	// we need to increase the minimum bounds
		//	if (_defaultMinSizePix?.Width is double width && width > 0)
		//		view.SetMinWidth((int)(width + (adjustment * 2)));
		//	if (_defaultMinSizePix?.Height is double height && height > 0)
		//		view.SetMinHeight((int)(height + (adjustment * 2)));

		//	// we need to increase the padding
		//	var padding = _element.Padding;
		//	view.SetPadding(
		//		(int)(Context.ToPixels(padding.Left) + _defaultPaddingPix.Value.Left),
		//		(int)(Context.ToPixels(padding.Top) + _defaultPaddingPix.Value.Top),
		//		(int)(Context.ToPixels(padding.Right) + _defaultPaddingPix.Value.Right),
		//		(int)(Context.ToPixels(padding.Bottom) + _defaultPaddingPix.Value.Bottom));

		//	if (invalidateMeasure)
		//		_element.InvalidateMeasureNonVirtual(InvalidationTrigger.MeasureChanged);
		//}

		bool UpdateTextAndImage()
		{
			if (_disposed || _renderer == null || _element == null)
				return false;

			AppCompatButton view = View;
			if (view == null)
				return false;

			string oldText = view.Text;
			view.Text = _element.Text;

			// If we went from or to having no text, we need to update the image position
			if (string.IsNullOrEmpty(oldText) != string.IsNullOrEmpty(view.Text))
			{
				UpdateImage();
				return true;
			}

			return false;
		}

		void UpdateImage()
		{
			if (_disposed || _renderer == null || _element == null)
				return;

			AppCompatButton view = View;
			if (view == null)
				return;

			FileImageSource elementImage = _element.Image;
			string imageFile = elementImage?.File;

			if (elementImage == null || string.IsNullOrEmpty(imageFile))
			{
				view.SetCompoundDrawablesWithIntrinsicBounds(null, null, null, null);
				return;
			}

			using (var image = Context.GetDrawable(imageFile))
			{
				// No text, so no need for relative position; just center the image
				// There's no option for just plain-old centering, so we'll use Top 
				// (which handles the horizontal centering) and some tricksy padding (in OnLayout)
				// to handle the vertical centering 
				var layout = string.IsNullOrEmpty(_element.Text) ? _imageOnlyLayout : _element.ContentLayout;

				view.CompoundDrawablePadding = (int)Context.ToPixels(layout.Spacing);

				switch (layout.Position)
				{
					case Button.ButtonContentLayout.ImagePosition.Top:
						TextViewCompat.SetCompoundDrawablesRelativeWithIntrinsicBounds(view, null, image, null, null);
						break;
					case Button.ButtonContentLayout.ImagePosition.Right:
						TextViewCompat.SetCompoundDrawablesRelativeWithIntrinsicBounds(view, null, null, image, null);
						break;
					case Button.ButtonContentLayout.ImagePosition.Bottom:
						TextViewCompat.SetCompoundDrawablesRelativeWithIntrinsicBounds(view, null, null, null, image);
						break;
					default:
						// Defaults to image on the left
						TextViewCompat.SetCompoundDrawablesRelativeWithIntrinsicBounds(view, image, null, null, null);
						break;
				}
			}
		}
	}
}
