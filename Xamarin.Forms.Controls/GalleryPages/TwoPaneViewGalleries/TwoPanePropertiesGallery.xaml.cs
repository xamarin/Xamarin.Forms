using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Xamarin.Forms.Controls.GalleryPages.TwoPaneViewGalleries
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class TwoPanePropertiesGallery : ContentPage
	{
		public TwoPanePropertiesGallery()
		{
			InitializeComponent();
			twoPaneView.ModeChanged += OnModeChanged;
			Pane1Length.ValueChanged += PaneLengthChanged;
			Pane2Length.ValueChanged += PaneLengthChanged;
		}

		void PaneLengthChanged(object sender, ValueChangedEventArgs e)
		{
			twoPaneView.Pane1Length = new GridLength(Pane1Length.Value, GridUnitType.Star);
			twoPaneView.Pane2Length = new GridLength(Pane2Length.Value, GridUnitType.Star);
		}

		void OnModeChanged(object sender, EventArgs e)
		{
			Setup(Width, Height);
		}

		protected override void OnAppearing()
		{
			base.OnAppearing();
			Setup(Width, Height);
		}

		void Setup(double width, double height)
		{
			if (width <= 0 || height <= 0)
				return;


			MinTallModeHeight.Maximum = height;
			MinWideModeWidth.Maximum = width;
		}

		protected override void OnSizeAllocated(double width, double height)
		{
			base.OnSizeAllocated(width, height);
			Setup(width, height);
		}

		void OnSwitchPanePriority(object sender, EventArgs e)
		{
			if (twoPaneView.PanePriority == DualScreen.TwoPaneViewPriority.Pane1)
				twoPaneView.PanePriority = DualScreen.TwoPaneViewPriority.Pane2;
			else
				twoPaneView.PanePriority = DualScreen.TwoPaneViewPriority.Pane1;
		}
	}
}