using System;
using System.ComponentModel;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

#if WINDOWS_UWP
namespace Xamarin.Forms.Platform.UWP
#else
namespace Xamarin.Forms.Platform.WinRT
#endif
{
	class NativePropertyListener : DependencyObject, IDisposable
	{
		readonly BindableProxy _bindableProxy;
		readonly DependencyObject _target;
	
		public static readonly DependencyProperty TargetPropertyValueProperty = DependencyProperty.Register(nameof(TargetPropertyValue), typeof(object), typeof(NativePropertyListener), new PropertyMetadata(null, OnNativePropertyChanged));

		public NativePropertyListener(DependencyObject target, BindableProxy bindableProxy)
		{
			_target = target;
			_bindableProxy = bindableProxy;
			BindingOperations.SetBinding(this, TargetPropertyValueProperty, new Windows.UI.Xaml.Data.Binding() { Source = this._target, Path = new PropertyPath(_bindableProxy.TargetPropertyName), Mode = Windows.UI.Xaml.Data.BindingMode.OneWay });
		}

		public void Dispose()
		{
			ClearValue(TargetPropertyValueProperty);
		}

		public object TargetPropertyValue
		{
			get
			{
				return GetValue(TargetPropertyValueProperty);
			}
		}

		static void OnNativePropertyChanged(object sender, DependencyPropertyChangedEventArgs args)
		{
			NativePropertyListener source = (NativePropertyListener)sender;
			source?._bindableProxy.OnTargetPropertyChanged();
		}		
	}
}
