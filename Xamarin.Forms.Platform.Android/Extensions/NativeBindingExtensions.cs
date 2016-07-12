using System;
using System.Collections.Generic;
using Java.Beans;
using Xamarin.Forms.Internals;
using AView = Android.Views.View;

namespace Xamarin.Forms.Platform.Android
{
	public static class NativeBindingExtensions
	{
		static Dictionary<AView, PropertyChangeSupport> viewsWithPropertyChangeSupport = new Dictionary<AView, PropertyChangeSupport>();

		public static void SetBinding(this AView self, string propertyName, Binding binding, string eventTargetName = null, Action<object, object> callback = null, Func<object> getter = null)
		{
			var proxy = FormsNativeBindingExtensions.SetNativeBinding(self, propertyName, binding, eventTargetName, callback, getter);
			if (proxy == null || binding.Mode != BindingMode.TwoWay)
				return;
			SubscribeNativeTwoWayBinding(self, propertyName, proxy);
		}

		static void SubscribeNativeTwoWayBinding(AView self, string propertyName, BindableProxy proxy)
		{
			if (!viewsWithPropertyChangeSupport.ContainsKey(self))
				viewsWithPropertyChangeSupport.Add(self, new PropertyChangeSupport(self));

			if (viewsWithPropertyChangeSupport[self].HasListeners(propertyName))
				return;

			viewsWithPropertyChangeSupport[self].AddPropertyChangeListener(propertyName, new NativeViewPropertyListener(proxy));
		}

		static void UnSubscribeNativeTwoWayBinding(AView view, BindableProxy proxy)
		{
			if (!viewsWithPropertyChangeSupport.ContainsKey(view))
				return;

			var listeners = viewsWithPropertyChangeSupport[view].GetPropertyChangeListeners(proxy.TargetPropertyName);
			foreach (var nativeViewPropertyListener in listeners)
			{
				viewsWithPropertyChangeSupport[view].RemovePropertyChangeListener(nativeViewPropertyListener);
				nativeViewPropertyListener.Dispose();
			}

			//if there's no properties being listen we can remove this
			if (viewsWithPropertyChangeSupport[view].GetPropertyChangeListeners().Length > 0)
				return;

			viewsWithPropertyChangeSupport[view].Dispose();
			viewsWithPropertyChangeSupport.Remove(view);
		}
	}
}

