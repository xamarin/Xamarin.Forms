using AndroidX.AppCompat.Widget;
using AView = Android.Views.View;

namespace Xamarin.Platform.Handlers
{
	public partial class ButtonHandler : AbstractViewHandler<IButton, AppCompatButton>
	{
		ButtonClickListener ClickListener { get; } = new ButtonClickListener();
		ButtonTouchListener TouchListener { get; } = new ButtonTouchListener();

		protected override AppCompatButton CreateNativeView()
		{
			AppCompatButton nativeButton = new AppCompatButton(Context)
			{
				SoundEffectsEnabled = false
			};

			return nativeButton;
		}

		protected override void ConnectHandler(AppCompatButton nativeView)
		{
			ClickListener.Handler = this;
			nativeView.SetOnClickListener(ClickListener);

			TouchListener.Handler = this;
			nativeView.SetOnTouchListener(TouchListener);

			base.ConnectHandler(nativeView);
		}

		protected override void DisconnectHandler(AppCompatButton nativeView)
		{
			ClickListener.Handler = null;
			nativeView.SetOnClickListener(null);

			TouchListener.Handler = null;
			nativeView.SetOnTouchListener(null);

			base.DisconnectHandler(nativeView);
		}

		public static void MapBackgroundColor(ButtonHandler handler, IButton button)
		{
			ViewHandler.CheckParameters(handler, button);

			handler.TypedNativeView?.UpdateBackgroundColor(button);
		}

		public static void MapColor(ButtonHandler handler, IButton button)
		{
			ViewHandler.CheckParameters(handler, button);

			handler.TypedNativeView?.UpdateTextColor(button);
		}

		public static void MapText(ButtonHandler handler, IButton button)
		{
			ViewHandler.CheckParameters(handler, button);

			handler.TypedNativeView?.UpdateText(button);
		}

		public static void MapFont(ButtonHandler handler, IButton button)
		{
			ViewHandler.CheckParameters(handler, button);

			handler.TypedNativeView?.UpdateFont(button);
		}

		public static void MapCharacterSpacing(ButtonHandler handler, IButton button)
		{
			ViewHandler.CheckParameters(handler, button);

			handler.TypedNativeView?.UpdateCharacterSpacing(button);
		}

		public static void MapCornerRadius(ButtonHandler handler, IButton button)
		{
			ViewHandler.CheckParameters(handler, button);

			handler.TypedNativeView?.UpdateCornerRadius(button);
		}

		public static void MapBorderColor(ButtonHandler handler, IButton button)
		{
			ViewHandler.CheckParameters(handler, button);

			handler.TypedNativeView?.UpdateBorderColor(button);
		}

		public static void MapBorderWidth(ButtonHandler handler, IButton button)
		{
			ViewHandler.CheckParameters(handler, button);

			handler.TypedNativeView?.UpdateBorderWidth(button);
		}

		public static void MapContentLayout(ButtonHandler handler, IButton button)
		{
			ViewHandler.CheckParameters(handler, button);

			handler.TypedNativeView?.UpdateContentLayout(button);
		}

		public static void MapPadding(ButtonHandler handler, IButton button)
		{
			ViewHandler.CheckParameters(handler, button);

			handler.TypedNativeView?.UpdatePadding(button);
		}

		public class ButtonClickListener : Java.Lang.Object, AView.IOnClickListener
		{
			public ButtonHandler? Handler { get; set; }

			public void OnClick(AView? v)
			{
				ButtonManager.OnClick(Handler?.VirtualView, v);
			}
		}

		public class ButtonTouchListener : Java.Lang.Object, AView.IOnTouchListener
		{
			public ButtonHandler? Handler { get; set; }

			public bool OnTouch(AView? v, global::Android.Views.MotionEvent? e)
			{
				ButtonManager.OnTouch(Handler?.VirtualView, v, e);
				return true;
			}
		}
	}
}