﻿namespace Xamarin.Forms
{
	public class StateTriggerBase : BindableObject
	{
		protected StateTriggerBase()
		{

		}

		internal bool IsTriggerActive { get; set; }

		internal VisualState VisualState { get; set; }

		protected void SetActive(bool active)
		{
			IsTriggerActive = active;

			VisualState?.VisualStateGroup?.UpdateStateTriggers();
		}
	}
}