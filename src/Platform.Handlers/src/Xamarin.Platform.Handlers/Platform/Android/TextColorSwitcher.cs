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
		Color _currentTextColor;

		public TextColorSwitcher(ColorStateList? textColors)
		{
			_defaultTextColors = textColors;
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
				var acolor = color.ToNative().ToArgb();
				setColor(new ColorStateList(ColorStates, new[] { acolor, acolor }));
			}
		}
	}
}
