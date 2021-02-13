using Android.Graphics.Drawables;
using Xamarin.Forms;
using ASwitch = AndroidX.AppCompat.Widget.SwitchCompat;
using AAttribute = Android.Resource.Attribute;
using Android.Content.Res;
using AColor = Android.Graphics.Color;
using System;
using APorterDuff = Android.Graphics.PorterDuff;
using AndroidX.Core.Graphics.Drawable;
using AndroidX.AppCompat.Widget;
using Android.Content;
using Android.Util;

namespace Xamarin.Platform
{
	public static class SwitchExtensions
	{
		static ColorTrackingColorStateList? mSwitchTrackStateList;
		// taken from android sourcec ccode
		static ColorTrackingColorStateList getSwitchTrackColorStateList(this Context context)
		{
			if (mSwitchTrackStateList == null)
			{
				int[][] states = new int[3][];
				int[] colors = new int[3];
				int i = 0;
				// Disabled state
				states[i] = new int[] { -AAttribute.StateEnabled };
				colors[i] = getThemeAttrColor(context, AAttribute.ColorForeground, 0.1f);
				i++;
				states[i] = new int[] { AAttribute.StateChecked };
				colors[i] = getThemeAttrColor(context, AAttribute.ColorControlActivated, 0.3f);
				i++;
				// Default enabled state
				states[i] = new int[0];
				colors[i] = getThemeAttrColor(context, AAttribute.ColorForeground, 0.3f);
				i++;
				mSwitchTrackStateList = new ColorTrackingColorStateList(states, colors);
			}
			return mSwitchTrackStateList;
		}

		static int getThemeAttrColor(Context mContext, int attr)
		{
			TypedValue mTypedValue = new TypedValue();
			if (mContext.Theme?.ResolveAttribute(attr, mTypedValue, true) == true)
			{
				if (mTypedValue.Type >= DataType.FirstInt
						&& mTypedValue.Type <= DataType.LastInt)
				{
					return mTypedValue.Data;
				}
				else if (mTypedValue.Type == DataType.String)
				{
					throw new NotImplementedException();
				}
			}
			return 0;
		}

		static int getThemeAttrColor(Context mContext, int attr, float alpha)
		{
			int color = getThemeAttrColor(mContext, attr);
			int originalAlpha = AColor.GetAlphaComponent(color);
			// Return the color, multiplying the original alpha by the disabled value
			return (color & 0x00ffffff) | ((int)Math.Round(originalAlpha * alpha) << 24);
		}

		static int[][] _checkedStates = new int[][]
					{
						new int[] { -AAttribute.StateEnabled,  },
						new int[] { AAttribute.StateChecked },
						new int[0],
					};


		public static void UpdateIsToggled(this ASwitch aSwitch, ISwitch view)
		{
			aSwitch.Checked = view.IsToggled;
		}

		//static Drawable? _defaultTrackDrawable;
		public static void UpdateOnColor(this ASwitch aSwitch, ISwitch view)
		{
			var onColor = view.OnColor;

			if (aSwitch.Context == null)
				return;

			if (aSwitch.TrackTintList == null)
				aSwitch.TrackTintList = getSwitchTrackColorStateList(aSwitch.Context);

			if(aSwitch.Checked && aSwitch.TrackTintList is ColorTrackingColorStateList csl)
			{
				int nativeOnColor = 0;
				if(onColor.IsDefault)
				{
					nativeOnColor = getThemeAttrColor(aSwitch.Context, AAttribute.ColorControlActivated, 0.3f);
				}
				else
				{
					nativeOnColor = onColor.ToNative();
				}

				var newList = csl.CreateForState(
					new int[] { AAttribute.StateChecked },
					nativeOnColor);

				if (newList != aSwitch.TrackTintList)
					aSwitch.TrackTintList = newList;
			}
		}

		public static void UpdateOnColorSimple(this ASwitch aSwitch, ISwitch view)
		{
			var onColor = view.OnColor;

			if (!onColor.IsDefault && aSwitch.Checked)
			{
				aSwitch.TrackDrawable.SetTintMode(APorterDuff.Mode.SrcAtop);
				aSwitch.TrackDrawable.SetColorFilter(onColor.ToNative(), FilterMode.SrcAtop);
			}
			else
			{
				aSwitch.TrackDrawable.ClearColorFilter();
			}
		}

		public static void UpdateThumbColor(this ASwitch aSwitch, ISwitch view)
		{
			var thumbColor = view.ThumbColor;
			if (!thumbColor.IsDefault)
			{
				aSwitch.ThumbDrawable.SetTintMode(APorterDuff.Mode.SrcAtop);
				aSwitch.ThumbDrawable.SetTintList(
					ColorTrackingColorStateList.Create(aSwitch.ThumbTintList, _checkedStates, thumbColor, GetCurrentState(aSwitch))
					);
			}
			// Validates that we set this ThumbTintList
			else
			{
				aSwitch.ThumbDrawable.SetTintMode(APorterDuff.Mode.SrcIn);
				aSwitch.ThumbDrawable.SetTintList(null);
			}
		}

		static int[] GetCurrentState(this ASwitch aSwitch)
		{
			if (!aSwitch.Enabled)
				return _checkedStates[0];

			if (aSwitch.Checked)
				return _checkedStates[1];

			return _checkedStates[2];
		}


		class ColorTrackingColorStateList : ColorStateList
		{
			readonly int[]? _colors;
			readonly int[][]? _states;

			public ColorTrackingColorStateList(int[][]? states, int[]? colors) : base(states, colors)
			{
				_colors = colors;
				_states = states;
			}

			public ColorTrackingColorStateList(int[][]? states, Xamarin.Forms.Color color)
				: this(states, CreateColors(states, color))
			{
			}

			public ColorTrackingColorStateList(int[][]? states, Color[]? colors)
				: this(states, CreateColors(colors))
			{
			}


			public ColorTrackingColorStateList CreateForState(int[] states, Color expectedColor)
			{
				return CreateForState(states, expectedColor.ToNative());
			}
			public ColorTrackingColorStateList CreateForState(int[] states, int expectedColor)
			{
				if (_states == null || _colors == null)
					return this;

				var myColor = GetColorForState(states, new AColor());
				
				for (int i = 0; i < _states.Length; i++)
				{
					if (_states[i].Length == 0 && states.Length == 0 ||
						_states[i][0] == states[0])
					{
						if (_colors[i] != expectedColor)
						{
							_colors[i] = expectedColor;
							return new ColorTrackingColorStateList(_states, _colors);
						}

						return this;
					}
				}

				return this;
			}

			internal static ColorStateList Create(ColorStateList thumbTintList, int[][] checkedStates, Color thumbColor, int[] currentState)
			{
				if (thumbTintList == null)
					return new ColorTrackingColorStateList(checkedStates, thumbColor);

				if (!(thumbTintList is ColorTrackingColorStateList trackingColorStateList))
					return thumbTintList;

				return trackingColorStateList.CreateForState(currentState, thumbColor);
			}


			static int[]? CreateColors(Xamarin.Forms.Color[]? colors)
			{
				if (colors == null)
					return null;

				int[] aColors = new int[colors.Length];

				for (int i = 0; i < aColors.Length; i++)
				{
					if (colors[i] != Color.Default)
						aColors[i] = colors[i].ToNative();
				}

				return aColors;
			}

			static int[]? CreateColors(int[][]? states, Xamarin.Forms.Color color)
			{
				if (states == null)
					return null;

				int[] colors = new int[states.Length];

				for (int i = 0; i < states.Length; i++)
				{
					colors[i] = color.ToNative();
				}

				return colors;
			}
		}
	}
}