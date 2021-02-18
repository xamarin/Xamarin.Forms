using Xamarin.Forms;
using ASwitch = AndroidX.AppCompat.Widget.SwitchCompat;
using AAttribute = Android.Resource.Attribute;
using APorterDuff = Android.Graphics.PorterDuff;
using Android.Content;

namespace Xamarin.Platform
{
	public static class SwitchExtensions
	{
		public static void UpdateIsToggled(this ASwitch aSwitch, ISwitch view)
		{
			aSwitch.Checked = view.IsToggled;
		}

		public static void UpdateTrackColor(this ASwitch aSwitch, ISwitch view) =>
			UpdateTrackColor(aSwitch, view, null);	

		public static void UpdateTrackColor(this ASwitch aSwitch, ISwitch view, ColorStateList? defaultTrackColor)
		{
			var trackColor = view.TrackColor;

			if (aSwitch.Context == null)
				return;

			var currentTrackTintList =
				aSwitch.TrackTintList ??
				defaultTrackColor ??
				aSwitch.GetDefaultSwitchTrackColorStateList();


			if(currentTrackTintList is ColorTrackingColorStateList csl)
			{

			}
			else
			{
				int nativeOnColor = 0;
				if(trackColor.IsDefault)
				{
					nativeOnColor = aSwitch.Context.GetThemeAttrColor(AAttribute.ColorControlActivated, 0.3f);
				}
				else
				{
					nativeOnColor = onColor.ToNative();
				}

				aSwitch.GetCurrentState();

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

		// taken from android sourcec ccode
		public static ColorTrackingColorStateList GetDefaultSwitchTrackColorStateList(this ASwitch aSwitch)
		{
			var context = aSwitch.Context;
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