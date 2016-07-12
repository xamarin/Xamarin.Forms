using System;
using System.Collections.Generic;

namespace Xamarin.Forms.Internals
{
	static class FormsNativeBindingExtensions
	{
		internal static void SetNativeBindingContext(List<object> views, object newContext)
		{
			foreach (var view in views)
			{
				if (!NativeBindingPool.ContainsKey(view))
					continue;

				foreach (var item in NativeBindingPool[view])
				{
					if (newContext.Equals(item.Key.BindingContext))
						continue;
					item.Key.BindingContext = newContext;
				}
			}
		}

		internal static void SetBinding(object view, NativeBinding binding, BindableProxy bindableProxy)
		{
			if (NativeBindingPool.ContainsKey(view))
				NativeBindingPool[view].Add(bindableProxy, binding);
			else
				NativeBindingPool.Add(view, new Dictionary<BindableProxy, NativeBinding> { { bindableProxy, binding } });
		}

		internal static BindableProxy SetNativeBinding(object view, string propertyName, Binding binding, string eventTargetName = null, Action<object, object> callback = null, Func<object> getter = null)
		{
			var bindableProxy = new BindableProxy(view, propertyName, callback, getter, eventTargetName);
			var nativeBinding = new NativeBinding(binding.Path, binding.Mode, binding.Converter, binding.ConverterParameter, binding.StringFormat, binding.Source, eventTargetName);

			if (NativeBindingPool.ContainsKey(view))
			{
				if (NativeBindingPool[view].ContainsValue(nativeBinding))
					return null;
				NativeBindingPool[view].Add(bindableProxy, nativeBinding);
			}
			else
			{
				NativeBindingPool.Add(view, new Dictionary<BindableProxy, NativeBinding> { { bindableProxy, nativeBinding } });
			}

			bindableProxy.SetBinding(bindableProxy.Property, nativeBinding);

			return bindableProxy;
		}

		internal static void ClearBindings(object nativeView)
		{
			if (nativeView == null || !NativeBindingPool.ContainsKey(nativeView))
				return;

			foreach (var item in NativeBindingPool[nativeView])
			{
				item.Value.Unapply();
				item.Key.Dispose();
			}

			NativeBindingPool.Remove(nativeView);
		}

		internal static Dictionary<object, Dictionary<BindableProxy, NativeBinding>> NativeBindingPool = new Dictionary<object, Dictionary<BindableProxy, NativeBinding>>();
	}
}

