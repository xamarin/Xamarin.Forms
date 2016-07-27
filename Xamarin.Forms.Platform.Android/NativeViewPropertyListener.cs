using Java.Beans;

namespace Xamarin.Forms.Platform.Android
{
	class NativeViewPropertyListener : Java.Lang.Object, IPropertyChangeListener
	{
		readonly BindableProxy _bindableProxy;

		public NativeViewPropertyListener(BindableProxy nativeViewBindableController)
		{
			_bindableProxy = nativeViewBindableController;
		}

		public void PropertyChange(PropertyChangeEvent e)
		{
			if (e.PropertyName == _bindableProxy.TargetPropertyName)
				_bindableProxy.OnTargetPropertyChanged();
		}
	}
}

