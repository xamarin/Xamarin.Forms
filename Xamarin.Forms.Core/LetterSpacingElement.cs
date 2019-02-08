namespace Xamarin.Forms
{
	static class LetterSpacingElement
	{
		public static readonly BindableProperty LetterSpacingProperty =
			BindableProperty.Create(nameof(ILetterSpacingElement.LetterSpacing), typeof(double), typeof(ILetterSpacingElement), 0.0d);

	}
}