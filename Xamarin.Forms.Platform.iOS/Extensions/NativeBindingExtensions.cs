using System;
using Xamarin.Forms.Internals;
using System.Collections.Generic;

#if __UNIFIED__
using UIKit;
using Foundation;

#else
using MonoTouch.UIKit;
using MonoTouch.Foundation;
#endif

namespace Xamarin.Forms.Platform.iOS
{
	public static class NativeBindingExtensions
	{
		static Dictionary<string, NativeViewPropertyListener> propertyListeners = new Dictionary<string, NativeViewPropertyListener>();

		public static void SetBinding(this UIView self, string propertyName, Binding binding, string eventTargetName = null, Action<object, object> callback = null, Func<object> getter = null)
		{
			var proxy = FormsNativeBindingExtensions.SetNativeBinding(self, propertyName, binding, eventTargetName, callback, getter);
			if (proxy == null || binding.Mode != BindingMode.TwoWay)
				return;
			SubscribeNativeTwoWayBinding(self, proxy);
		}

		static void SubscribeNativeTwoWayBinding(UIView self, BindableProxy proxy)
		{
			if (propertyListeners.ContainsKey(proxy.TargetPropertyName))
				return;

			var propertyListener = new NativeViewPropertyListener(proxy);
			self.AddObserver(propertyListener, new NSString(proxy.TargetPropertyName), 0, IntPtr.Zero);
			propertyListeners.Add(proxy.TargetPropertyName, propertyListener);
		}

		static void UnSubscribeNativeTwoWayBinding(UIView self, BindableProxy proxy)
		{
			if (!propertyListeners.ContainsKey(proxy.TargetPropertyName))
				return;

			var propertyListener = propertyListeners[proxy.TargetPropertyName];
			self.RemoveObserver(propertyListener, proxy.TargetPropertyName);
			propertyListeners.Remove(proxy.TargetPropertyName);
			propertyListener.Dispose();
		}
	}
}
