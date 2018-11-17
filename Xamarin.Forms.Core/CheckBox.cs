using System;
using Xamarin.Forms.Platform;

namespace Xamarin.Forms
{
	[RenderWith(typeof(_CheckBoxRenderer))]
	public class CheckBox : View, IElementConfiguration<CheckBox>
	{
		public static readonly BindableProperty IsCheckedProperty = BindableProperty.Create(nameof(IsChecked), typeof(bool), typeof(CheckBox), false, propertyChanged: (bindable, oldValue, newValue) =>
		{
			((CheckBox)bindable).CheckedChanged?.Invoke(bindable, new CheckedChangedEventArgs((bool)newValue));
		}, defaultBindingMode: BindingMode.TwoWay);

		public static readonly BindableProperty CheckedColorProperty = BindableProperty.Create(nameof(CheckedColor), typeof(Color), typeof(CheckBox), Color.Default);

		public Color CheckedColor
		{
			get { return (Color)GetValue(CheckedColorProperty); }
			set { SetValue(CheckedColorProperty, value); }
		}

		public static readonly BindableProperty UnCheckedColorProperty = BindableProperty.Create(nameof(UnCheckedColor), typeof(Color), typeof(CheckBox), Color.Default);

		public Color UnCheckedColor
		{
			get { return (Color)GetValue(UnCheckedColorProperty); }
			set { SetValue(UnCheckedColorProperty, value); }
		}


		readonly Lazy<PlatformConfigurationRegistry<CheckBox>> _platformConfigurationRegistry;

		public CheckBox()
		{
			_platformConfigurationRegistry = new Lazy<PlatformConfigurationRegistry<CheckBox>>(() => new PlatformConfigurationRegistry<CheckBox>(this));
		}

		public bool IsChecked
		{
			get { return (bool)GetValue(IsCheckedProperty); }
			set { SetValue(IsCheckedProperty, value); }
		}

		public event EventHandler<CheckedChangedEventArgs> CheckedChanged;

		public IPlatformElementConfiguration<T, CheckBox> On<T>() where T : IConfigPlatform
		{
			return _platformConfigurationRegistry.Value.On<T>();
		}
	}
}