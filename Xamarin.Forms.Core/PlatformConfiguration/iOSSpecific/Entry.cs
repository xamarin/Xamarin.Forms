using System;
using System.Collections.Generic;

namespace Xamarin.Forms.PlatformConfiguration.iOSSpecific
{
	using FormsElement = Forms.Entry;

	public static class Entry
	{
		public static readonly BindableProperty AdjustsFontSizeToFitWidthProperty =
			BindableProperty.Create("AdjustsFontSizeToFitWidth", typeof(bool),
			typeof(Entry), false);

		public static bool GetAdjustsFontSizeToFitWidth(BindableObject element)
		{
			return (bool)element.GetValue(AdjustsFontSizeToFitWidthProperty);
		}

		public static void SetAdjustsFontSizeToFitWidth(BindableObject element, bool value)
		{
			element.SetValue(AdjustsFontSizeToFitWidthProperty, value);
		}

		public static bool AdjustsFontSizeToFitWidth(this IPlatformElementConfiguration<iOS, FormsElement> config)
		{
			return GetAdjustsFontSizeToFitWidth(config.Element);
		}

		public static IPlatformElementConfiguration<iOS, FormsElement> SetAdjustsFontSizeToFitWidth(this IPlatformElementConfiguration<iOS, FormsElement> config, bool value)
		{
			SetAdjustsFontSizeToFitWidth(config.Element, value);
			return config;
		}

		public static IPlatformElementConfiguration<iOS, FormsElement> EnableAdjustsFontSizeToFitWidth(this IPlatformElementConfiguration<iOS, FormsElement> config)
		{
			SetAdjustsFontSizeToFitWidth(config.Element, true);
			return config;
		}

		public static IPlatformElementConfiguration<iOS, FormsElement> DisableAdjustsFontSizeToFitWidth(this IPlatformElementConfiguration<iOS, FormsElement> config)
		{
			SetAdjustsFontSizeToFitWidth(config.Element, false);
			return config;
		}

		public static readonly BindableProperty DisabledSelectorActionsProperty = BindableProperty.Create(nameof(DisabledSelectorActions), typeof(List<SelectorAction>), typeof(Entry));

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

			if(disabledSelectorActions == null || disabledSelectorActions.Count == 0)
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