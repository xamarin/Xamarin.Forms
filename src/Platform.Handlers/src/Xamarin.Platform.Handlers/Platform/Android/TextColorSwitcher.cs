using System;
using Android.Content.Res;
using Android.Widget;
using Xamarin.Forms;

namespace Xamarin.Platform
{
	/// <summary>
	/// Handles color state management for the TextColor property 
	/// for Entry, Button, Picker, TimePicker, and DatePicker
	/// </summary>
	public class TextColorSwitcher
	{
		static readonly int[][] ColorStates = { new[] { global::Android.Resource.Attribute.StateEnabled }, new[] { -global::Android.Resource.Attribute.StateEnabled } };

		readonly ColorStateList? _defaultTextColors;
		readonly bool _useLegacyColorManagement;
		Color _currentTextColor;

		public TextColorSwitcher(ColorStateList? textColors, bool useLegacyColorManagement = true)
		{
			_defaultTextColors = textColors;
			_useLegacyColorManagement = useLegacyColorManagement;
		}

		public void UpdateTextColor(TextView control, Color color, Action<ColorStateList?>? setColor = null)
		{
			if (color == _currentTextColor)
				return;

			if (setColor == null)
			{
				setColor = control.SetTextColor;
			}

			_currentTextColor = color;

			if (color.IsDefault)
			{
				setColor(_defaultTextColors);
			}
			else
			{
				if (_useLegacyColorManagement && _defaultTextColors != null)
				{
					// Set the new enabled state color, preserving the default disabled state color
					var defaultDisabledColor = _defaultTextColors.GetColorForState(ColorStates[1], color.ToNative());

					setColor(new ColorStateList(ColorStates, new[] { color.ToNative().ToArgb(), defaultDisabledColor }));
				}
				else
				{
					var acolor = color.ToNative().ToArgb();
					setColor(new ColorStateList(ColorStates, new[] { acolor, acolor }));
				}
			}
		}
	}
}