using System;
using System.Collections.Generic;

namespace Xamarin.Forms.PlatformConfiguration.iOSSpecific
{
	using FormsElement = Forms.TimePicker;

	public static class TimePicker
	{
		public static readonly BindableProperty DisabledSelectorActionsProperty = BindableProperty.Create(nameof(DisabledSelectorActions), typeof(List<SelectorAction>), typeof(TimePicker));

		public static List<SelectorAction> GetDisabledSelectorActions(BindableObject element)
		{
			return (List<SelectorAction>)element.GetValue(DisabledSelectorActionsProperty);
		}

		public static void SetDisabledSelectorActions(BindableObject element, List<SelectorAction> value)
		{
			element.SetValue(DisabledSelectorActionsProperty, value);
		}

		public static List<SelectorAction> DisabledSelectorActions(this IPlatformElementConfiguration<iOS, FormsElement> config)
		{
			return GetDisabledSelectorActions(config.Element);
		}

		public static IPlatformElementConfiguration<iOS, FormsElement> SetDisabledSelectorActions(this IPlatformElementConfiguration<iOS, FormsElement> config, List<SelectorAction> value)
		{
			SetDisabledSelectorActions(config.Element, value);
			return config;
		}

		public static IPlatformElementConfiguration<iOS, FormsElement> EnableDisabledSelectorActions(this IPlatformElementConfiguration<iOS, FormsElement> config)
		{
			var disabledSelectorActions = GetDisabledSelectorActions(config.Element);

			if (disabledSelectorActions == null || disabledSelectorActions.Count == 0)
				disabledSelectorActions = new List<SelectorAction> { SelectorAction.All };

			SetDisabledSelectorActions(config.Element, disabledSelectorActions);
			return config;
		}

		public static IPlatformElementConfiguration<iOS, FormsElement> DisableDisabledSelectorActions(this IPlatformElementConfiguration<iOS, FormsElement> config)
		{
			SetDisabledSelectorActions(config.Element, null);
			return config;
		}
	}
}