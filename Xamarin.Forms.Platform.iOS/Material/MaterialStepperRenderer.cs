using System;
using System.ComponentModel;
using CoreGraphics;
using MaterialComponents;
using UIKit;
using Xamarin.Forms;
using MButton = MaterialComponents.Button;

[assembly: ExportRenderer(typeof(Xamarin.Forms.Stepper), typeof(Xamarin.Forms.Platform.iOS.Material.MaterialStepperRenderer), new[] { typeof(VisualRendererMarker.Material) })]

namespace Xamarin.Forms.Platform.iOS.Material
{
	public class MaterialStepperRenderer : ViewRenderer<Stepper, MaterialStepper>
	{
		ButtonScheme _buttonScheme;

		public MaterialStepperRenderer()
		{
			VisualElement.VerifyVisualFlagEnabled();
		}

		protected override void Dispose(bool disposing)
		{
			if (Control is MaterialStepper control)
			{
				control.DecrementButton.TouchUpInside += OnDecrement;
				control.IncrementButton.TouchUpInside += OnIncrement;
			}

			base.Dispose(disposing);
		}

		protected override void OnElementChanged(ElementChangedEventArgs<Stepper> e)
		{
			_buttonScheme?.Dispose();
			_buttonScheme = CreateButtonScheme();

			base.OnElementChanged(e);

			if (e.NewElement != null)
			{
				if (Control == null)
				{
					var stepper = CreateNativeControl();
					stepper.DecrementButton.TouchUpInside += OnDecrement;
					stepper.IncrementButton.TouchUpInside += OnIncrement;
					SetNativeControl(stepper);
				}

				UpdateButtons();
				ApplyTheme();
			}
		}

		protected virtual ButtonScheme CreateButtonScheme()
		{
			return new ButtonScheme
			{
				ColorScheme = MaterialColors.Light.CreateColorScheme(),
				ShapeScheme = new ShapeScheme(),
				TypographyScheme = new TypographyScheme(),
			};
		}

		protected virtual void ApplyTheme()
		{
			OutlinedButtonThemer.ApplyScheme(_buttonScheme, Control.DecrementButton);
			OutlinedButtonThemer.ApplyScheme(_buttonScheme, Control.IncrementButton);
		}

		protected override MaterialStepper CreateNativeControl()
		{
			return new MaterialStepper();
		}

		protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			base.OnElementPropertyChanged(sender, e);

			if (e.PropertyName == Stepper.MinimumProperty.PropertyName ||
				e.PropertyName == Stepper.MaximumProperty.PropertyName ||
				e.PropertyName == Stepper.ValueProperty.PropertyName ||
				e.PropertyName == VisualElement.IsEnabledProperty.PropertyName)
			{
				UpdateButtons();
			}
		}

		private void UpdateButtons()
		{
			if (Element is Stepper stepper && Control is MaterialStepper control)
			{
				control.DecrementButton.Enabled = stepper.IsEnabled && stepper.Value > stepper.Minimum;
				control.IncrementButton.Enabled = stepper.IsEnabled && stepper.Value < stepper.Maximum;
			}
		}

		private void OnDecrement(object sender, EventArgs e)
		{
			if (Element is Stepper stepper)
				stepper.SetValueFromRenderer(Stepper.ValueProperty, stepper.Value - stepper.Increment);
		}

		private void OnIncrement(object sender, EventArgs e)
		{
			if (Element is Stepper stepper)
				stepper.SetValueFromRenderer(Stepper.ValueProperty, stepper.Value + stepper.Increment);
		}
	}

	public class MaterialStepper : UIView
	{
		const int DefaultButtonSpacing = 4;

		public MaterialStepper()
		{
			DecrementButton = new MButton();
			DecrementButton.SetTitle("-", UIControlState.Normal);

			IncrementButton = new MButton();
			IncrementButton.SetTitle("+", UIControlState.Normal);

			AddSubviews(DecrementButton, IncrementButton);
		}

		public MButton DecrementButton { get; }

		public MButton IncrementButton { get; }

		public override CGSize SizeThatFits(CGSize size)
		{
			var dec = DecrementButton.SizeThatFits(CGSize.Empty);
			var inc = IncrementButton.SizeThatFits(CGSize.Empty);
			var btn = new CGSize(
				Math.Max(dec.Width, inc.Width),
				Math.Max(dec.Height, inc.Height));

			return new CGSize(btn.Width + DefaultButtonSpacing + btn.Width, btn.Height);
		}

		public override void LayoutSubviews()
		{
			base.LayoutSubviews();

			var dec = DecrementButton.SizeThatFits(CGSize.Empty);
			var inc = IncrementButton.SizeThatFits(CGSize.Empty);
			var btn = new CGSize(
				Math.Max(dec.Width, inc.Width),
				Math.Max(dec.Height, inc.Height));

			DecrementButton.Frame = new CGRect(0, 0, btn.Width, btn.Height);
			IncrementButton.Frame = new CGRect(btn.Width + DefaultButtonSpacing, 0, btn.Width, btn.Height);
		}
	}
}
