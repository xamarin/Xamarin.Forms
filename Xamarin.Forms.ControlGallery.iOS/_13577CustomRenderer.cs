using System;
using System.Collections.Generic;
using CoreGraphics;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.ControlGallery.iOS;
using Xamarin.Forms.Controls.Issues;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportRenderer(typeof(NullableDatePicker), typeof(_13577CustomRenderer))]
namespace Xamarin.Forms.ControlGallery.iOS
{
	public class _13577CustomRenderer : DatePickerRenderer
    {
        public NullableDatePicker Entry => Element as NullableDatePicker;

        protected override void OnElementChanged(ElementChangedEventArgs<Xamarin.Forms.DatePicker> e)
        {
            base.OnElementChanged(e);

            if (Control != null)
            {
                AddClearButton();

                // Border style; Remove corner radius and change colors
                Control.BorderStyle = UITextBorderStyle.Line;
                Control.Layer.CornerRadius = 0;
                Control.Layer.BorderWidth = 1;

                // Add padding
                Control.LeftView = new UIView(new CGRect(0, 0, 10, 0));
                Control.LeftViewMode = UITextFieldViewMode.Always;
                Control.RightView = new UIView(new CGRect(0, 0, 10, 0));
                Control.RightViewMode = UITextFieldViewMode.Always;

                if (e.NewElement != null)
                {
                    try
                    {
                        if (UIDevice.CurrentDevice.CheckSystemVersion(14, 0))
                        {
                            UIDatePicker picker = (UIDatePicker)Control.InputView;
                            picker.PreferredDatePickerStyle = UIDatePickerStyle.Wheels;
                        }
                    }
                    catch (Exception)
                    {
                        // Do nothing
                    }
                }
            }
        }

        private void AddClearButton()
        {
            if (Control.InputAccessoryView is UIToolbar originalToolbar && originalToolbar.Items.Length <= 2)
            {
                var clearButton = new UIBarButtonItem("Delete", UIBarButtonItemStyle.Plain, (sender, ev) =>
                {
                    NullableDatePicker baseDatePicker = this.Element as NullableDatePicker;
                    Element.Unfocus();
                    baseDatePicker.CleanDate();
                });

                var newItems = new List<UIBarButtonItem>();
                foreach (var item in originalToolbar.Items)
                {
                    newItems.Add(item);
                }

                newItems.Insert(0, clearButton);

                originalToolbar.Items = newItems.ToArray();
                originalToolbar.SetNeedsDisplay();
            }
        }
    }
}