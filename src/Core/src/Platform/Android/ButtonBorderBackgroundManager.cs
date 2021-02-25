using Android.Content.Res;
using Android.Graphics.Drawables;
using Android.Views;
using System;

namespace Microsoft.Maui
{
	public class ButtonBorderBackgroundManager : IDisposable
	{
		Drawable? _defaultDrawable;
		ButtonBorderDrawable? _backgroundDrawable;
		RippleDrawable? _rippleDrawable;
		bool _drawableEnabled;

		View? _nativeView;
		IBorder? _border;

		public ButtonBorderBackgroundManager(View nativeView, IBorder? border)
		{
			_nativeView = nativeView;
			_border = border;
		}

		public static Color ColorButtonNormalOverride { get; set; }

		public void UpdateDrawable()
		{
			// TODO: Set BorderColor and BorderWidth

			if (_border == null || _nativeView == null)
				return;

			bool cornerRadiusIsDefault = _border.CornerRadius == -1;
			bool backgroundColorIsDefault = _border.BackgroundColor == Color.Default;

			if (backgroundColorIsDefault
				&& cornerRadiusIsDefault)
			{
				if (!_drawableEnabled)
					return;

				if (_defaultDrawable != null)
					_nativeView?.SetBackground(_defaultDrawable);

				_drawableEnabled = false;
				Reset();
			}
			else
			{
				if (_nativeView != null && _backgroundDrawable == null)
					_backgroundDrawable = new ButtonBorderDrawable(_nativeView.Context!.ToPixels, _nativeView.GetColorButtonNormal());

				if (_backgroundDrawable != null)
					_backgroundDrawable.BorderElement = _border;

				if (_drawableEnabled)
					return;

				if (_defaultDrawable == null)
					_defaultDrawable = _nativeView?.Background;

				if (!backgroundColorIsDefault)
				{
					var rippleColor = _backgroundDrawable?.PressedBackgroundColor.ToNative();

					if (rippleColor.HasValue)
					{
						_rippleDrawable = new RippleDrawable(ColorStateList.ValueOf(rippleColor.Value), _backgroundDrawable, null);
						_nativeView?.SetBackground(_rippleDrawable);
					}
				}

				_drawableEnabled = true;
			}

			_nativeView?.Invalidate();
		}

		public void Reset()
		{
			if (_drawableEnabled)
			{
				_drawableEnabled = false;
				_backgroundDrawable?.Reset();
				_backgroundDrawable = null;
				_rippleDrawable = null;
			}
		}

		public void Dispose()
		{
			_backgroundDrawable?.Dispose();
			_backgroundDrawable = null;

			_defaultDrawable?.Dispose();
			_defaultDrawable = null;

			_rippleDrawable?.Dispose();
			_rippleDrawable = null;

			_border = null;
			_nativeView = null;
		}
	}
}