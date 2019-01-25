namespace Xamarin.Forms
{
	static class LetterSpacingElement
	{
		public static readonly BindableProperty LetterSpacingProperty =
			BindableProperty.Create("LetterSpacing", typeof(double), typeof(ILetterSpacingElement), 0.0d,
				propertyChanged: OnLetterSpacingChanged);

		static void OnLetterSpacingChanged(BindableObject bindable, object oldValue, object newValue)
		{
			((ILetterSpacingElement)bindable).OnLetterSpacingChanged((double)oldValue, (double)newValue);
		}
	}
}