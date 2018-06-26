using System;

// PR Fix for entry focus bug #242
namespace Xamarin.Forms.Platform.Android
{
    public class SoftKeyboardEventArgs : EventArgs
    {
        public SoftKeyboardEventArgs(bool isVisible)
        {
            IsVisible = isVisible;
        }

        public bool IsVisible { get; private set; }
    }
}