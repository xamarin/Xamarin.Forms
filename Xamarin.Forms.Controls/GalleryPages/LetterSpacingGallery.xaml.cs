using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Xamarin.Forms.Controls.GalleryPages
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class LetterSpacingGallery : ContentPage
	{
		public LetterSpacingGallery()
		{
			InitializeComponent();
		}

		void Slider_OnValueChanged(object sender, ValueChangedEventArgs e)
		{
			LetterSpacingValue.Text = e.NewValue.ToString();
			Button.LetterSpacing = e.NewValue;
			DatePicker.LetterSpacing = e.NewValue;
			Editor.LetterSpacing = e.NewValue;
			Entry.LetterSpacing = e.NewValue;
			PlaceholderEntry.LetterSpacing = e.NewValue;
			PlaceholderEditor.LetterSpacing = e.NewValue;
			Label.LetterSpacing = e.NewValue;
			Picker.LetterSpacing = e.NewValue;
			SearchBar.LetterSpacing = e.NewValue;
			PlaceholderSearchBar.LetterSpacing = e.NewValue;
			TimePicker.LetterSpacing = e.NewValue;
			Span.LetterSpacing = e.NewValue;
		}
	}
}