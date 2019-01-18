using AButton = Android.Widget.Button;

namespace Xamarin.Forms.Platform.Android
{
	public interface IStepperRenderer
	{
		Stepper Element { get; }
		AButton GetButton(bool upButton);
		AButton CreateButton(bool isUpButton);
	}
}
