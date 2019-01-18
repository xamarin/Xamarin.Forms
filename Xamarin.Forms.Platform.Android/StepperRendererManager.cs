using System.ComponentModel;
using Android.Views;
using AButton = Android.Widget.Button;
using AView = Android.Views.View;

namespace Xamarin.Forms.Platform.Android
{
	public static class StepperRendererManager
	{
		public static void CreateStepperButtons<TButton>(IStepperRenderer renderer, out TButton downButton, out TButton upButton)
			where TButton : AButton
		{
			downButton = (TButton)renderer.CreateButton(false);
			downButton.Text = "-";
			downButton.Gravity = GravityFlags.Center;
			downButton.Tag = renderer as Java.Lang.Object;
			downButton.SetOnClickListener(StepperListener.Instance);

			upButton = (TButton)renderer.CreateButton(true);
			upButton.Text = "+";
			upButton.Gravity = GravityFlags.Center;
			upButton.Tag = renderer as Java.Lang.Object;
			upButton.SetOnClickListener(StepperListener.Instance);
		}

		public static void UpdateButtons<TButton>(IStepperRenderer renderer, PropertyChangedEventArgs e, TButton downButton, TButton upButton)
			where TButton : AButton
		{
			if (!(renderer?.Element is Stepper stepper))
				return;

			if (e == null ||
				e.PropertyName == Stepper.MinimumProperty.PropertyName ||
				e.PropertyName == Stepper.MaximumProperty.PropertyName ||
				e.PropertyName == Stepper.ValueProperty.PropertyName ||
				e.PropertyName == VisualElement.IsEnabledProperty.PropertyName)
			{
				downButton.Enabled = stepper.IsEnabled && stepper.Value > stepper.Minimum;
				upButton.Enabled = stepper.IsEnabled && stepper.Value < stepper.Maximum;
			}
		}

		class StepperListener : Java.Lang.Object, AView.IOnClickListener
		{
			public static readonly StepperListener Instance = new StepperListener();

			public void OnClick(AView v)
			{
				if (!(v?.Tag is IStepperRenderer renderer))
					return;

				if (!(renderer?.Element is Stepper stepper))
					return;

				if (v == renderer.GetButton(true))
					((IElementController)stepper).SetValueFromRenderer(Stepper.ValueProperty, stepper.Value + stepper.Increment);
				else if (v == renderer.GetButton(false))
					((IElementController)stepper).SetValueFromRenderer(Stepper.ValueProperty, stepper.Value - stepper.Increment);
			}
		}
	}
}
