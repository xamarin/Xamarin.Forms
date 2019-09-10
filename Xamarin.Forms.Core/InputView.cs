namespace Xamarin.Forms
{
	public class InputView : View, IPlaceholderElement, ITextElement
	{
		public static readonly BindableProperty KeyboardProperty = BindableProperty.Create("Keyboard", typeof(Keyboard), typeof(InputView), Keyboard.Default,
			coerceValue: (o, v) => (Keyboard)v ?? Keyboard.Default);
		public static readonly BindableProperty IsSpellCheckEnabledProperty = BindableProperty.Create("IsSpellCheckEnabled", typeof(bool), typeof(InputView), true);

		public static readonly BindableProperty MaxLengthProperty = BindableProperty.Create(nameof(MaxLength), typeof(int), typeof(int), int.MaxValue);

		public static readonly BindableProperty IsReadOnlyProperty = BindableProperty.Create(nameof(IsReadOnly), typeof(bool), typeof(InputView), false);

		public static readonly BindableProperty PlaceholderProperty = PlaceholderElement.PlaceholderProperty;

		public static readonly BindableProperty PlaceholderColorProperty = PlaceholderElement.PlaceholderColorProperty;

		public static readonly BindableProperty TextColorProperty = TextElement.TextColorProperty;

		public static readonly BindableProperty CharacterSpacingProperty = TextElement.CharacterSpacingProperty;

		public int MaxLength
		{
			get => (int)GetValue(MaxLengthProperty);
			set => SetValue(MaxLengthProperty, value);
		}

		internal InputView()
		{
		}

		public Keyboard Keyboard
		{
			get => (Keyboard)GetValue(KeyboardProperty);
			set => SetValue(KeyboardProperty, value);
		}

		public bool IsSpellCheckEnabled
		{
			get => (bool)GetValue(IsSpellCheckEnabledProperty);
			set => SetValue(IsSpellCheckEnabledProperty, value);
		}

		public bool IsReadOnly
		{
			get => (bool)GetValue(IsReadOnlyProperty);
			set => SetValue(IsReadOnlyProperty, value);
		}

		public string Placeholder
		{
			get => (string)GetValue(PlaceholderProperty);
			set => SetValue(PlaceholderProperty, value);
		}

		public Color PlaceholderColor
		{
			get => (Color)GetValue(PlaceholderColorProperty);
			set => SetValue(PlaceholderColorProperty, value);
		}

		public Color TextColor
		{
			get => (Color)GetValue(TextColorProperty);
			set => SetValue(TextColorProperty, value);
		}

		public double CharacterSpacing
		{
			get => (double)GetValue(CharacterSpacingProperty);
			set => SetValue(CharacterSpacingProperty, value);
		}

		void ITextElement.OnTextColorPropertyChanged(Color oldValue, Color newValue)
		{
		}

		void ITextElement.OnCharacterSpacingPropertyChanged(double oldValue, double newValue)
		{
			InvalidateMeasure();
		}
	}

	public enum ClearButtonVisibility
	{
		Never,
		WhileEditing
	}
}