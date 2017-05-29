﻿using System;
using System.ComponentModel;
using Android.Graphics.Drawables;
using AButton = Android.Widget.Button;

namespace Xamarin.Forms.Platform.Android
{
	internal class ButtonBackgroundTracker : IDisposable
	{
		Drawable _defaultDrawable;
		ButtonDrawable _backgroundDrawable;
		Button _button;
		AButton _nativeButton;
		bool _drawableEnabled;
		bool disposedValue;

		public ButtonBackgroundTracker(Button button, AButton nativeButton)
		{
			_button = button;
			_nativeButton = nativeButton;
			_button.PropertyChanged += ButtonPropertyChanged;
		}

		public void UpdateDrawable()
		{
			if (_button.BackgroundColor == Color.Default)
			{
				if (!_drawableEnabled)
					return;

				if (_defaultDrawable != null)
					_nativeButton.SetBackground(_defaultDrawable);

				_drawableEnabled = false;
			}
			else
			{
				if (_backgroundDrawable == null)
					_backgroundDrawable = new ButtonDrawable();

				_backgroundDrawable.Button = _button;

				if (_drawableEnabled)
					return;

				if (_defaultDrawable == null)
					_defaultDrawable = _nativeButton.Background;

				_nativeButton.SetBackground(_backgroundDrawable);
				_drawableEnabled = true;
			}

			_nativeButton.Invalidate();
		}

		public void Reset()
		{
			if (_drawableEnabled)
			{
				_drawableEnabled = false;
				_backgroundDrawable.Reset();
				_backgroundDrawable = null;
			}
		}

		public void UpdateBackgroundColor()
		{
			if (_button == null)
			{
				return;
			}
			UpdateDrawable();
		}
		
		public void Dispose()
		{
			Dispose(true);
		}

		protected virtual void Dispose(bool disposing)
		{
			if (!disposedValue)
			{
				if (disposing)
				{
					_backgroundDrawable?.Dispose();
					_backgroundDrawable = null;
					_defaultDrawable?.Dispose();
					_defaultDrawable = null;
					if (_button != null)
					{
						_button.PropertyChanged -= ButtonPropertyChanged;
						_button = null;
					}
					_nativeButton = null;
				}
				disposedValue = true;
			}
		}

		void ButtonPropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			if (e.PropertyName.Equals(Button.BorderColorProperty.PropertyName) ||
				e.PropertyName.Equals(Button.BorderWidthProperty.PropertyName) ||
				e.PropertyName.Equals(Button.BorderRadiusProperty.PropertyName) ||
				e.PropertyName.Equals(VisualElement.BackgroundColorProperty.PropertyName))
			{
				Reset();
				UpdateDrawable();
			}
		}

	}
}