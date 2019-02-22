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

		public static readonly DependencyProperty LetterSpacingProperty = DependencyProperty.Register(nameof(LetterSpacing), typeof(int), typeof(FormsButton),
			new PropertyMetadata(default(int), OnLetterSpacingChanged));

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

		public int LetterSpacing
		{
			get
			{
				return (int)GetValue(LetterSpacingProperty);
			}
			set
			{
				SetValue(LetterSpacingProperty, value);
			}
		}

		protected override void OnApplyTemplate()
		{
			base.OnApplyTemplate();

			_contentPresenter = GetTemplateChild("ContentPresenter") as WContentPresenter;

			UpdateBackgroundColor();
			UpdateBorderRadius();
		}

		static void OnBackgroundColorChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			((FormsButton)d).UpdateBackgroundColor();
		}

		static void OnBorderRadiusChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			((FormsButton)d).UpdateBorderRadius();
		}

		static void OnLetterSpacingChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			((FormsButton)d).LetterSpacingChanged();
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

		void LetterSpacingChanged()
		{
			CharacterSpacing = LetterSpacing;
			if (_contentPresenter != null)
				_contentPresenter.CharacterSpacing = LetterSpacing;
		}
	}
}