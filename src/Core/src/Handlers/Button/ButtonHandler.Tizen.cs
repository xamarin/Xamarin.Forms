using System;
using Tizen.UIExtensions.ElmSharp;

namespace Microsoft.Maui.Handlers
{
	public partial class ButtonHandler : AbstractViewHandler<IButton, Button>
	{
		protected override Button CreateNativeView()
		{
			if (NativeParent == null)
			{
				throw new InvalidOperationException($"{nameof(NativeParent)} cannot be null");
			}
			return new Button(NativeParent);
		}

		protected override void ConnectHandler(Button nativeView)
		{
			nativeView.Released += OnButtonClicked;
			nativeView.Clicked += OnButtonReleased;
			nativeView.Pressed += OnButtonPressed;

			base.ConnectHandler(nativeView);
		}

		protected override void DisconnectHandler(Button nativeView)
		{
			nativeView.Released -= OnButtonClicked;
			nativeView.Clicked -= OnButtonReleased;
			nativeView.Pressed -= OnButtonPressed;

			base.DisconnectHandler(nativeView);
		}

		protected override void SetupDefaults(Button nativeView)
		{
			base.SetupDefaults(nativeView);
		}

		public static void MapText(ButtonHandler handler, IButton button)
		{
			handler.TypedNativeView?.UpdateText(button);
		}

		public static void MapTextColor(ButtonHandler handler, IButton button)
		{
			handler.TypedNativeView?.UpdateTextColor(button);
		}

		void OnButtonClicked(object? sender, EventArgs e)
		{
			VirtualView?.Clicked();
		}

		void OnButtonReleased(object? sender, EventArgs e)
		{
			VirtualView?.Released();
		}

		void OnButtonPressed(object? sender, EventArgs e)
		{
			VirtualView?.Pressed();
		}
	}
}