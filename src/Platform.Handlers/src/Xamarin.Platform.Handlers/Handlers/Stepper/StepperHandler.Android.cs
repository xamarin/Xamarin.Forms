using System;
using Android.Widget;
using Android.Views;
using AButton = Android.Widget.Button;
using AOrientation = Android.Widget.Orientation;

namespace Xamarin.Platform.Handlers
{
	public partial class StepperHandler : AbstractViewHandler<IStepper, LinearLayout>, IStepperHandler
	{
		AButton? _downButton;
		AButton? _upButton;

		IStepper? IStepperHandler.VirtualView => VirtualView;

		AButton? IStepperHandler.UpButton => _upButton;

		AButton? IStepperHandler.DownButton => _downButton;

		protected override LinearLayout CreateNativeView()
		{
			var stepperLayout = new LinearLayout(Context)
			{
				Orientation = AOrientation.Horizontal,
				Focusable = true,
				DescendantFocusability = DescendantFocusability.AfterDescendants
			};

			StepperHandlerManager.CreateStepperButtons(this, out _downButton, out _upButton);

			if (_downButton != null)
				stepperLayout.AddView(_downButton, new LinearLayout.LayoutParams(ViewGroup.LayoutParams.WrapContent, ViewGroup.LayoutParams.MatchParent));

			if (_upButton != null)
				stepperLayout.AddView(_upButton, new LinearLayout.LayoutParams(ViewGroup.LayoutParams.WrapContent, ViewGroup.LayoutParams.MatchParent));

			return stepperLayout;
		}

		AButton IStepperHandler.CreateButton()
		{
			if (Context == null)
				throw new ArgumentException("Context is null or empty", nameof(Context));

			var button = new AButton(Context);
			button.SetHeight((int)Context.ToPixels(10.0));
			return button;
		}
	}
}