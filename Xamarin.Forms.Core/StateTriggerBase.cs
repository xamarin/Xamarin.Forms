namespace Xamarin.Forms
{
	public abstract class StateTriggerBase : BindableObject
	{
		public StateTriggerBase()
		{
			ExperimentalFlags.VerifyFlagEnabled(nameof(IndicatorView), ExperimentalFlags.StateTriggersExperimental);
		}

		internal bool IsTriggerActive { get; set; }

		internal VisualState VisualState { get; set; }

		protected void SetActive(bool active)
		{
			IsTriggerActive = active;

			VisualState?.VisualStateGroup?.UpdateStateTriggers();
		}

		internal virtual void OnAttached()
		{

		}

		internal virtual void OnDetached()
		{

		}
	}
}