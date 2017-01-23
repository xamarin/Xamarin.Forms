using System;
using Xamarin.Forms;

namespace FormsGallery
{
    class StackLayoutDemoPage : ContentPage
    {
        public StackLayoutDemoPage()
        {
            Label header = new Label
            {
                Text = "StackLayout",
				FontSize = 50,
				FontAttributes = FontAttributes.Bold,
                HorizontalOptions = LayoutOptions.Center
            };

            StackLayout stackLayout = new StackLayout
            {
                Spacing = 0,
                VerticalOptions = LayoutOptions.FillAndExpand,
                Children = 
                {
                    new Label
                    {
                        Text = "StackLayout",
                        HorizontalOptions = LayoutOptions.Start
                    },
                    new Label
                    {
                        Text = "stacks its children",
                        HorizontalOptions = LayoutOptions.Center
                    },
                    new Label
                    {
                        Text = "vertically",
                        HorizontalOptions = LayoutOptions.End
                    },
                    new Label
                    {
                        Text = "by default,",
                        HorizontalOptions = LayoutOptions.Center
                    },
                    new Label
                    {
                        Text = "but horizontal placement",
                        HorizontalOptions = LayoutOptions.Start
                    },
                    new Label
                    {
                        Text = "can be controlled with",
                        HorizontalOptions = LayoutOptions.Center
                    },
                    new Label
                    {
                        Text = "the HorizontalOptions property.",
                        HorizontalOptions = LayoutOptions.End
                    },
                    new Label
                    {
                        Text = "An Expand option allows one or more children " +
                               "to occupy the an area within the remaining " +
                               "space of the StackLayout after it's been sized " +
                               "to the height of its parent.",
                        VerticalOptions = LayoutOptions.CenterAndExpand,
                        HorizontalOptions = LayoutOptions.End
                    },
                    new StackLayout
                    {
                        Spacing = 0,
                        Orientation = StackOrientation.Horizontal,
                        Children = 
                        {
                            new Label
                            {
                                Text = "Stacking",
                            },
                            new Label
                            {
                                Text = "can also be",
                                HorizontalOptions = LayoutOptions.CenterAndExpand
                            },
                            new Label
                            {
                                Text = "horizontal.",
                            },
                        }
                    }
                }
            };

            // Build the page.
            this.Content = new StackLayout
            {
                Children = 
                {
                    header,
                    stackLayout
                }
            };
        }
    }
}
