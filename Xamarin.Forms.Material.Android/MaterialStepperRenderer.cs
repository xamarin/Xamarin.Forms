#if __ANDROID_28__
using System.ComponentModel;
using Android.Content;
using Android.Views;
using Android.Widget;
using Xamarin.Forms;
using Xamarin.Forms.Material.Android;
using Xamarin.Forms.Platform.Android;
using AButton = Android.Widget.Button;
using MButton = Android.Support.Design.Button.MaterialButton;


namespace Xamarin.Forms.Material.Android
{
	public class MaterialStepperRenderer : ViewRenderer<Stepper, LinearLayout>, IStepperRenderer
	{
		const int DefaultButtonSpacing = 4;

		MButton _downButton;
		MButton _upButton;

		public MaterialStepperRenderer(Context context) : base(context)
		{
			AutoPackage = false;
		}

		protected override LinearLayout CreateNativeControl()
		{
			return new LinearLayout(Context)
			{
				Orientation = Orientation.Horizontal,
				Focusable = true,
				DescendantFocusability = DescendantFocusability.AfterDescendants
			};
		}

		protected override void OnElementChanged(ElementChangedEventArgs<Stepper> e)
		{
			base.OnElementChanged(e);

			if (e.OldElement == null)
			{
				if (Control == null)
				{
					var layout = CreateNativeControl();
					StepperRendererManager.CreateStepperButtons(this, out _downButton, out _upButton);
					layout.AddView(_downButton, new LinearLayout.LayoutParams(LayoutParams.WrapContent, LayoutParams.MatchParent)
					{
						Weight = 1,
						RightMargin = (int)(Context.ToPixels(DefaultButtonSpacing) / 2),
					});
					layout.AddView(_upButton, new LinearLayout.LayoutParams(LayoutParams.WrapContent, LayoutParams.MatchParent)
					{
						Weight = 1,
						LeftMargin = (int)(Context.ToPixels(DefaultButtonSpacing) / 2),
					});

					SetNativeControl(layout);
				}
			}

			StepperRendererManager.UpdateButtons(this, _downButton, _upButton);
		}

		protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			base.OnElementPropertyChanged(sender, e);

			StepperRendererManager.UpdateButtons(this, _downButton, _upButton, e);
		}

		protected override void UpdateBackgroundColor()
		{
			// don't call base
		}

		// IStepperRenderer

		Stepper IStepperRenderer.Element => Element;

		AButton IStepperRenderer.UpButton => _upButton;

		AButton IStepperRenderer.DownButton => _downButton;

		AButton IStepperRenderer.CreateButton()
		{
			var button = new MButton(MaterialContextThemeWrapper.Create(Context), null, Resource.Attribute.materialOutlinedButtonStyle);

			// the buttons are meant to be "square", but are usually wide,
			// so, copy the vertical properties into the horizontal properties
			button.SetMinimumWidth(button.MinimumHeight);
			button.SetMinWidth(button.MinHeight);
			button.SetPadding(button.PaddingTop, button.PaddingTop, button.PaddingBottom, button.PaddingBottom);

			return button;
		}
	}
}
#endif
