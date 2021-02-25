namespace Microsoft.Maui.Handlers
{
	public partial class ButtonHandler
	{
		public static PropertyMapper<IButton, ButtonHandler> ButtonMapper = new PropertyMapper<IButton, ButtonHandler>(ViewHandler.ViewMapper)
		{
			[nameof(IButton.Text)] = MapText,
			[nameof(IButton.TextColor)] = MapTextColor,
			[nameof(IButton.CornerRadius)] = MapCornerRadius
		};

		public ButtonHandler() : base(ButtonMapper)
		{

		}

		public ButtonHandler(PropertyMapper mapper) : base(mapper ?? ButtonMapper)
		{

		}
	}
}
