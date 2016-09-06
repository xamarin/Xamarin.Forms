using System;

namespace Xamarin.Forms
{
    public class CheckedEventArgs : EventArgs
    {
        public CheckedEventArgs(bool value)
        {
            Value = value;
        }

        public bool Value { get; private set; }
    }
}