using System;
using Xamarin.Forms.Internals;
using FormsDevice = Xamarin.Forms.Device;

namespace Xamarin.Forms
{
	public class OrientationStateTrigger : StateTriggerBase
	{
		public OrientationStateTrigger()
		{
			UpdateState();

			if (!DesignMode.IsDesignModeEnabled)
			{
				var weakEvent = new WeakEventListener<OrientationStateTrigger, object, EventArgs>(this)
				{
					OnEventAction = (instance, source, eventArgs) => OnSizeChanged(source, eventArgs),
					OnDetachAction = (listener) => { Application.Current.MainPage.SizeChanged -= listener.OnEvent; }
				};

				Application.Current.MainPage.SizeChanged += weakEvent.OnEvent;
			}
		}

		public DeviceOrientation Orientation
		{
			get => (DeviceOrientation)GetValue(OrientationProperty);
			set => SetValue(OrientationProperty, value);
		}

		public static readonly BindableProperty OrientationProperty =
		BindableProperty.Create(nameof(Orientation), typeof(DeviceOrientation), typeof(OrientationStateTrigger), null,
			propertyChanged: OnOrientationChanged);

		static void OnOrientationChanged(BindableObject bindable, object oldvalue, object newvalue)
		{
			((OrientationStateTrigger)bindable).UpdateState();
		}

		void OnSizeChanged(object sender, EventArgs e)
		{
			UpdateState();
		}

		void UpdateState()
		{
			var currentOrientation = FormsDevice.Info.CurrentOrientation;

			switch (Orientation)
			{
				case DeviceOrientation.Landscape:
					SetActive(currentOrientation == DeviceOrientation.Landscape);
					break;
				case DeviceOrientation.LandscapeLeft:
					SetActive(currentOrientation == DeviceOrientation.LandscapeLeft);
					break;
				case DeviceOrientation.LandscapeRight:
					SetActive(currentOrientation == DeviceOrientation.LandscapeRight);
					break;
				case DeviceOrientation.Portrait:
					SetActive(currentOrientation == DeviceOrientation.Portrait);
					break;
				case DeviceOrientation.PortraitDown:
					SetActive(currentOrientation == DeviceOrientation.PortraitDown);
					break;
				case DeviceOrientation.PortraitUp:
					SetActive(currentOrientation == DeviceOrientation.PortraitUp);
					break;
			}
		}
	}
}