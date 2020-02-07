using System.ComponentModel;
using Xamarin.Forms.Internals;
using FormsDevice = Xamarin.Forms.Device;

namespace Xamarin.Forms
{
	public sealed class OrientationStateTrigger : StateTriggerBase
	{
		public OrientationStateTrigger()
		{
			UpdateState();

			if (!DesignMode.IsDesignModeEnabled)
			{
				var weakEvent = new WeakEventListener<OrientationStateTrigger, object, PropertyChangedEventArgs>(this)
				{
					OnEventAction = (instance, source, eventArgs) => OnInfoPropertyChanged(source, eventArgs),
					OnDetachAction = (listener) => { FormsDevice.Info.PropertyChanged -= listener.OnEvent; }
				};

				FormsDevice.Info.PropertyChanged += weakEvent.OnEvent;
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

		void OnInfoPropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			if(e.PropertyName == "CurrentOrientation")
				UpdateState();
		}

		void UpdateState()
		{
			var currentOrientation = FormsDevice.Info.CurrentOrientation;

			switch (Orientation)
			{
				case DeviceOrientation.Landscape:
				case DeviceOrientation.LandscapeLeft:
				case DeviceOrientation.LandscapeRight:
					SetActive(
						currentOrientation == DeviceOrientation.Landscape ||
						currentOrientation == DeviceOrientation.LandscapeLeft ||
						currentOrientation == DeviceOrientation.LandscapeRight);
					break;
				case DeviceOrientation.Portrait:
				case DeviceOrientation.PortraitDown:
				case DeviceOrientation.PortraitUp:
					SetActive(
						currentOrientation == DeviceOrientation.Portrait ||
						currentOrientation == DeviceOrientation.PortraitDown ||
						currentOrientation == DeviceOrientation.PortraitUp);
					break;
			}
		}
	}
}