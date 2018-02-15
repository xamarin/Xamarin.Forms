namespace Xamarin.Forms
{
	public class InputView : View
	{
		public static readonly BindableProperty KeyboardProperty = BindableProperty.Create("Keyboard", typeof(Keyboard), typeof(InputView), Keyboard.Default,
			coerceValue: (o, v) => (Keyboard)v ?? Keyboard.Default);

		public static readonly BindableProperty MaxLengthProperty = BindableProperty.Create(nameof(MaxLength), typeof(int), typeof(int), int.MaxValue);

		public int MaxLength
		{
			get { return (int)GetValue(MaxLengthProperty); }
			set { SetValue(MaxLengthProperty, value); }
		}

		internal InputView()
		{
		}

		public Keyboard Keyboard
		{
			get { return (Keyboard)GetValue(KeyboardProperty); }
			set { SetValue(KeyboardProperty, value); }
		}
	}
}