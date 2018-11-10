using System;
using System.ComponentModel;
using Android.Content;
using Android.Widget;
using ACheckBox = Android.Widget.CheckBox;

namespace Xamarin.Forms.Platform.Android
{
	public class CheckBoxRenderer : ViewRenderer<CheckBox, ACheckBox>, CompoundButton.IOnCheckedChangeListener
	{
		public CheckBoxRenderer(Context context) : base(context)
		{
			AutoPackage = false;
		}

		void CompoundButton.IOnCheckedChangeListener.OnCheckedChanged(CompoundButton buttonView, bool isChecked)
		{
			((IViewController)Element).SetValueFromRenderer(CheckBox.IsCheckedProperty, isChecked);
		}

		public override SizeRequest GetDesiredSize(int widthConstraint, int heightConstraint)
		{
			SizeRequest sizeConstraint = base.GetDesiredSize(widthConstraint, heightConstraint);

			if (sizeConstraint.Request.Width == 0)
			{
				int width = widthConstraint;
				sizeConstraint = new SizeRequest(new Size(width, sizeConstraint.Request.Height), new Size(width, sizeConstraint.Minimum.Height));
			}

			return sizeConstraint;
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing && Control != null)
			{
				if (Element != null)
					Element.CheckedChanged -= HandleCheckedChanged;

				Control.SetOnCheckedChangeListener(null);
			}

			base.Dispose(disposing);
		}

		protected override ACheckBox CreateNativeControl()
		{
			return new ACheckBox(Context);
		}

		protected override void OnElementChanged(ElementChangedEventArgs<CheckBox> e)
		{
			base.OnElementChanged(e);

			if (e.OldElement != null)
				e.OldElement.CheckedChanged -= HandleCheckedChanged;

			if (e.NewElement != null)
			{
				if (Control == null)
				{
					var acheckbox = CreateNativeControl();
					acheckbox.SetOnCheckedChangeListener(this);
					SetNativeControl(acheckbox);
				}
				else
				{
					UpdateEnabled(); // Normally set by SetNativeControl, but not when the Control is reused.
				}

				e.NewElement.CheckedChanged += HandleCheckedChanged;
				Control.Checked = e.NewElement.IsChecked;
			}
		}

		protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			base.OnElementPropertyChanged(sender, e);
		}

		void HandleCheckedChanged(object sender, EventArgs e)
		{
			Control.Checked = Element.IsChecked;
		}

		void UpdateEnabled()
		{
			Control.Enabled = Element.IsEnabled;
		}
	}
}