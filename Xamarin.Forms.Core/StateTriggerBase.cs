using System;

namespace Xamarin.Forms
{
	public abstract class StateTriggerBase : BindableObject
	{
		bool _isTriggerActive;
		public event EventHandler IsTriggerActiveChanged;

		public StateTriggerBase()
		{
			ExperimentalFlags.VerifyFlagEnabled(nameof(IndicatorView), ExperimentalFlags.StateTriggersExperimental);
		}

		public bool IsTriggerActive
		{
			get => _isTriggerActive;
			private set
			{
				if (_isTriggerActive == value)
					return;

				_isTriggerActive = value;
				IsTriggerActiveChanged?.Invoke(this, EventArgs.Empty);
			}
		}

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