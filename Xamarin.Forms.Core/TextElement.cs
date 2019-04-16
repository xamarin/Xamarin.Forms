namespace Xamarin.Forms
{
	static class TextElement
	{
		public static readonly BindableProperty TextColorProperty =
			BindableProperty.Create(nameof(ITextElement.TextColor), typeof(Color), typeof(ITextElement), Color.Default,
									propertyChanged: OnTextColorPropertyChanged);

		public static readonly BindableProperty LetterSpacingProperty =
			BindableProperty.Create(nameof(ITextElement.LetterSpacing), typeof(double), typeof(ITextElement), 0.0d,
				propertyChanged: OnLetterSpacingPropertyChanged);

		static void OnTextColorPropertyChanged(BindableObject bindable, object oldValue, object newValue)
		{
			((ITextElement)bindable).OnTextColorPropertyChanged((Color)oldValue, (Color)newValue);
		}

		static void OnLetterSpacingPropertyChanged(BindableObject bindable, object oldValue, object newValue)
		{
			((ITextElement)bindable).OnLetterSpacingPropertyChanged((double)oldValue, (double)newValue);
		}

	}
}