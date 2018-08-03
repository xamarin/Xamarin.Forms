using System;
using System.Collections.Generic;
using System.Text;

namespace Xamarin.Forms.Controls
{
	internal class MediaElementDemoPage : ContentPage
	{
		MediaElement element;
		public MediaElementDemoPage()
		{
			element = new MediaElement();
			element.HorizontalOptions = new LayoutOptions(LayoutAlignment.Fill,true);
			element.VerticalOptions = new LayoutOptions(LayoutAlignment.Fill,true);
			element.AutoPlay = false;

			var button = new Button();
			button.Text = "Play/Pause";
			button.HorizontalOptions = new LayoutOptions(LayoutAlignment.Center, true);
			button.Clicked += Button_Clicked;
			var stack = new StackLayout();
			stack.Padding = new Thickness(10);
			stack.Spacing = 10;
			stack.HorizontalOptions = new LayoutOptions(LayoutAlignment.Fill, false);
			stack.VerticalOptions = new LayoutOptions(LayoutAlignment.Fill, false);
			stack.Children.Add(element);
			stack.Children.Add(button);
			Content = stack;	
		}

		private void Button_Clicked(object sender, EventArgs e)
		{
			if (element.CurrentState == MediaElementState.Playing)
			{
				element.Pause();
			}
			else
			{
				element.Play();
			}
		}

		protected override void OnAppearing()
		{
			base.OnAppearing();

			element.Source = new Uri("http://sec.ch9.ms/ch9/5d93/a1eab4bf-3288-4faf-81c4-294402a85d93/XamarinShow_mid.mp4");
		}
	}
}
