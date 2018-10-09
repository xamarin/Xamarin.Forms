using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;

using WContentPresenter = Windows.UI.Xaml.Controls.ContentPresenter;

namespace Xamarin.Forms.Platform.UWP
{
	public class FormsButton : Windows.UI.Xaml.Controls.Button
	{
		public static readonly DependencyProperty BorderRadiusProperty = DependencyProperty.Register(nameof(BorderRadius), typeof(int), typeof(FormsButton),
			new PropertyMetadata(default(int), OnBorderRadiusChanged));

		public static readonly DependencyProperty BackgroundColorProperty = DependencyProperty.Register(nameof(BackgroundColor), typeof(Brush), typeof(FormsButton),
			new PropertyMetadata(default(Brush), OnBackgroundColorChanged));

		public static readonly DependencyProperty TextWrappingProperty = DependencyProperty.Register(
			nameof(TextWrapping),
			typeof(TextWrapping),
			typeof(FormsButton),
			new PropertyMetadata(TextWrapping.NoWrap, OnTextWrappingChanged));

		WContentPresenter _contentPresenter;

		public Brush BackgroundColor
		{
			get
			{
				return (Brush)GetValue(BackgroundColorProperty);
			}
			set
			{
				SetValue(BackgroundColorProperty, value);
			}
		}

		public int BorderRadius
		{
			get
			{
				return (int)GetValue(BorderRadiusProperty);
			}
			set
			{
				SetValue(BorderRadiusProperty, value);
			}
		}

		public TextWrapping TextWrapping
		{
			get { return (TextWrapping)GetValue(TextWrappingProperty); }
			set { SetValue(TextWrappingProperty, value); }
		}

		protected override void OnApplyTemplate()
		{
			base.OnApplyTemplate();

			_contentPresenter = GetTemplateChild("ContentPresenter") as WContentPresenter;

			UpdateBackgroundColor();
			UpdateBorderRadius();
			UpdateTextWrapping();
		}

		static void OnBackgroundColorChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			((FormsButton)d).UpdateBackgroundColor();
		}

		static void OnBorderRadiusChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			((FormsButton)d).UpdateBorderRadius();
		}

		static void OnTextWrappingChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			((FormsButton)d).UpdateTextWrapping();
		}

		void UpdateBackgroundColor()
		{
			if (BackgroundColor == null)
				BackgroundColor = Background;

			if (_contentPresenter != null)
				_contentPresenter.Background = BackgroundColor;
			Background = Color.Transparent.ToBrush();
		}

		void UpdateBorderRadius()
		{

			if (_contentPresenter != null)
				_contentPresenter.CornerRadius = new Windows.UI.Xaml.CornerRadius(BorderRadius);
		}

		void UpdateTextWrapping()
		{
			if (_contentPresenter != null)
			{
				_contentPresenter.TextWrapping = TextWrapping;
			}
		}
	}
}