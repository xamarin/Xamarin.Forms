using System;
using System.ComponentModel;
using Android.Content;
using Android.Support.V7.Widget;
using Android.Widget;
using Xamarin.Forms.Platform.Android.FastRenderers;

namespace Xamarin.Forms.Platform.Android.AppCompat
{
	public class CheckBoxRenderer : ViewRenderer<CheckBox, AppCompatCheckBox>, CompoundButton.IOnCheckedChangeListener
	{
		bool _disposed;
		string _defaultContentDescription;

		public CheckBoxRenderer(Context context) : base(context)
		{
			AutoPackage = false;
		}

		protected override void SetContentDescription()
			=> AutomationPropertiesProvider.SetBasicContentDescription(this, Element, ref _defaultContentDescription);

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

		protected override AppCompatCheckBox CreateNativeControl()
		{
			return new AppCompatCheckBox(Context);
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing && !_disposed)
			{
				_disposed = true;

				if (Element != null)
					Element.CheckedChanged -= HandleCheckedChanged;

				Control.SetOnCheckedChangeListener(null);
			}

			base.Dispose(disposing);
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
					UpdateEnabled(); // Normally set by SetNativeControl, but not when the Control is reused.

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