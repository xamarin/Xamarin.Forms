using System;
using Android.Hardware.Camera2;

namespace Xamarin.Forms.Platform.Android
{
	class CameraStateListener : CameraDevice.StateCallback
	{
		public Action<CameraDevice> OnOpenedAction;
		public Action<CameraDevice> OnDisconnectedAction;
		public Action<CameraDevice, CameraError> OnErrorAction;
		public Action<CameraDevice> OnClosedAction;

		public override void OnOpened(CameraDevice camera) => OnOpenedAction?.Invoke(camera);

		public override void OnDisconnected(CameraDevice camera) => OnDisconnectedAction?.Invoke(camera);

		public override void OnError(CameraDevice camera, CameraError error) => OnErrorAction(camera, error);

		public override void OnClosed(CameraDevice camera) => OnClosedAction(camera);
	}
}