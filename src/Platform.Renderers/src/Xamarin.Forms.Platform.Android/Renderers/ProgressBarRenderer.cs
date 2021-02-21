using System.ComponentModel;
using Android.Content;
using Android.Content.Res;
using Android.OS;
using Xamarin.Platform;
using AProgressBar = Android.Widget.ProgressBar;

namespace Xamarin.Forms.Platform.Android
{
	public class ProgressBarRenderer : ViewRenderer<ProgressBar, AProgressBar>
	{
		public ProgressBarRenderer(Context context) : base(context)
		{
			AutoPackage = false;
		}

		protected override AProgressBar CreateNativeControl()
		{
			return new AProgressBar(Context, null, global::Android.Resource.Attribute.ProgressBarStyleHorizontal) { Indeterminate = false, Max = 10000 };
		}

		protected override void OnElementChanged(ElementChangedEventArgs<ProgressBar> e)
		{
			base.OnElementChanged(e);

			if (e.NewElement != null)
			{
				if (Control == null)
				{
					var progressBar = CreateNativeControl();

					SetNativeControl(progressBar);
				}

				UpdateProgressColor();
				UpdateProgress();
			}
		}

		protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			if (this.IsDisposed())
			{
				return;
			}

			base.OnElementPropertyChanged(sender, e);

			if (e.PropertyName == ProgressBar.ProgressProperty.PropertyName)
				UpdateProgress();
			else if (e.PropertyName == ProgressBar.ProgressColorProperty.PropertyName)
				UpdateProgressColor();
		}

		internal virtual protected void UpdateProgressColor()
		{
			if (Element == null || Control == null)
				return;

			Color color = Element.ProgressColor;

			if (color.IsDefault)
			{
				(Control.Indeterminate ? Control.IndeterminateDrawable :
					Control.ProgressDrawable).ClearColorFilter();
			}
			else
			{
				if (Forms.SdkInt < BuildVersionCodes.Lollipop)
				{
					(Control.Indeterminate ? Control.IndeterminateDrawable :
						Control.ProgressDrawable).SetColorFilter(color, FilterMode.SrcIn);
				}
				else
				{
					var tintList = ColorStateList.ValueOf(color.ToAndroid());
					if (Control.Indeterminate)
						Control.IndeterminateTintList = tintList;
					else
						Control.ProgressTintList = tintList;
				}
			}
		}

		void UpdateProgress()
		{
			Control.Progress = (int)(Element.Progress * 10000);
		}
	}
}