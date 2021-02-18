using Xamarin.Forms;
using ASwitch = AndroidX.AppCompat.Widget.SwitchCompat;
using AAttribute = Android.Resource.Attribute;
using APorterDuff = Android.Graphics.PorterDuff;
using Android.Content;
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

			if(currentTrackTintList is ColorTrackingColorStateList csl)
			{
				// Option one we detect color changes based on state
				var currentState = aSwitch.GetCurrentState();

				var newState  = csl.CreateForState(currentState, trackColor, defaultTrackColor);
				if (newState != aSwitch.TrackTintList)
					aSwitch.TrackTintList = newState;

				// Option two we just blow away the entire CSL
				aSwitch.TrackTintList = new ColorTrackingColorStateList(_checkedStates, trackColor);
			}
			else // user has define their own CSL
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

		//public static void UpdateOnColorSimple(this ASwitch aSwitch, ISwitch view)
		//{
		//	var onColor = view.OnColor;

		//	if (!onColor.IsDefault && aSwitch.Checked)
		//	{
		//		aSwitch.TrackDrawable.SetTintMode(APorterDuff.Mode.SrcAtop);
		//		aSwitch.TrackDrawable.SetColorFilter(onColor.ToNative(), FilterMode.SrcAtop);
		//	}
		//	else
		//	{
		//		aSwitch.TrackDrawable.ClearColorFilter();
		//	}
		//}

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

		// taken from android sourcec ccode
		public static ColorStateList GetDefaultSwitchTrackColorStateList(this ASwitch aSwitch)
		{
			var context = aSwitch.Context;
			if (context == null)
				return new ColorTrackingColorStateList();

			int[][] states = new int[3][];
			int[] colors = new int[3];
			int i = 0;
			// Disabled state
			states[i] = new int[] { -AAttribute.StateEnabled };
			colors[i] = context.GetThemeAttrColor(AAttribute.ColorForeground, 0.1f);
			i++;
			states[i] = new int[] { AAttribute.StateChecked };
			colors[i] = context.GetThemeAttrColor(AAttribute.ColorControlActivated, 0.3f);
			i++;
			// Default enabled state
			states[i] = new int[0];
			colors[i] = context.GetThemeAttrColor(AAttribute.ColorForeground, 0.3f);
			i++;
			return new ColorTrackingColorStateList(states, colors);
		}

		static int[][] _checkedStates = new int[][]
					{
						new int[] { -AAttribute.StateEnabled,  },
						new int[] { AAttribute.StateChecked },
						new int[0],
					};

	}
}