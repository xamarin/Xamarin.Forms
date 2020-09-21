using System;

namespace Xamarin.Platform.Handlers
{
	public partial class ButtonHandler
	{
		public static PropertyMapper<IButton, ButtonHandler> ButtonMapper = new PropertyMapper<IButton, ButtonHandler>(ViewHandler.ViewMapper)
		{
			[nameof(IButton.Text)] = MapText,
			[nameof(IButton.Color)] = MapColor,
			Actions = {
				["DemoAction"] = DemoAction
			}
		};

		public static void MapColor(ButtonHandler handler, IButton button)
		{
			handler.TypedNativeView.UpdateColor(button);
		}

		public static void DemoAction(IViewHandler arg1, IButton arg2)
		{
		}

		public ButtonHandler() : base(ButtonMapper)
		{

		}

		public ButtonHandler(PropertyMapper mapper) : base(mapper ?? ButtonMapper)
		{

		}
	}
}