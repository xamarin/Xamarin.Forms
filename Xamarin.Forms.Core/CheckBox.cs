using System;
using Xamarin.Forms.Platform;

namespace Xamarin.Forms
{
	[RenderWith(typeof(_CheckBoxRenderer))]
	public class CheckBox : View, IElementConfiguration<CheckBox>
	{
		public const string IsCheckedVisualState = "IsChecked";

		public static readonly BindableProperty IsCheckedProperty = BindableProperty.Create(nameof(IsChecked), typeof(bool), typeof(CheckBox), false, propertyChanged: (bindable, oldValue, newValue) =>
		{
			((CheckBox)bindable).CheckedChanged?.Invoke(bindable, new CheckedChangedEventArgs((bool)newValue));
			((CheckBox)bindable).ChangeVisualState();
		}, defaultBindingMode: BindingMode.TwoWay);

		public static readonly BindableProperty TintColorProperty = BindableProperty.Create(nameof(TintColor), typeof(Color), typeof(CheckBox), Color.Default);

		public Color TintColor
		{
			get { return (Color)GetValue(TintColorProperty); }
			set { SetValue(TintColorProperty, value); }
		}
	
		readonly Lazy<PlatformConfigurationRegistry<CheckBox>> _platformConfigurationRegistry;

		public CheckBox()
		{
			_platformConfigurationRegistry = new Lazy<PlatformConfigurationRegistry<CheckBox>>(() => new PlatformConfigurationRegistry<CheckBox>(this));
		}

		public bool IsChecked
		{
			get { return (bool)GetValue(IsCheckedProperty); }
			set
			{
				SetValue(IsCheckedProperty, value);
				ChangeVisualState();
			}
		}

		protected internal override void ChangeVisualState()
		{
			if (IsEnabled && IsChecked)
			{
				VisualStateManager.GoToState(this, IsCheckedVisualState);
			}
			else
			{
				base.ChangeVisualState();
			}
		}

		public event EventHandler<CheckedChangedEventArgs> CheckedChanged;

		public IPlatformElementConfiguration<T, CheckBox> On<T>() where T : IConfigPlatform
		{
			return _platformConfigurationRegistry.Value.On<T>();
		}
	}
}