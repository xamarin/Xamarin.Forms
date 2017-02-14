using EProgressBar = ElmSharp.ProgressBar;
using EColor = ElmSharp.Color;

namespace Xamarin.Forms.Platform.Tizen
{
	public class ActivityIndicatorRenderer : ViewRenderer<ActivityIndicator, EProgressBar>
	{
		static readonly EColor s_defaultColor = EColor.Black;

		public ActivityIndicatorRenderer()
		{
			RegisterPropertyHandler(ActivityIndicator.ColorProperty, UpdateColor);
			RegisterPropertyHandler(ActivityIndicator.IsRunningProperty, UpdateIsRunning);
		}

		protected override void OnElementChanged(ElementChangedEventArgs<ActivityIndicator> e)
		{
			if (Control == null)
			{
				var ac = new EProgressBar(Forms.Context.MainWindow)
				{
					Style = "process_medium",
					IsPulseMode = true,
				};
				SetNativeControl(ac);
			}

			if (e.OldElement != null)
			{
			}

			if (e.NewElement != null)
			{
			}

			base.OnElementChanged(e);
		}

		void UpdateColor()
		{
			Control.Color = (Element.Color == Color.Default) ? s_defaultColor : Element.Color.ToNative();
		}

		void UpdateIsRunning()
		{
			if (Element.IsRunning)
			{
				Control.PlayPulse();
			}
			else
			{
				Control.StopPulse();
			}
		}

	};
}
