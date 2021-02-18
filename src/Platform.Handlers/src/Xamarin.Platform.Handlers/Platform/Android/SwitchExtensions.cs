using Xamarin.Forms;
using ASwitch = AndroidX.AppCompat.Widget.SwitchCompat;
using AAttribute = Android.Resource.Attribute;
using APorterDuff = Android.Graphics.PorterDuff;
using Android.Content.Res;

namespace Xamarin.Platform
{
	public static class SwitchExtensions
	{
		public static void UpdateIsToggled(this ASwitch aSwitch, ISwitch view)
		{
			aSwitch.Checked = view.IsToggled;
		}

		public static void UpdateTrackColor(this ASwitch aSwitch, ISwitch view) =>
			UpdateTrackColor(aSwitch, view, aSwitch.GetDefaultSwitchTrackColorStateList());	

		public static void UpdateTrackColor(this ASwitch aSwitch, ISwitch view, ColorStateList? defaultTrackColor)
		{
			var trackColor = view.TrackColor;

			if (aSwitch.Context == null)
				return;

			ColorStateList? currentTrackTintList =
				aSwitch.TrackTintList ??
				defaultTrackColor;

			if (aSwitch.TrackTintList == null)
			{
				aSwitch.TrackTintList = new ColorTrackingColorStateList(_checkedStates, trackColor);
			}
			else if(currentTrackTintList is ColorTrackingColorStateList csl)
			{
				var currentState = aSwitch.GetCurrentState();

				var newState  = csl.CreateForState(currentState, trackColor, defaultTrackColor);
				if (newState != aSwitch.TrackTintList)
					aSwitch.TrackTintList = newState;
			}
			else
			{
				if (!trackColor.IsDefault)
				{
					aSwitch.TrackDrawable.SetTintMode(APorterDuff.Mode.SrcAtop);
					aSwitch.TrackDrawable.SetColorFilter(trackColor.ToNative(), FilterMode.SrcAtop);
				}
				else
				{
					aSwitch.TrackDrawable.ClearColorFilter();
				}
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

		public static ColorStateList GetDefaultSwitchTrackColorStateList(this ASwitch aSwitch)
		{
			var context = aSwitch.Context;
			if (context == null)
				return new ColorTrackingColorStateList();

			int[][] states = new int[3][];
			int[] colors = new int[3];

			states[0] = new int[] { -AAttribute.StateEnabled };
			colors[0] = context.GetThemeAttrColor(AAttribute.ColorForeground, 0.1f);

			states[1] = new int[] { AAttribute.StateChecked };
			colors[1] = context.GetThemeAttrColor(AAttribute.ColorControlActivated, 0.3f);

			states[2] = new int[0];
			colors[2] = context.GetThemeAttrColor(AAttribute.ColorForeground, 0.3f);
			return new ColorTrackingColorStateList(states, colors);
		}

		static int[][] _checkedStates = new int[][]
		{
			new int[] { -AAttribute.StateEnabled },
			new int[] { AAttribute.StateChecked },
			new int[0],
		};

	}
}