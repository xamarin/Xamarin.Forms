﻿using Windows.UI.Xaml;
using WBrush = Windows.UI.Xaml.Media.Brush;
using WindowsCheckbox = Windows.UI.Xaml.Controls.CheckBox;

namespace Xamarin.Forms.Platform.UWP
{
	public class FormsCheckBox : WindowsCheckbox
	{
		public static readonly DependencyProperty TintBrushProperty =
			DependencyProperty.Register(nameof(TintBrush), typeof(WBrush), typeof(FormsCheckBox),
				new PropertyMetadata(default(WBrush), OnTintBrushPropertyChanged));

		static void OnTintBrushPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			var checkBox =  (FormsCheckBox)d;

			if(checkBox.IsChecked == false)
			{
				checkBox.DefaultFillBrush = Color.Transparent.ToBrush();
			}
			else
			{
				checkBox.DefaultFillBrush = (WBrush)e.NewValue;
			}
		}

		public static readonly DependencyProperty DefaultFillBrushProperty =
			DependencyProperty.Register(nameof(DefaultFillBrush), typeof(WBrush), typeof(FormsCheckBox),
				new PropertyMetadata(default(WBrush)));

		public FormsCheckBox()
		{
			
		}

		public WBrush TintBrush
		{
			get { return (WBrush)GetValue(TintBrushProperty); }
			set { SetValue(TintBrushProperty, value);  }
		}

		public WBrush DefaultFillBrush
		{
			get { return (WBrush)GetValue(DefaultFillBrushProperty); }
			set { SetValue(DefaultFillBrushProperty, value); }
		}
	}
}
