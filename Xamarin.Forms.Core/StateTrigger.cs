namespace Xamarin.Forms
{
	public class StateTrigger : StateTriggerBase
	{
		public bool IsActive
		{
			get => (bool)GetValue(IsActiveProperty);
			set => SetValue(IsActiveProperty, value);
		}

		public static readonly BindableProperty IsActiveProperty =
			BindableProperty.Create(nameof(IsActive), typeof(bool), typeof(StateTrigger), default(bool),
				propertyChanged: OnIsActiveChanged);

		static void OnIsActiveChanged(BindableObject bindable, object oldvalue, object newvalue)
		{
			if (newvalue is bool b)
			{
				((StateTrigger)bindable).SetActive(b);
			}
		}
	}
}