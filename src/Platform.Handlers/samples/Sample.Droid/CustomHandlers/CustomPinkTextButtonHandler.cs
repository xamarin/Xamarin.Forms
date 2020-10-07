using Xamarin.Platform;
using Xamarin.Platform.Handlers;

namespace Sample.Droid.CustomHandlers
{
	class CustomPinkTextButtonHandler : ButtonHandler
	{
		public CustomPinkTextButtonHandler() : base()
		{
			ButtonMapper[nameof(IButton.Text)] = MapTextWithColor;
		}

		public static void MapTextWithColor(ButtonHandler handler, IButton button)
		{
			MapText(handler, button);
			(handler.NativeView as AndroidX.AppCompat.Widget.AppCompatButton).SetTextColor(Android.Graphics.Color.Pink);
		}
	}
}