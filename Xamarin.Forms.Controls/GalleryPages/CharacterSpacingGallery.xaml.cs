﻿using System;
using Xamarin.Forms.Xaml;

namespace Xamarin.Forms.Controls.GalleryPages
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class CharacterSpacingGallery : ContentPage
	{
		public CharacterSpacingGallery()
		{
			InitializeComponent();
		}

		void ResetButtonClicked(object sender, EventArgs e)
		{
			slider.Value = 0;
		}

		void Slider_OnValueChanged(object sender, ValueChangedEventArgs e)
		{
			CharacterSpacingValue.Text = e.NewValue.ToString();
			Button.CharacterSpacing = e.NewValue;
			DatePicker.CharacterSpacing = e.NewValue;
			Editor.CharacterSpacing = e.NewValue;
			Entry.CharacterSpacing = e.NewValue;
			PlaceholderEntry.CharacterSpacing = e.NewValue;
			PlaceholderEditor.CharacterSpacing = e.NewValue;
			Label.CharacterSpacing = e.NewValue;
			Picker.CharacterSpacing = e.NewValue;
			SearchBar.CharacterSpacing = e.NewValue;
			PlaceholderSearchBar.CharacterSpacing = e.NewValue;
			TimePicker.CharacterSpacing = e.NewValue;
			Span.CharacterSpacing = e.NewValue;
		}
	}
}