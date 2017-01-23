using System;
using Xamarin.Forms;

namespace FormsGallery
{
    class StepperDemoPage : ContentPage
    {
        Label label;

        public StepperDemoPage()
        {
            Label header = new Label
            {
                Text = "Stepper",
				FontSize = 50,
				FontAttributes = FontAttributes.Bold,
                HorizontalOptions = LayoutOptions.Center
            };

            Stepper stepper = new Stepper
            {
                Minimum = 0,
                Maximum = 10,
                Increment = 0.1,
                HorizontalOptions = LayoutOptions.Center,
                VerticalOptions = LayoutOptions.CenterAndExpand
            };
            stepper.ValueChanged += OnStepperValueChanged;

            label = new Label
            {
                Text = "Stepper value is 0",
				FontSize = Device.GetNamedSize(NamedSize.Large, typeof(Label)),
                HorizontalOptions = LayoutOptions.Center,
                VerticalOptions = LayoutOptions.CenterAndExpand
            };

            // Build the page.
            this.Content = new StackLayout
            {
                Children = 
                {
                    header,
                    stepper,
                    label
                }
            };
        }

        void OnStepperValueChanged(object sender, ValueChangedEventArgs e)
        {
            label.Text = String.Format("Stepper value is {0:F1}", e.NewValue);
        }
    }
}
