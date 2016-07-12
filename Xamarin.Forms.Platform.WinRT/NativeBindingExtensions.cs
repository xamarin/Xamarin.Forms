using System;
using System.Collections.Generic;
using Windows.UI.Xaml;
using Xamarin.Forms.Internals;

#if WINDOWS_UWP
namespace Xamarin.Forms.Platform.UWP
#else
namespace Xamarin.Forms.Platform.WinRT
#endif
{
	public static class NativeBindingExtensions
	{
		static Dictionary<string, NativePropertyListener> _watchers = new Dictionary<string, NativePropertyListener>();
		public static void SetBinding(this FrameworkElement self, string propertyName, Binding binding, string eventTargetName = null, Action<object, object> callback = null, Func<object> getter = null)
		{
			var proxy = FormsNativeBindingExtensions.SetNativeBinding(self, propertyName, binding, eventTargetName, callback, getter);
			if (proxy == null || binding.Mode != BindingMode.TwoWay)
				return;
			SubscribeNativeTwoWayBinding(self, proxy);
		}

		static void SubscribeNativeTwoWayBinding(FrameworkElement control, BindableProxy proxy)
		{
			var key = proxy.TargetPropertyName;
			if (_watchers.ContainsKey(key))
				return;
			var watcher = new NativePropertyListener(control, proxy);
			_watchers.Add(key, watcher);
		}

		static void UnSubscribeNativeTwoWayBinding(FrameworkElement control, BindableProxy proxy)
		{
			var key = proxy.TargetPropertyName;
			if (!_watchers.ContainsKey(key))
				return;
			_watchers[key].Dispose();
			_watchers.Remove(proxy.TargetPropertyName);
		}	
	}
}
