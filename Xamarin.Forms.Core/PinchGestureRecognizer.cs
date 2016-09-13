using System;

namespace Xamarin.Forms
{
	public sealed class PinchGestureRecognizer : GestureRecognizer, IPinchGestureController
	{
		bool IPinchGestureController.IsPinching { get; set; }

		void IPinchGestureController.SendPinch(Element sender, double delta, Point currentScalePoint)
		{
			PinchUpdated?.Invoke(sender, new PinchGestureUpdatedEventArgs(GestureStatus.Running, delta, currentScalePoint));
			((IPinchGestureController)this).IsPinching = true;
		}

		void IPinchGestureController.SendPinchCanceled(Element sender)
		{
			PinchUpdated?.Invoke(sender, new PinchGestureUpdatedEventArgs(GestureStatus.Canceled));
			((IPinchGestureController)this).IsPinching = false;
		}

		void IPinchGestureController.SendPinchEnded(Element sender)
		{
			PinchUpdated?.Invoke(sender, new PinchGestureUpdatedEventArgs(GestureStatus.Completed));
			((IPinchGestureController)this).IsPinching = false;
		}

		void IPinchGestureController.SendPinchStarted(Element sender, Point initialScalePoint)
		{
			PinchUpdated?.Invoke(sender, new PinchGestureUpdatedEventArgs(GestureStatus.Started, 1, initialScalePoint));
			((IPinchGestureController)this).IsPinching = true;
		}

		public event EventHandler<PinchGestureUpdatedEventArgs> PinchUpdated;
	}
}