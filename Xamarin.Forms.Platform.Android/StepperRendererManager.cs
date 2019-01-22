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
			downButton = (TButton)renderer.CreateButton();
			downButton.Id = Platform.GenerateViewId();
			downButton.Focusable = true;
			downButton.Text = "-";
			downButton.Gravity = GravityFlags.Center;
			downButton.Tag = renderer as Java.Lang.Object;
			downButton.SetOnClickListener(StepperListener.Instance);

			upButton = (TButton)renderer.CreateButton();
			upButton.Id = Platform.GenerateViewId();
			upButton.Focusable = true;
			upButton.Text = "+";
			upButton.Gravity = GravityFlags.Center;
			upButton.Tag = renderer as Java.Lang.Object;
			upButton.SetOnClickListener(StepperListener.Instance);

			downButton.NextFocusForwardId = upButton.Id;
		}

		public static void UpdateButtons<TButton>(IStepperRenderer renderer, TButton downButton, TButton upButton, PropertyChangedEventArgs e = null)
			where TButton : AButton
		{
			if (!(renderer?.Element is Stepper stepper))
				return;

			// NOTE: a value of `null` means that we are forcing an update
			if (e == null ||
				e.IsOneOf(Stepper.MinimumProperty, Stepper.MaximumProperty, Stepper.ValueProperty, VisualElement.IsEnabledProperty))
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

				var increment = stepper.Increment;
				if (v == renderer.DownButton)
					increment = -increment;

				((IElementController)stepper).SetValueFromRenderer(Stepper.ValueProperty, stepper.Value + increment);
			}
		}
	}
}
