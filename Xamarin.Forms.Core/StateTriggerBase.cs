using System.ComponentModel;

namespace Xamarin.Forms
{
	public abstract class StateTriggerBase : BindableObject
	{
		public StateTriggerBase()
		{
			ExperimentalFlags.VerifyFlagEnabled(nameof(IndicatorView), ExperimentalFlags.AdaptiveTriggersExperimental);
		}

		internal bool IsTriggerActive { get; set; }

		[EditorBrowsable(EditorBrowsableState.Never)]
		internal VisualState VisualState { get; set; }

		protected void SetActive(bool active)
		{
			IsTriggerActive = active;

			VisualState?.VisualStateGroup?.UpdateStateTriggers();
		}
	}
}