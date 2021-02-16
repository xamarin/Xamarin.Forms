namespace Xamarin.Platform.Handlers
{
	public partial class ButtonHandler
	{
		public static PropertyMapper<IButton, ButtonHandler> ButtonMapper = new PropertyMapper<IButton, ButtonHandler>(ViewHandler.ViewMapper)
		{
#if MONOANDROID
			[nameof(IButton.BackgroundColor)] = MapBackgroundColor,
#endif
			[nameof(IButton.Text)] = MapText,
			[nameof(IButton.TextColor)] = MapColor,
			[nameof(IButton.Font)] = MapFont,
			[nameof(IButton.CharacterSpacing)] = MapCharacterSpacing,
			[nameof(IButton.CornerRadius)] = MapCornerRadius,
			[nameof(IButton.BorderColor)] = MapBorderColor,
			[nameof(IButton.BorderWidth)] = MapBorderWidth,
			[nameof(IButton.FontSize)] = MapFont,
			[nameof(IButton.FontAttributes)] = MapFont,
			[nameof(IButton.ContentLayout)] = MapContentLayout,
			[nameof(IButton.Padding)] = MapPadding
		};
				
		public ButtonHandler() : base(ButtonMapper)
		{

		}

		public ButtonHandler(PropertyMapper mapper) : base(mapper ?? ButtonMapper)
		{

		}
	}
}