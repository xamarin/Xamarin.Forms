using System;

namespace Xamarin.Forms
{
	public class AdaptiveTrigger : StateTriggerBase
	{
		public AdaptiveTrigger()
		{
			UpdateState();

			if (!DesignMode.IsDesignModeEnabled)
			{
				var weakEvent = new WeakEventListener<AdaptiveTrigger, object, EventArgs>(this)
				{
					OnEventAction = (instance, source, eventArgs) => OnSizeChanged(source, eventArgs),
					OnDetachAction = (listener) => { Application.Current.MainPage.SizeChanged -= listener.OnEvent; }
				};

				Application.Current.MainPage.SizeChanged += weakEvent.OnEvent;
			}
		}

		public double MinWindowHeight
		{
			get => (double)GetValue(MinWindowHeightProperty);
			set => SetValue(MinWindowHeightProperty, value);
		}

		public static readonly BindableProperty MinWindowHeightProperty =
		BindableProperty.Create(nameof(MinWindowHeight), typeof(double), typeof(AdaptiveTrigger), -1d,
			propertyChanged: OnMinWindowHeightChanged);

		static void OnMinWindowHeightChanged(BindableObject bindable, object oldvalue, object newvalue)
		{
			((AdaptiveTrigger)bindable).UpdateState();
		}

		public double MinWindowWidth
		{
			get => (double)GetValue(MinWindowWidthProperty);
			set => SetValue(MinWindowWidthProperty, value);
		}

		public static readonly BindableProperty MinWindowWidthProperty =
			BindableProperty.Create(nameof(MinWindowWidthProperty), typeof(double), typeof(AdaptiveTrigger), -1d,
				propertyChanged: OnMinWindowWidthChanged);

		static void OnMinWindowWidthChanged(BindableObject bindable, object oldvalue, object newvalue)
		{
			((AdaptiveTrigger)bindable).UpdateState();
		}

		void OnSizeChanged(object sender, EventArgs e)
		{
			UpdateState();
		}

		void UpdateState()
		{
			var size = Device.Info.PixelScreenSize;

			var w = size.Width;
			var h = size.Height;
			var mw = MinWindowWidth;
			var mh = MinWindowHeight;

			var isActive = w >= mw && h >= mh;

			if (isActive && mw >= 0)
				SetActivePrecedence(StateTriggerPrecedence.MinWidthTrigger);
			else if (isActive)
				SetActivePrecedence(StateTriggerPrecedence.MinHeightTrigger);
			else
				SetActivePrecedence(StateTriggerPrecedence.Inactive);
		}
	}
}