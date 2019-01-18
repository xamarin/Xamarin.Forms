#if __ANDROID_28__
using System.ComponentModel;
using Android.Content;
using Android.Support.V7.View;
using Android.Widget;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android.Material;
using AButton = Android.Widget.Button;
using MButton = Android.Support.Design.Button.MaterialButton;

[assembly: ExportRenderer(typeof(Xamarin.Forms.Stepper), typeof(MaterialStepperRenderer), new[] { typeof(VisualRendererMarker.Material) })]

namespace Xamarin.Forms.Platform.Android.Material
{
	public class MaterialStepperRenderer : ViewRenderer<Stepper, LinearLayout>, IStepperRenderer
	{
		const int DefaultButtonSpacing = 8;

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
			};
		}

		protected override void OnElementChanged(ElementChangedEventArgs<Stepper> e)
		{
			base.OnElementChanged(e);

			if (e.OldElement == null)
			{
				var layout = CreateNativeControl();
				StepperRendererManager.CreateStepperButtons(this, out _downButton, out _upButton);
				layout.AddView(_downButton, new LinearLayout.LayoutParams(LayoutParams.WrapContent, LayoutParams.MatchParent));
				layout.AddView(_upButton, new LinearLayout.LayoutParams(LayoutParams.WrapContent, LayoutParams.MatchParent)
				{
					// because the buttons have no inset, we add spacing in the form of a margin to one button
					LeftMargin = (int)Context.ToPixels(DefaultButtonSpacing)
				});
				SetNativeControl(layout);
			}

			StepperRendererManager.UpdateButtons(this, null, _downButton, _upButton);
		}

		protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			base.OnElementPropertyChanged(sender, e);
			StepperRendererManager.UpdateButtons(this, e, _downButton, _upButton);
		}

		Stepper IStepperRenderer.Element => Element;

		AButton IStepperRenderer.GetButton(bool upButton) => upButton ? _upButton : _downButton;

		AButton IStepperRenderer.CreateButton(bool isUpButton)
		{
			var button = new MButton(new ContextThemeWrapper(Context, Resource.Style.XamarinFormsMaterialTheme));

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
