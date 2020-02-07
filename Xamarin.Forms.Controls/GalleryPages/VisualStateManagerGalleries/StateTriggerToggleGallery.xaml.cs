﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Xamarin.Forms.Controls.GalleryPages.VisualStateManagerGalleries
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class StateTriggerToggleGallery : ContentPage
	{
		public StateTriggerToggleGallery()
		{
			InitializeComponent();
			this.BindingContext = new StateTriggerToggleGalleryViewModel();
		}

		public class StateTriggerToggleGalleryViewModel : BindableObject
		{
			bool _toggleState;
			bool _toggleStateInverted;

			public bool ToggleState
			{
				get => _toggleState;
				set
				{
					_toggleState = value;
					OnPropertyChanged(nameof(ToggleState));
					ToggleStateInverted = !value;
				}
			}

			public bool ToggleStateInverted
			{
				get => _toggleStateInverted;
				set
				{
					_toggleStateInverted = value;
					OnPropertyChanged(nameof(ToggleStateInverted));
				}
			}
		}
	}
}