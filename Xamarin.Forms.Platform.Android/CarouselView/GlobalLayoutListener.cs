using System;
using Android.App;
using Android.Content;
using Android.Views;
using Android.Views.InputMethods;
using Object = Java.Lang.Object;

// PR Fix for entry focus bug #242
namespace Xamarin.Forms.Platform.Android
{
    internal class GlobalLayoutListener : Object, ViewTreeObserver.IOnGlobalLayoutListener
    {
        static InputMethodManager _inputManager;
        readonly SoftKeyboardService _softwareKeyboardService;
        Activity _activity;

        private static void ObtainInputManager(Activity activity)
        {
            _inputManager = (InputMethodManager)activity.GetSystemService(Context.InputMethodService);
        }

        public GlobalLayoutListener(SoftKeyboardService softwareKeyboardService, Activity activity)
        {
            _softwareKeyboardService = softwareKeyboardService;
            _activity = activity;
            ObtainInputManager(activity);
        }

        public void OnGlobalLayout()
        {
            if (_inputManager.Handle == IntPtr.Zero)
            {
                ObtainInputManager(_activity);
            }

            _softwareKeyboardService.InvokeVisibilityChanged(new SoftKeyboardEventArgs(_inputManager.IsAcceptingText));
        }
    }
}